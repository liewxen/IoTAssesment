using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IoTAssesment.Models;

/// <summary>
/// Represents telemetry data with generic storage for different data types
/// This allows for flexible and scalable IoT data storage
/// </summary>
[Table("Telemetries")]
public class Telemetry
{
    [Key]
    public long Id { get; set; }

    [Required]
    public int DeviceId { get; set; }

    [Required]
    public int KeyId { get; set; }

    // Generic value storage for different data types
    public double? DblValue { get; set; } // For numeric values (temperature, humidity, battery, etc.)

    public long? LongValue { get; set; } // For integer values (counts, IDs, timestamps, etc.)

    [StringLength(1000)]
    public string? StrValue { get; set; } // For string values (status, names, etc.)

    public string? JsonValue { get; set; } // For complex JSON objects

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Quality { get; set; } // Data quality indicator (Good, Bad, Uncertain)

    [StringLength(500)]
    public string? Context { get; set; } // Additional context or metadata

    // Partitioning key for time-based partitioning
    [Column("partition_date")]
    public DateTime PartitionDate { get; set; } = DateTime.UtcNow.Date;

    // Foreign key relationships
    [ForeignKey("DeviceId")]
    public virtual IoTDevice Device { get; set; } = null!;

    [ForeignKey("KeyId")]
    public virtual KeyDictionary Key { get; set; } = null!;

    // Helper methods to get typed values
    public T? GetValue<T>()
    {
        return typeof(T) switch
        {
            Type t when t == typeof(double) || t == typeof(double?) => (T?)(object?)DblValue,
            Type t when t == typeof(float) || t == typeof(float?) => (T?)(object?)(float?)DblValue,
            Type t when t == typeof(int) || t == typeof(int?) => (T?)(object?)(int?)LongValue,
            Type t when t == typeof(long) || t == typeof(long?) => (T?)(object?)LongValue,
            Type t when t == typeof(string) => (T?)(object?)StrValue,
            Type t when t == typeof(bool) || t == typeof(bool?) => (T?)(object?)(LongValue == 1),
            _ => throw new ArgumentException($"Type {typeof(T)} is not supported")
        };
    }

    public void SetValue<T>(T value)
    {
        switch (value)
        {
            case double d:
                DblValue = d;
                break;
            case float f:
                DblValue = f;
                break;
            case int i:
                LongValue = i;
                break;
            case long l:
                LongValue = l;
                break;
            case string s:
                StrValue = s;
                break;
            case bool b:
                LongValue = b ? 1 : 0;
                break;
            case null:
                DblValue = null;
                LongValue = null;
                StrValue = null;
                JsonValue = null;
                break;
            default:
                JsonValue = System.Text.Json.JsonSerializer.Serialize(value);
                break;
        }
    }
}