using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using IoTAssesment.Interfaces;

namespace IoTAssesment.Services;

/// <summary>
/// MQTT Service for device communication using MQTTnet
/// </summary>
public class MqttService : IMqttService, IDisposable
{
    private readonly ILogger<MqttService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDeviceService _deviceService;
    private readonly IDeviceLogService _logService;
    private readonly ITelemetryService _telemetryService;
    private IMqttClient? _mqttClient;
    private bool _disposed = false;

    // MQTT Configuration
    private readonly string _brokerHost;
    private readonly int _brokerPort;
    private readonly string _clientId;
    private readonly string? _username;
    private readonly string? _password;

    // Standardized topic patterns (client ID based identification)
    private const string TELEMETRY_TOPIC = "v1/telemetry";
    private const string STATUS_TOPIC = "v1/status";
    private const string COMMAND_TOPIC = "v1/command";
    private const string RESPONSE_TOPIC = "v1/response";
    private const string HEARTBEAT_TOPIC = "v1/heartbeat";
    private const string ERROR_TOPIC = "v1/error";
    private const string SYSTEM_TOPIC = "v1/system";

    // Events
    public event EventHandler<DeviceStatusChangedEventArgs>? DeviceStatusChanged;
    public event EventHandler<SensorDataReceivedEventArgs>? SensorDataReceived;
    public event EventHandler<DeviceConnectedEventArgs>? DeviceConnected;
    public event EventHandler<DeviceDisconnectedEventArgs>? DeviceDisconnected;

    public MqttService(ILogger<MqttService> logger, IConfiguration configuration, IDeviceService deviceService, IDeviceLogService logService, ITelemetryService telemetryService)
    {
        _logger = logger;
        _configuration = configuration;
        _deviceService = deviceService;
        _logService = logService;
        _telemetryService = telemetryService;

        // Load MQTT configuration
        _brokerHost = _configuration["MQTT:BrokerHost"] ?? "localhost";
        _brokerPort = int.Parse(_configuration["MQTT:BrokerPort"] ?? "1883");
        _clientId = _configuration["MQTT:ClientId"] ?? "IoTDeviceManager";
        _username = _configuration["MQTT:Username"];
        _password = _configuration["MQTT:Password"];
    }

