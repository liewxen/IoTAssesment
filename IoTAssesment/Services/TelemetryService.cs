using Microsoft.EntityFrameworkCore;
using IoTAssesment.Models;
using IoTAssesment.Interfaces;

namespace IoTAssesment.Services;

/// <summary>
/// Service for handling generic telemetry data operations
/// </summary>
public class TelemetryService : ITelemetryService
{
    private readonly IoTDeviceContext _context;
    private readonly ILogger<TelemetryService> _logger;
    private readonly Dictionary<string, int> _keyCache = new();

    public TelemetryService(IoTDeviceContext context, ILogger<TelemetryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Basic Telemetry Operations

    public async Task<bool> StoreValueAsync<T>(int deviceId, string keyName, T value, string? quality = "Good", string? context = null)
    {
        try
        {
            var keyId = await GetOrCreateKeyIdAsync(keyName, typeof(T));
            if (keyId == null)
            {
                _logger.LogError("Failed to get or create key ID for {KeyName}", keyName);
                return false;
            }

            var telemetry = new Telemetry
            {
                DeviceId = deviceId,
                KeyId = keyId.Value,
                Timestamp = DateTime.UtcNow,
                PartitionDate = DateTime.UtcNow.Date,
                Quality = quality,
                Context = context
            };

            telemetry.SetValue(value);

            _context.Telemetries.Add(telemetry);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Stored telemetry value for device {DeviceId}, key {KeyName}: {Value}", deviceId, keyName, value);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing telemetry value for device {DeviceId}, key {KeyName}", deviceId, keyName);
            return false;
        }
    }

    public async Task<T?> GetLatestValueAsync<T>(int deviceId, string keyName)
    {
        try
        {
            var keyId = await GetKeyIdAsync(keyName);
            if (keyId == null)
            {
                return default(T);
            }

            var telemetry = await _context.Telemetries
                .Where(t => t.DeviceId == deviceId && t.KeyId == keyId.Value)
                .OrderByDescending(t => t.Timestamp)
                .FirstOrDefaultAsync();

            if (telemetry == null)
            {
                return default(T);
            }

            return telemetry.GetValue<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest value for device {DeviceId}, key {KeyName}", deviceId, keyName);
            return default(T);
        }
    }

    public async Task<List<Telemetry>> GetDeviceTelemetryAsync(int deviceId, DateTime? fromDate = null, DateTime? toDate = null, int limit = 100)
    {
        try
        {
            var query = _context.Telemetries
                .Include(t => t.Key)
                .Where(t => t.DeviceId == deviceId);

            if (fromDate.HasValue)
                query = query.Where(t => t.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.Timestamp <= toDate.Value);

            return await query
                .OrderByDescending(t => t.Timestamp)
                .Take(limit)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting telemetry for device {DeviceId}", deviceId);
            return new List<Telemetry>();
        }
    }

    public async Task<List<Telemetry>> GetKeyTelemetryAsync(int deviceId, string keyName, DateTime? fromDate = null, DateTime? toDate = null, int limit = 100)
    {
        try
        {
            var keyId = await GetKeyIdAsync(keyName);
            if (keyId == null)
            {
                return new List<Telemetry>();
            }

            var query = _context.Telemetries
                .Include(t => t.Key)
                .Where(t => t.DeviceId == deviceId && t.KeyId == keyId.Value);

            if (fromDate.HasValue)
                query = query.Where(t => t.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.Timestamp <= toDate.Value);

            return await query
                .OrderByDescending(t => t.Timestamp)
                .Take(limit)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting telemetry for device {DeviceId}, key {KeyName}", deviceId, keyName);
            return new List<Telemetry>();
        }
    }

    #endregion

    #region Batch Operations

    public async Task<bool> StoreBatchValuesAsync(int deviceId, Dictionary<string, object> values, string? quality = "Good", string? context = null)
    {
        try
        {
            var telemetries = new List<Telemetry>();
            var timestamp = DateTime.UtcNow;
            var partitionDate = timestamp.Date;

            foreach (var kvp in values)
            {
                var keyId = await GetOrCreateKeyIdAsync(kvp.Key, kvp.Value?.GetType() ?? typeof(string));
                if (keyId == null) continue;

                var telemetry = new Telemetry
                {
                    DeviceId = deviceId,
                    KeyId = keyId.Value,
                    Timestamp = timestamp,
                    PartitionDate = partitionDate,
                    Quality = quality,
                    Context = context
                };

                telemetry.SetValue(kvp.Value);
                telemetries.Add(telemetry);
            }

            if (telemetries.Any())
            {
                _context.Telemetries.AddRange(telemetries);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Stored {Count} telemetry values for device {DeviceId}", telemetries.Count, deviceId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing batch telemetry values for device {DeviceId}", deviceId);
            return false;
        }
    }

