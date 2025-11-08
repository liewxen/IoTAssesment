namespace IoTAssesment.Interfaces;

/// <summary>
/// Interface for MQTT Service operations
/// </summary>
public interface IMqttService
{
    // Connection management
    Task<bool> StartAsync();
    Task<bool> StopAsync();
    Task<bool> IsConnectedAsync();

    // Publishing messages
    Task<bool> PublishDeviceCommandAsync(int deviceId, string command, object? payload = null);
    Task<bool> PublishStatusUpdateAsync(int deviceId, bool isOnline);
    Task<bool> PublishSensorDataRequestAsync(int deviceId);
    Task<bool> PublishFirmwareUpdateAsync(int deviceId, string firmwareVersion, string downloadUrl);

    // Subscription management
    Task<bool> SubscribeToDeviceTopicsAsync();
    Task<bool> UnsubscribeFromDeviceTopicsAsync();

    // Event handlers
    event EventHandler<DeviceStatusChangedEventArgs>? DeviceStatusChanged;
    event EventHandler<SensorDataReceivedEventArgs>? SensorDataReceived;
    event EventHandler<DeviceConnectedEventArgs>? DeviceConnected;
    event EventHandler<DeviceDisconnectedEventArgs>? DeviceDisconnected;
}

/// <summary>
/// Event arguments for device status changes
/// </summary>
public class DeviceStatusChangedEventArgs : EventArgs
{
    public int DeviceId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Event arguments for sensor data received
/// </summary>
public class SensorDataReceivedEventArgs : EventArgs
{
    public int DeviceId { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public double? BatteryLevel { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Event arguments for device connection
/// </summary>
public class DeviceConnectedEventArgs : EventArgs
{
    public int DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Event arguments for device disconnection
/// </summary>
public class DeviceDisconnectedEventArgs : EventArgs
{
    public int DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}