    public async Task<bool> StartAsync()
    {
        try
        {
            var mqttFactory = new MqttFactory();

            var clientOptionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_brokerHost, _brokerPort)
                .WithClientId(_clientId)
                .WithCleanSession();

            if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
            {
                clientOptionsBuilder = clientOptionsBuilder.WithCredentials(_username, _password);
            }

            var clientOptions = clientOptionsBuilder.Build();

            _mqttClient = mqttFactory.CreateMqttClient();

            // Setup event handlers
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
            _mqttClient.ConnectedAsync += OnConnected;
            _mqttClient.DisconnectedAsync += OnDisconnected;

            await _mqttClient.ConnectAsync(clientOptions);

            // Subscribe to device topics
            await SubscribeToDeviceTopicsAsync();

            _logger.LogInformation("MQTT service started successfully. Connected to {BrokerHost}:{BrokerPort}", _brokerHost, _brokerPort);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MQTT service");
            return false;
        }
    }

    public async Task<bool> StopAsync()
    {
        try
        {
            if (_mqttClient != null)
            {
                await _mqttClient.DisconnectAsync();
                _mqttClient?.Dispose();
                _mqttClient = null;
            }

            _logger.LogInformation("MQTT service stopped successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop MQTT service");
            return false;
        }
    }

    public async Task<bool> IsConnectedAsync()
    {
        return _mqttClient?.IsConnected ?? false;
    }

    public async Task<bool> PublishDeviceCommandAsync(int deviceId, string command, object? payload = null)
    {
        try
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                _logger.LogWarning("MQTT client is not connected. Cannot publish command for device {DeviceId}", deviceId);
                return false;
            }

            var topic = COMMAND_TOPIC;
            var message = new
            {
                DeviceId = deviceId,
                Command = command,
                Payload = payload,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(message);
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(json)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(mqttMessage);

            await _logService.LogActionAsync(deviceId, "CommandSent", $"MQTT command '{command}' sent to device", "Success");

            _logger.LogInformation("MQTT command '{Command}' sent to device {DeviceId}", command, deviceId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish MQTT command for device {DeviceId}", deviceId);
            await _logService.LogActionAsync(deviceId, "CommandSent", $"Failed to send MQTT command '{command}'", "Error");
            return false;
        }
    }

    public async Task<bool> PublishStatusUpdateAsync(int deviceId, bool isOnline)
    {
        try
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                return false;
            }

            var topic = STATUS_TOPIC;
            var message = new
            {
                DeviceId = deviceId,
                IsOnline = isOnline,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(message);
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(json)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(mqttMessage);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish status update for device {DeviceId}", deviceId);
            return false;
        }
    }

    public async Task<bool> PublishSensorDataRequestAsync(int deviceId)
    {
        return await PublishDeviceCommandAsync(deviceId, "REQUEST_SENSOR_DATA");
    }

    public async Task<bool> PublishFirmwareUpdateAsync(int deviceId, string firmwareVersion, string downloadUrl)
    {
        return await PublishDeviceCommandAsync(deviceId, "FIRMWARE_UPDATE", new { FirmwareVersion = firmwareVersion, DownloadUrl = downloadUrl });
    }

    public async Task<bool> SubscribeToDeviceTopicsAsync()
    {
        try
        {
            if (_mqttClient == null)
            {
                return false;
            }

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(TELEMETRY_TOPIC)
                .WithTopicFilter(STATUS_TOPIC)
                .WithTopicFilter(HEARTBEAT_TOPIC)
                .WithTopicFilter(ERROR_TOPIC)
                .WithTopicFilter(SYSTEM_TOPIC)
                .Build();

            await _mqttClient.SubscribeAsync(subscribeOptions);

            _logger.LogInformation("Subscribed to device topics");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to device topics");
            return false;
        }
    }

    public async Task<bool> UnsubscribeFromDeviceTopicsAsync()
    {
        try
        {
            if (_mqttClient == null)
            {
                return false;
            }

            var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                .WithTopicFilter("devices/+/status")
                .WithTopicFilter("devices/+/sensor")
                .WithTopicFilter("devices/+/response")
                .WithTopicFilter($"{SYSTEM_TOPIC}/+")
                .Build();

            await _mqttClient.UnsubscribeAsync(unsubscribeOptions);

            _logger.LogInformation("Unsubscribed from device topics");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe from device topics");
            return false;
        }
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            _logger.LogDebug("Received MQTT message on topic: {Topic}, Payload: {Payload}", topic, payload);

            await ProcessReceivedMessage(topic, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing received MQTT message");
        }
    }

    private async Task ProcessReceivedMessage(string topic, string payload)
    {
        string? clientId = null;
        try
        {
            // Extract client ID from the payload
            try
            {
                var jsonDoc = JsonDocument.Parse(payload);
                if (jsonDoc.RootElement.TryGetProperty("clientid", out var clientIdElement))
                {
                    clientId = clientIdElement.GetString();
                }
                else if (jsonDoc.RootElement.TryGetProperty("clientId", out var clientIdElement2))
                {
                    clientId = clientIdElement2.GetString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse client ID from payload: {Payload}", payload);
            }

            if (string.IsNullOrEmpty(clientId))
            {
                _logger.LogWarning("No client ID found in payload for topic: {Topic}", topic);
                return;
            }

            // Get device by client ID from the payload
            var device = await _deviceService.GetDeviceByClientIdAsync(clientId);
            if (device == null)
            {
                _logger.LogWarning("Unknown device with client ID: {ClientId} from topic: {Topic}", clientId, topic);
                return;
            }

            var deviceId = device.Id;
            _logger.LogInformation("Processing message for device {DeviceId} (ClientId: {ClientId}) on topic: {Topic}", deviceId, clientId, topic);

            // Handle standardized topics
            switch (topic)
            {
                case TELEMETRY_TOPIC:
                    await ProcessTelemetryMessage(deviceId, payload, clientId);
                    break;
                case STATUS_TOPIC:
                    await ProcessStatusMessage(deviceId, payload);
                    break;
                case HEARTBEAT_TOPIC:
                    await ProcessHeartbeatMessage(deviceId, clientId);
                    break;
                case ERROR_TOPIC:
                    await ProcessErrorMessage(deviceId, payload, clientId);
                    break;
                default:
                    _logger.LogDebug("Unhandled topic: {Topic} from client: {ClientId}", topic, clientId);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MQTT message for topic: {Topic} from client: {ClientId}", topic, clientId);
        }
    }

    private async Task ProcessStatusMessage(int deviceId, string payload)
    {
        try
        {
            var statusData = JsonSerializer.Deserialize<JsonDocument>(payload);
            var isOnline = statusData?.RootElement.GetProperty("IsOnline").GetBoolean() ?? false;

            await _deviceService.UpdateDeviceStatusAsync(deviceId, isOnline);

            DeviceStatusChanged?.Invoke(this, new DeviceStatusChangedEventArgs
            {
                DeviceId = deviceId,
                IsOnline = isOnline,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Device {DeviceId} status updated to {Status}", deviceId, isOnline ? "Online" : "Offline");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing status message for device {DeviceId}", deviceId);
        }
    }

    private async Task ProcessTelemetryMessage(int deviceId, string payload, string clientId)
    {
        try
        {
            var telemetryData = JsonSerializer.Deserialize<JsonDocument>(payload);
            var root = telemetryData?.RootElement;

            if (!root.HasValue)
            {
                _logger.LogWarning("Empty telemetry payload from client {ClientId} for device {DeviceId}", clientId, deviceId);
                return;
            }

            // Parse all properties from the JSON payload
            var telemetryValues = new Dictionary<string, object>();
            int processedCount = 0;

            foreach (var property in root.Value.EnumerateObject())
            {
                var keyName = property.Name.ToLower();
                
                // Skip metadata fields
                if (keyName == "timestamp" || keyName == "clientid" || keyName == "messageid")
                    continue;

                try
                {
                    // Determine value type and extract
                    object? value = property.Value.ValueKind switch
                    {
                        JsonValueKind.Number => property.Value.TryGetInt64(out var longVal) ? longVal : property.Value.GetDouble(),
                        JsonValueKind.String => property.Value.GetString(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => null,
                        _ => property.Value.GetRawText() // Complex objects as JSON string
                    };

                    if (value != null)
                    {
                        telemetryValues[keyName] = value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse telemetry key {KeyName} from client {ClientId}", property.Name, clientId);
                }
            }

            // Store telemetry data using TelemetryService
            // This will automatically handle key dictionary creation if keys don't exist
            if (telemetryValues.Any())
            {
                var success = await _telemetryService.StoreBatchValuesAsync(
                    deviceId, 
                    telemetryValues, 
                    quality: "Good", 
                    context: $"MQTT from {clientId}"
                );

                if (success)
                {
                    processedCount = telemetryValues.Count;
                    _logger.LogInformation("Stored {Count} telemetry values from client {ClientId} for device {DeviceId}: {Keys}", 
                        processedCount, clientId, deviceId, string.Join(", ", telemetryValues.Keys));

                    // Also update legacy sensor data fields for backward compatibility
                    await UpdateLegacySensorData(deviceId, telemetryValues);

                    // Raise event for real-time updates
                    SensorDataReceived?.Invoke(this, new SensorDataReceivedEventArgs
                    {
                        DeviceId = deviceId,
                        Temperature = telemetryValues.ContainsKey("temperature") ? Convert.ToDouble(telemetryValues["temperature"]) : null,
                        Humidity = telemetryValues.ContainsKey("humidity") ? Convert.ToDouble(telemetryValues["humidity"]) : null,
                        BatteryLevel = telemetryValues.ContainsKey("batterylevel") || telemetryValues.ContainsKey("battery_level") 
                            ? Convert.ToDouble(telemetryValues.ContainsKey("batterylevel") ? telemetryValues["batterylevel"] : telemetryValues["battery_level"]) 
                            : null,
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    _logger.LogError("Failed to store telemetry data from client {ClientId} for device {DeviceId}", clientId, deviceId);
                }
            }
            else
            {
                _logger.LogWarning("No valid telemetry values found in payload from client {ClientId} for device {DeviceId}", clientId, deviceId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing telemetry message from client {ClientId} for device {DeviceId}", clientId, deviceId);
        }
    }

    private async Task UpdateLegacySensorData(int deviceId, Dictionary<string, object> telemetryValues)
    {
        try
        {
            // Extract common sensor values for backward compatibility with existing UI
            double? temperature = null;
            double? humidity = null;
            double? batteryLevel = null;

            if (telemetryValues.ContainsKey("temperature"))
                temperature = Convert.ToDouble(telemetryValues["temperature"]);

            if (telemetryValues.ContainsKey("humidity"))
                humidity = Convert.ToDouble(telemetryValues["humidity"]);

            if (telemetryValues.ContainsKey("batterylevel"))
                batteryLevel = Convert.ToDouble(telemetryValues["batterylevel"]);
            else if (telemetryValues.ContainsKey("battery_level"))
                batteryLevel = Convert.ToDouble(telemetryValues["battery_level"]);
            else if (telemetryValues.ContainsKey("battery"))
                batteryLevel = Convert.ToDouble(telemetryValues["battery"]);

            // Only update if we have at least one value
            if (temperature.HasValue || humidity.HasValue || batteryLevel.HasValue)
            {
                await _deviceService.UpdateDeviceSensorDataAsync(deviceId, temperature, humidity, batteryLevel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error updating legacy sensor data for device {DeviceId}", deviceId);
        }
    }

    private async Task ProcessHeartbeatMessage(int deviceId, string clientId)
    {
        try
        {
            // Update device status to online when heartbeat is received
            await _deviceService.UpdateDeviceStatusAsync(deviceId, true);

            _logger.LogDebug("Received heartbeat from client {ClientId} for device {DeviceId}", clientId, deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing heartbeat message from client {ClientId} for device {DeviceId}", clientId, deviceId);
        }
    }

    private async Task ProcessErrorMessage(int deviceId, string payload, string clientId)
    {
        try
        {
            var errorData = JsonSerializer.Deserialize<JsonDocument>(payload);
            var root = errorData?.RootElement;

            var errorMessage = root?.GetProperty("message").GetString() ?? "Unknown error";
            var errorCode = root?.GetProperty("code").GetString() ?? "ERR_UNKNOWN";

            // Log the error using existing logging
            await _logService.LogActionAsync(deviceId, "Error", $"Device error: {errorMessage} (Code: {errorCode})");

            _logger.LogWarning("Device error from client {ClientId} for device {DeviceId}: {ErrorMessage} (Code: {ErrorCode})",
                clientId, deviceId, errorMessage, errorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing error message from client {ClientId} for device {DeviceId}", clientId, deviceId);
        }
    }

    private async Task ProcessSensorMessage(int deviceId, string payload)
    {
        try
        {
            var sensorData = JsonSerializer.Deserialize<JsonDocument>(payload);
            var root = sensorData?.RootElement;

            double? temperature = null;
            double? humidity = null;
            double? batteryLevel = null;

            if (root?.TryGetProperty("Temperature", out var tempElement) == true)
                temperature = tempElement.GetDouble();

            if (root?.TryGetProperty("Humidity", out var humElement) == true)
                humidity = humElement.GetDouble();

            if (root?.TryGetProperty("BatteryLevel", out var battElement) == true)
                batteryLevel = battElement.GetDouble();

            await _deviceService.UpdateDeviceSensorDataAsync(deviceId, temperature, humidity, batteryLevel);

            SensorDataReceived?.Invoke(this, new SensorDataReceivedEventArgs
            {
                DeviceId = deviceId,
                Temperature = temperature,
                Humidity = humidity,
                BatteryLevel = batteryLevel,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Sensor data received for device {DeviceId}", deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing sensor message for device {DeviceId}", deviceId);
        }
    }

    private async Task ProcessResponseMessage(int deviceId, string payload)
    {
        try
        {
            await _logService.LogActionAsync(deviceId, "ResponseReceived", $"Device response: {payload}", "Success");

            _logger.LogInformation("Response received from device {DeviceId}", deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing response message for device {DeviceId}", deviceId);
        }
    }

    private async Task OnConnected(MqttClientConnectedEventArgs e)
    {
        _logger.LogInformation("MQTT client connected successfully");
    }

    private async Task OnDisconnected(MqttClientDisconnectedEventArgs e)
    {
        _logger.LogWarning("MQTT client disconnected. Reason: {Reason}", e.Reason);
    }

    // Note: ConnectingFailed event is not available in basic MQTTnet client
    // Connection failures will be handled through exceptions in ConnectAsync

    public void Dispose()
    {
        if (!_disposed)
        {
            _mqttClient?.Dispose();
            _disposed = true;
        }
    }
}