    public async Task<Dictionary<string, object?>> GetLatestValuesAsync(int deviceId, List<string> keyNames)
    {
        try
        {
            var result = new Dictionary<string, object?>();

            foreach (var keyName in keyNames)
            {
                var keyId = await GetKeyIdAsync(keyName);
                if (keyId == null)
                {
                    result[keyName] = null;
                    continue;
                }

                var telemetry = await _context.Telemetries
                    .Where(t => t.DeviceId == deviceId && t.KeyId == keyId.Value)
                    .OrderByDescending(t => t.Timestamp)
                    .FirstOrDefaultAsync();

                if (telemetry != null)
                {
                    // Return the appropriate value based on which field is populated
                    result[keyName] = telemetry.DblValue ?? (object?)telemetry.LongValue ?? telemetry.StrValue ?? telemetry.JsonValue;
                }
                else
                {
                    result[keyName] = null;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest values for device {DeviceId}", deviceId);
            return new Dictionary<string, object?>();
        }
    }

    public async Task<Dictionary<string, object?>> GetAllLatestValuesAsync(int deviceId)
    {
        try
        {
            var latestTelemetry = await _context.Telemetries
                .Include(t => t.Key)
                .Where(t => t.DeviceId == deviceId)
                .GroupBy(t => t.KeyId)
                .Select(g => g.OrderByDescending(t => t.Timestamp).First())
                .ToListAsync();

            var result = new Dictionary<string, object?>();

            foreach (var telemetry in latestTelemetry)
            {
                var value = telemetry.DblValue ?? (object?)telemetry.LongValue ?? telemetry.StrValue ?? telemetry.JsonValue;
                result[telemetry.Key.KeyName] = value;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all latest values for device {DeviceId}", deviceId);
            return new Dictionary<string, object?>();
        }
    }

    #endregion

    #region Key Management

    public async Task<List<KeyDictionary>> GetAvailableKeysAsync(string? category = null)
    {
        try
        {
            var query = _context.KeyDictionaries.Where(k => k.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(k => k.Category == category);
            }

            return await query.OrderBy(k => k.KeyName).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available keys");
            return new List<KeyDictionary>();
        }
    }

    public async Task<KeyDictionary?> GetKeyDefinitionAsync(string keyName)
    {
        try
        {
            return await _context.KeyDictionaries
                .FirstOrDefaultAsync(k => k.KeyName == keyName && k.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting key definition for {KeyName}", keyName);
            return null;
        }
    }

    public async Task<bool> EnsureKeyExistsAsync(string keyName, string dataType, string? description = null, string? unit = null, string? category = null)
    {
        try
        {
            var existing = await _context.KeyDictionaries
                .FirstOrDefaultAsync(k => k.KeyName == keyName);

            if (existing == null)
            {
                var newKey = new KeyDictionary
                {
                    KeyName = keyName,
                    DataType = dataType,
                    Description = description ?? $"Auto-created key for {keyName}",
                    Unit = unit,
                    Category = category ?? "sensor",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.KeyDictionaries.Add(newKey);
                await _context.SaveChangesAsync();

                // Update cache
                _keyCache[keyName] = newKey.KeyId;

                _logger.LogInformation("Created new key definition: {KeyName} ({DataType})", keyName, dataType);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring key exists: {KeyName}", keyName);
            return false;
        }
    }

    #endregion

    #region Device-Specific Convenience Methods

    public async Task<double?> GetBatteryLevelAsync(int deviceId)
    {
        return await GetLatestValueAsync<double?>(deviceId, "battery_level");
    }

    public async Task<double?> GetTemperatureAsync(int deviceId)
    {
        return await GetLatestValueAsync<double?>(deviceId, "temperature");
    }

    public async Task<double?> GetHumidityAsync(int deviceId)
    {
        return await GetLatestValueAsync<double?>(deviceId, "humidity");
    }

    public async Task<string?> GetDeviceStatusAsync(int deviceId)
    {
        return await GetLatestValueAsync<string>(deviceId, "device_status");
    }

    #endregion

    #region Statistics and Aggregation

    public async Task<double?> GetAverageValueAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var keyId = await GetKeyIdAsync(keyName);
            if (keyId == null) return null;

            return await _context.Telemetries
                .Where(t => t.DeviceId == deviceId && t.KeyId == keyId.Value &&
                           t.Timestamp >= fromDate && t.Timestamp <= toDate &&
                           t.DblValue.HasValue)
                .AverageAsync(t => t.DblValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating average for device {DeviceId}, key {KeyName}", deviceId, keyName);
            return null;
        }
    }

    public async Task<double?> GetMinValueAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var keyId = await GetKeyIdAsync(keyName);
            if (keyId == null) return null;

            return await _context.Telemetries
                .Where(t => t.DeviceId == deviceId && t.KeyId == keyId.Value &&
                           t.Timestamp >= fromDate && t.Timestamp <= toDate &&
                           t.DblValue.HasValue)
                .MinAsync(t => t.DblValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating minimum for device {DeviceId}, key {KeyName}", deviceId, keyName);
            return null;
        }
    }

    public async Task<double?> GetMaxValueAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var keyId = await GetKeyIdAsync(keyName);
            if (keyId == null) return null;

            return await _context.Telemetries
                .Where(t => t.DeviceId == deviceId && t.KeyId == keyId.Value &&
                           t.Timestamp >= fromDate && t.Timestamp <= toDate &&
                           t.DblValue.HasValue)
                .MaxAsync(t => t.DblValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating maximum for device {DeviceId}, key {KeyName}", deviceId, keyName);
            return null;
        }
    }

    public async Task<int> GetDataPointCountAsync(int deviceId, string keyName, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var keyId = await GetKeyIdAsync(keyName);
            if (keyId == null) return 0;

            return await _context.Telemetries
                .Where(t => t.DeviceId == deviceId && t.KeyId == keyId.Value &&
                           t.Timestamp >= fromDate && t.Timestamp <= toDate)
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting data points for device {DeviceId}, key {KeyName}", deviceId, keyName);
            return 0;
        }
    }

    #endregion

    #region Cleanup and Maintenance

    public async Task<int> CleanupOldDataAsync(DateTime beforeDate)
    {
        try
        {
            var oldTelemetry = _context.Telemetries
                .Where(t => t.Timestamp < beforeDate);

            var count = await oldTelemetry.CountAsync();

            _context.Telemetries.RemoveRange(oldTelemetry);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} telemetry records before {Date}", count, beforeDate);

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old telemetry data");
            return 0;
        }
    }

    public async Task<bool> OptimizePartitionsAsync()
    {
        try
        {
            // This would contain partition-specific optimization logic
            // For now, just log that optimization was requested
            _logger.LogInformation("Partition optimization requested - implement based on PostgreSQL partition strategy");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing partitions");
            return false;
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task<int?> GetKeyIdAsync(string keyName)
    {
        // Check cache first
        if (_keyCache.TryGetValue(keyName, out var cachedId))
        {
            return cachedId;
        }

        try
        {
            var key = await _context.KeyDictionaries
                .FirstOrDefaultAsync(k => k.KeyName == keyName && k.IsActive);

            if (key != null)
            {
                _keyCache[keyName] = key.KeyId;
                return key.KeyId;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting key ID for {KeyName}", keyName);
            return null;
        }
    }

    private async Task<int?> GetOrCreateKeyIdAsync(string keyName, Type? valueType)
    {
        var keyId = await GetKeyIdAsync(keyName);
        if (keyId.HasValue)
        {
            return keyId;
        }

        // Auto-create key based on value type
        var dataType = GetDataTypeFromType(valueType);
        var success = await EnsureKeyExistsAsync(keyName, dataType, category: "sensor");

        return success ? await GetKeyIdAsync(keyName) : null;
    }

    private static string GetDataTypeFromType(Type? type)
    {
        if (type == null) return "string";

        return Type.GetTypeCode(type) switch
        {
            TypeCode.Double or TypeCode.Single or TypeCode.Decimal => "double",
            TypeCode.Int32 or TypeCode.Int64 or TypeCode.Int16 or TypeCode.Byte => "long",
            TypeCode.Boolean => "long", // Store as 0/1
            TypeCode.String => "string",
            _ => "json" // Complex objects
        };
    }

    #endregion
}