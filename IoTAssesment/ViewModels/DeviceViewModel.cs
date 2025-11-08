using System.ComponentModel.DataAnnotations;

namespace IoTAssesment.ViewModels;

/// <summary>
/// ViewModel for IoT Device operations (frontend-backend interaction layer)
/// </summary>
public class DeviceViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Device name is required")]
    [StringLength(100, ErrorMessage = "Device name cannot exceed 100 characters")]
    [Display(Name = "Device Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device type is required")]
    [StringLength(50, ErrorMessage = "Device type cannot exceed 50 characters")]
    [Display(Name = "Device Type")]
    public string DeviceType { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Online Status")]
    public bool IsOnline { get; set; }

    [Display(Name = "Last Seen")]
    public DateTime LastSeen { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Updated Date")]
    public DateTime UpdatedAt { get; set; }

    [StringLength(100, ErrorMessage = "Serial number cannot exceed 100 characters")]
    [Display(Name = "Serial Number")]
    public string? SerialNumber { get; set; }

    [StringLength(50, ErrorMessage = "Firmware version cannot exceed 50 characters")]
    [Display(Name = "Firmware Version")]
    public string? FirmwareVersion { get; set; }

    [Range(0, 100, ErrorMessage = "Battery level must be between 0 and 100")]
    [Display(Name = "Battery Level (%)")]
    public double? BatteryLevel { get; set; }

    [Range(-50, 100, ErrorMessage = "Temperature must be between -50 and 100 degrees")]
    [Display(Name = "Temperature (°C)")]
    public double? Temperature { get; set; }

    [Range(0, 100, ErrorMessage = "Humidity must be between 0 and 100%")]
    [Display(Name = "Humidity (%)")]
    public double? Humidity { get; set; }

    [StringLength(100, ErrorMessage = "Manufacturer name cannot exceed 100 characters")]
    [Display(Name = "Manufacturer")]
    public string? ManufacturerName { get; set; }

    [StringLength(100, ErrorMessage = "Model number cannot exceed 100 characters")]
    [Display(Name = "Model Number")]
    public string? ModelNumber { get; set; }

    // MQTT Credentials and Connection
    [Required(ErrorMessage = "Client ID is required")]
    [StringLength(100, ErrorMessage = "Client ID cannot exceed 100 characters")]
    [Display(Name = "MQTT Client ID")]
    public string ClientId { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "MQTT username cannot exceed 100 characters")]
    [Display(Name = "MQTT Username")]
    public string? MqttUsername { get; set; }

    [StringLength(255, ErrorMessage = "MQTT password cannot exceed 255 characters")]
    [Display(Name = "MQTT Password")]
    [DataType(DataType.Password)]
    public string? MqttPassword { get; set; } // Plain text for UI, will be hashed on save

    [StringLength(50, ErrorMessage = "API key cannot exceed 50 characters")]
    [Display(Name = "API Key")]
    public string ApiKey { get; set; } = string.Empty; // Auto-generated on device creation

    [Display(Name = "Last MQTT Connection")]
    public DateTime LastMqttConnection { get; set; }

    [Display(Name = "Connection Status")]
    public string ConnectionStatus { get; set; } = "Never Connected";

    // Additional properties for UI display
    [Display(Name = "Status")]
    public string Status => IsOnline ? "Online" : "Offline";

    [Display(Name = "Connection Quality")]
    public string ConnectionQuality
    {
        get
        {
            if (!IsOnline) return "Disconnected";

            var minutesSinceLastSeen = (DateTime.UtcNow - LastSeen).TotalMinutes;
            return minutesSinceLastSeen switch
            {
                < 1 => "Excellent",
                < 5 => "Good",
                < 15 => "Fair",
                _ => "Poor"
            };
        }
    }

    [Display(Name = "Battery Status")]
    public string BatteryStatus
    {
        get
        {
            if (!BatteryLevel.HasValue) return "Unknown";

            return BatteryLevel.Value switch
            {
                > 75 => "High",
                > 50 => "Medium",
                > 25 => "Low",
                > 10 => "Very Low",
                _ => "Critical"
            };
        }
    }
}

/// <summary>
/// ViewModel for device listing with pagination and filtering
/// </summary>
public class DeviceListViewModel
{
    public List<DeviceViewModel> Devices { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    // Filtering properties
    public string? SearchTerm { get; set; }
    public string? DeviceTypeFilter { get; set; }
    public bool? OnlineStatusFilter { get; set; }
    public string? LocationFilter { get; set; }

    // Sorting properties
    public string SortBy { get; set; } = "Name";
    public string SortDirection { get; set; } = "asc";

    // Statistics
    public int OnlineDevicesCount { get; set; }
    public int OfflineDevicesCount { get; set; }
    public List<string> DeviceTypes { get; set; } = new();
    public List<string> Locations { get; set; } = new();
}

/// <summary>
/// ViewModel for creating/editing devices
/// </summary>
public class DeviceFormViewModel
{
    public DeviceViewModel Device { get; set; } = new();
    public List<string> AvailableDeviceTypes { get; set; } = new()
    {
        "Temperature Sensor",
        "Humidity Sensor",
        "Environmental Sensor",
        "Access Control",
        "Security Sensor",
        "Motion Detector",
        "Smart Lock",
        "Camera",
        "Air Quality Monitor",
        "Pressure Sensor",
        "Light Sensor",
        "Sound Sensor"
    };
    public bool IsEdit => Device.Id > 0;
}