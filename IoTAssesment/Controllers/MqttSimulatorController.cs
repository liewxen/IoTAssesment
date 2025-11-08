using Microsoft.AspNetCore.Mvc;
using IoTAssesment.Interfaces;
using System.Text.Json;

namespace IoTAssesment.Controllers;

/// <summary>
/// Controller for simulating MQTT messages without needing an actual MQTT broker
/// Perfect for testing and demonstrations
/// </summary>
public class MqttSimulatorController : Controller
{
    private readonly IDeviceService _deviceService;
    private readonly ITelemetryService _telemetryService;
    private readonly IDeviceLogService _logService;
    private readonly ILogger<MqttSimulatorController> _logger;

    public MqttSimulatorController(
        IDeviceService deviceService,
        ITelemetryService telemetryService,
        IDeviceLogService logService,
        ILogger<MqttSimulatorController> logger)
    {
        _deviceService = deviceService;
        _telemetryService = telemetryService;
        _logService = logService;
        _logger = logger;
    }

    /// <summary>
    /// Display the MQTT simulator page
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // Get all active devices for dropdown
        var devices = await _deviceService.GetDevicesAsync(1, 100, null, null, null, null, "Name", "asc");
        ViewBag.Devices = devices.Devices;
        return View();
    }

    /// <summary>
    /// Simulate sending an MQTT message
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SimulateMqtt(string topic, string payload)
    {
        try
        {
            if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(payload))
            {
                return BadRequest(new { success = false, message = "Topic and payload are required" });
            }

            _logger.LogInformation("Simulating MQTT message on topic: {Topic}, Payload: {Payload}", topic, payload);

            // Parse the JSON payload
            JsonDocument jsonDoc;
            try
            {
                jsonDoc = JsonDocument.Parse(payload);
            }
            catch (JsonException)
            {
                return BadRequest(new { success = false, message = "Invalid JSON payload" });
            }

            // Extract client ID from payload
            string? clientId = null;
            if (jsonDoc.RootElement.TryGetProperty("clientid", out var clientIdElement))
            {
                clientId = clientIdElement.GetString();
            }
            else if (jsonDoc.RootElement.TryGetProperty("clientId", out var clientIdElement2))
            {
                clientId = clientIdElement2.GetString();
            }

            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest(new { success = false, message = "No 'clientid' found in payload" });
            }

            // Get device by client ID
            var device = await _deviceService.GetDeviceByClientIdAsync(clientId);
            if (device == null)
            {
                return NotFound(new { success = false, message = $"No device found with Client ID: {clientId}" });
            }

            // Process based on topic
            string result = "";
            switch (topic.ToLower())
            {
                case "v1/telemetry":
                    result = await ProcessTelemetryMessage(device.Id, jsonDoc.RootElement, clientId);
                    break;

                case "v1/status":
                    result = await ProcessStatusMessage(device.Id, jsonDoc.RootElement);
                    break;

                case "v1/heartbeat":
                    result = await ProcessHeartbeatMessage(device.Id, clientId);
                    break;

                case "v1/error":
                    result = await ProcessErrorMessage(device.Id, jsonDoc.RootElement, clientId);
                    break;

                default:
                    return BadRequest(new { success = false, message = $"Unknown topic: {topic}" });
            }

            return Ok(new 
            { 
                success = true, 
                message = result,
                deviceId = device.Id,
                deviceName = device.Name
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error simulating MQTT message");
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    private async Task<string> ProcessTelemetryMessage(int deviceId, JsonElement root, string clientId)
    {
        var telemetryValues = new Dictionary<string, object>();
        int processedCount = 0;

        foreach (var property in root.EnumerateObject())
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
                    _ => property.Value.GetRawText()
                };

                if (value != null)
                {
                    telemetryValues[keyName] = value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse telemetry key {KeyName}", property.Name);
            }
        }

        if (telemetryValues.Any())
        {
            var success = await _telemetryService.StoreBatchValuesAsync(
                deviceId,
                telemetryValues,
                quality: "Good",
                context: $"Simulated MQTT from {clientId}"
            );

            if (success)
            {
                processedCount = telemetryValues.Count;
                _logger.LogInformation("Stored {Count} telemetry values from simulated client {ClientId} for device {DeviceId}: {Keys}",
                    processedCount, clientId, deviceId, string.Join(", ", telemetryValues.Keys));

                // Update legacy sensor data fields
                await UpdateLegacySensorData(deviceId, telemetryValues);

                await _logService.LogActionAsync(deviceId, "TelemetryReceived",
                    $"Simulated telemetry data received: {string.Join(", ", telemetryValues.Keys)}", "Success");

                return $"Successfully stored {processedCount} telemetry values: {string.Join(", ", telemetryValues.Keys)}";
            }
            else
            {
                return "Failed to store telemetry data";
            }
        }
        else
        {
            return "No valid telemetry values found in payload";
        }
    }

    private async Task<string> ProcessStatusMessage(int deviceId, JsonElement root)
    {
        if (root.TryGetProperty("status", out var statusElement))
        {
            var status = statusElement.GetString()?.ToLower();
            var isOnline = status == "online";

            await _deviceService.UpdateDeviceStatusAsync(deviceId, isOnline);
            await _logService.LogActionAsync(deviceId, "StatusUpdate",
                $"Device status updated to {(isOnline ? "Online" : "Offline")} via simulator", "Success");

            return $"Device status updated to {(isOnline ? "Online" : "Offline")}";
        }

        return "No status field found in payload";
    }

    private async Task<string> ProcessHeartbeatMessage(int deviceId, string clientId)
    {
        await _deviceService.UpdateDeviceStatusAsync(deviceId, true);
        await _logService.LogActionAsync(deviceId, "Heartbeat",
            "Heartbeat received via simulator", "Success");

        return $"Heartbeat processed for device {deviceId}";
    }

    private async Task<string> ProcessErrorMessage(int deviceId, JsonElement root, string clientId)
    {
        var errorMessage = root.TryGetProperty("message", out var msgElement) 
            ? msgElement.GetString() ?? "Unknown error" 
            : "Unknown error";
        var errorCode = root.TryGetProperty("code", out var codeElement) 
            ? codeElement.GetString() ?? "ERR_UNKNOWN" 
            : "ERR_UNKNOWN";

        await _logService.LogActionAsync(deviceId, "Error",
            $"Device error (simulated): {errorMessage} (Code: {errorCode})", "Error");

        return $"Error logged: {errorMessage}";
    }

    private async Task UpdateLegacySensorData(int deviceId, Dictionary<string, object> telemetryValues)
    {
        try
        {
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
}

