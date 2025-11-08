using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTAssesment.Models;

/// <summary>
/// Represents a log entry for device actions and events
/// </summary>
public class DeviceLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DeviceId { get; set; }

    [Required]
    [StringLength(50)]
    public string Action { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Status { get; set; }

    [StringLength(100)]
    public string? UserAgent { get; set; }

    // Foreign key relationship
    [ForeignKey("DeviceId")]
    public virtual IoTDevice Device { get; set; } = null!;
}