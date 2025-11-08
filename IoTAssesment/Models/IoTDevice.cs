using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTAssesment.Models;

/// <summary>
/// Represents an IoT device in the system with basic metadata only
/// All sensor data and dynamic properties are stored in the Telemetry table
/// </summary>
public class IoTDevice
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string DeviceType { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    public bool IsOnline { get; set; } = false;

    public DateTime LastSeen { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    [StringLength(50)]
    public string? FirmwareVersion { get; set; }

    [StringLength(100)]
    public string? ManufacturerName { get; set; }

    [StringLength(100)]
    public string? ModelNumber { get; set; }

    [StringLength(1000)]
    public string? ConfigurationJson { get; set; } // For device-specific configuration

    // MQTT Credentials and Connection
    [Required]
    [StringLength(100)]
    public string ClientId { get; set; } = string.Empty; // Unique MQTT client ID

    [StringLength(100)]
    public string? MqttUsername { get; set; }

    [StringLength(255)]
    public string? MqttPasswordHash { get; set; } // Encrypted/hashed password

    [StringLength(50)]
    public string ApiKey { get; set; } = string.Empty; // For API authentication

    public DateTime LastMqttConnection { get; set; } = DateTime.MinValue;

    [StringLength(50)]
    public string ConnectionStatus { get; set; } = "Never Connected"; // Never Connected, Connected, Disconnected, Error

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<DeviceLog> DeviceLogs { get; set; } = new List<DeviceLog>();
    public virtual ICollection<Telemetry> Telemetries { get; set; } = new List<Telemetry>();
}