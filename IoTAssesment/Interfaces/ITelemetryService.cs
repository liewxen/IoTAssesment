using IoTAssesment.Models;

namespace IoTAssesment.Interfaces;

/// <summary>
/// Interface for telemetry data operations with generic data types
/// </summary>
public interface ITelemetryService
{
    // Basic telemetry operations
    Task<bool> StoreValueAsync<T>(int deviceId, string keyName, T value, string? quality = "Good", string? context = null);
    Task<T?> GetLatestValueAsync<T>(int deviceId, string keyName);
    Task<List<Telemetry>> GetDeviceTelemetryAsync(int deviceId, DateTime? fromDate = null, DateTime? toDate = null, int limit = 100);
    Task<List<Telemetry>> GetKeyTelemetryAsync(int deviceId, string keyName, DateTime? fromDate = null, DateTime? toDate = null, int limit = 100);

    // Batch operations
    Task<bool> StoreBatchValuesAsync(int deviceId, Dictionary<string, object> values, string? quality = "Good", string? context = null);
    Task<Dictionary<string, object?>> GetLatestValuesAsync(int deviceId, List<string> keyNames);
    Task<Dictionary<string, object?>> GetAllLatestValuesAsync(int deviceId);

    // Key management
    Task<List<KeyDictionary>> GetAvailableKeysAsync(string? category = null);
    Task<KeyDictionary?> GetKeyDefinitionAsync(string keyName);
    Task<bool> EnsureKeyExistsAsync(string keyName, string dataType, string? description = null, string? unit = null, string? category = null);

    // Device-specific queries
    Task<double?> GetBatteryLevelAsync(int deviceId);
    Task<double?> GetTemperatureAsync(int deviceId);
    Task<double?> GetHumidityAsync(int deviceId);
    Task<string?> GetDeviceStatusAsync(int deviceId);

    // Statistics and aggregation
    Task<double?> GetAverageValueAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate);
    Task<double?> GetMinValueAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate);
    Task<double?> GetMaxValueAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate);
    Task<int> GetDataPointCountAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate);

    // Cleanup and maintenance
    Task<int> CleanupOldDataAsync(DateTime beforeDate);
    Task<bool> OptimizePartitionsAsync();
}