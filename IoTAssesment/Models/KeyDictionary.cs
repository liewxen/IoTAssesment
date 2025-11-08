using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTAssesment.Models;

/// <summary>
/// Represents a key dictionary for storing device attribute definitions
/// This allows for dynamic and extensible device properties
/// </summary>
public class KeyDictionary
{
    [Key]
    public int KeyId { get; set; }

    [Required]
    [StringLength(100)]
    public string KeyName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(20)]
    public string DataType { get; set; } = string.Empty; // "double", "long", "string", "json"

    [StringLength(50)]
    public string? Unit { get; set; } // e.g., "°C", "%", "V", etc.

    [StringLength(100)]
    public string? Category { get; set; } // e.g., "sensor", "config", "status", "metadata"

    public bool IsRequired { get; set; } = false;

    public double? MinValue { get; set; }

    public double? MaxValue { get; set; }

    [StringLength(1000)]
    public string? DefaultValue { get; set; }

    [StringLength(1000)]
    public string? ValidationRules { get; set; } // JSON string for complex validation

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<Telemetry> Telemetries { get; set; } = new List<Telemetry>();
}