using Microsoft.EntityFrameworkCore;
using IoTAssesment.Models;
using IoTAssesment.ViewModels;
using IoTAssesment.Interfaces;

namespace IoTAssesment.Services;

/// <summary>
/// Service class containing business logic for IoT Device operations with telemetry support
/// </summary>
public class DeviceService : IDeviceService
{
    private readonly IoTDeviceContext _context;
    private readonly ILogger<DeviceService> _logger;
    private readonly IDeviceLogService _logService;
    private readonly ITelemetryService _telemetryService;

    public DeviceService(IoTDeviceContext context, ILogger<DeviceService> logger, IDeviceLogService logService, ITelemetryService telemetryService)
    {
        _context = context;
        _logger = logger;
        _logService = logService;
        _telemetryService = telemetryService;
    }

    public async Task<DeviceListViewModel> GetDevicesAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? deviceTypeFilter = null,
        bool? onlineStatusFilter = null,
        string? locationFilter = null,
        string sortBy = "Name",
        string sortDirection = "asc")
    {
        try
        {
            var query = _context.IoTDevices.Where(d => d.IsActive).AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d =>
                    d.Name.Contains(searchTerm) ||
                    d.Description!.Contains(searchTerm) ||
                    d.SerialNumber!.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(deviceTypeFilter))
            {
                query = query.Where(d => d.DeviceType == deviceTypeFilter);
            }

            if (onlineStatusFilter.HasValue)
            {
                query = query.Where(d => d.IsOnline == onlineStatusFilter.Value);
            }

            if (!string.IsNullOrEmpty(locationFilter))
            {
                query = query.Where(d => d.Location.Contains(locationFilter));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "name" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                "devicetype" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(d => d.DeviceType) : query.OrderBy(d => d.DeviceType),
                "location" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(d => d.Location) : query.OrderBy(d => d.Location),
                "isonline" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(d => d.IsOnline) : query.OrderBy(d => d.IsOnline),
                "lastseen" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(d => d.LastSeen) : query.OrderBy(d => d.LastSeen),
                "createdat" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(d => d.CreatedAt) : query.OrderBy(d => d.CreatedAt),
                _ => query.OrderBy(d => d.Name)
            };

            var totalCount = await query.CountAsync();

            var devices = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Convert to ViewModels with telemetry data
            var deviceViewModels = new List<DeviceViewModel>();
            foreach (var device in devices)
            {
                var viewModel = await ConvertToViewModelWithTelemetryAsync(device);
                deviceViewModels.Add(viewModel);
            }

            var onlineCount = await _context.IoTDevices.CountAsync(d => d.IsOnline && d.IsActive);
            var offlineCount = await _context.IoTDevices.CountAsync(d => !d.IsOnline && d.IsActive);
            var deviceTypes = await _context.IoTDevices.Where(d => d.IsActive).Select(d => d.DeviceType).Distinct().ToListAsync();
            var locations = await _context.IoTDevices.Where(d => d.IsActive).Select(d => d.Location).Distinct().ToListAsync();

            return new DeviceListViewModel
            {
                Devices = deviceViewModels,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                DeviceTypeFilter = deviceTypeFilter,
                OnlineStatusFilter = onlineStatusFilter,
                LocationFilter = locationFilter,
                SortBy = sortBy,
                SortDirection = sortDirection,
                OnlineDevicesCount = onlineCount,
                OfflineDevicesCount = offlineCount,
                DeviceTypes = deviceTypes,
                Locations = locations
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving devices");
            throw;
        }
    }

    public async Task<DeviceViewModel?> GetDeviceByIdAsync(int id)
    {
        try
        {
            var device = await _context.IoTDevices
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

            if (device == null)
            {
                return null;
            }

            return await ConvertToViewModelWithTelemetryAsync(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving device with ID {DeviceId}", id);
            throw;
        }
    }

    public async Task<DeviceViewModel?> GetDeviceByClientIdAsync(string clientId)
    {
        try
        {
            var device = await _context.IoTDevices
                .FirstOrDefaultAsync(d => d.ClientId == clientId && d.IsActive);

            if (device == null)
            {
                return null;
            }

            return await ConvertToViewModelWithTelemetryAsync(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving device with Client ID {ClientId}", clientId);
            throw;
        }
    }

    public async Task<DeviceViewModel> CreateDeviceAsync(DeviceViewModel deviceViewModel)
    {
        try
        {
            var device = new IoTDevice
            {
                Name = deviceViewModel.Name,
                DeviceType = deviceViewModel.DeviceType,
                Description = deviceViewModel.Description,
                Location = deviceViewModel.Location,
                IsOnline = deviceViewModel.IsOnline,
                SerialNumber = deviceViewModel.SerialNumber,
                FirmwareVersion = deviceViewModel.FirmwareVersion,
                ManufacturerName = deviceViewModel.ManufacturerName,
                ModelNumber = deviceViewModel.ModelNumber,
                ClientId = deviceViewModel.ClientId,
                MqttUsername = deviceViewModel.MqttUsername,
                MqttPasswordHash = !string.IsNullOrEmpty(deviceViewModel.MqttPassword) 
                    ? HashPassword(deviceViewModel.MqttPassword) 
                    : null,
                ApiKey = GenerateApiKey(),
                LastSeen = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.IoTDevices.Add(device);
            await _context.SaveChangesAsync();

            // Store initial telemetry values if provided
            var telemetryValues = new Dictionary<string, object>();

            if (deviceViewModel.BatteryLevel.HasValue)
                telemetryValues["battery_level"] = deviceViewModel.BatteryLevel.Value;

            if (deviceViewModel.Temperature.HasValue)
                telemetryValues["temperature"] = deviceViewModel.Temperature.Value;

            if (deviceViewModel.Humidity.HasValue)
                telemetryValues["humidity"] = deviceViewModel.Humidity.Value;

            if (telemetryValues.Any())
            {
                await _telemetryService.StoreBatchValuesAsync(device.Id, telemetryValues, "Good", "Initial device setup");
                await _logService.LogActionAsync(device.Id, "TelemetryInitialized", $"Initial sensor values stored: {string.Join(", ", telemetryValues.Keys)}", "Success");
            }

            deviceViewModel.Id = device.Id;
            deviceViewModel.CreatedAt = device.CreatedAt;
            deviceViewModel.UpdatedAt = device.UpdatedAt;
            deviceViewModel.LastSeen = device.LastSeen;

            await _logService.LogActionAsync(device.Id, "Created", "Device created and registered", "Success");

            _logger.LogInformation("Device created successfully with ID {DeviceId}", device.Id);

            return await ConvertToViewModelWithTelemetryAsync(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating device");
            throw;
        }
    }

    public async Task<DeviceViewModel?> UpdateDeviceAsync(int id, DeviceViewModel deviceViewModel)
    {
        try
        {
            var device = await _context.IoTDevices.FindAsync(id);
            if (device == null || !device.IsActive)
            {
                return null;
            }

            // Track changes for logging
            var changes = new List<string>();

            if (device.Name != deviceViewModel.Name) changes.Add($"Name: '{device.Name}' → '{deviceViewModel.Name}'");
            if (device.DeviceType != deviceViewModel.DeviceType) changes.Add($"Type: '{device.DeviceType}' → '{deviceViewModel.DeviceType}'");
            if (device.Description != deviceViewModel.Description) changes.Add($"Description: '{device.Description}' → '{deviceViewModel.Description}'");
            if (device.Location != deviceViewModel.Location) changes.Add($"Location: '{device.Location}' → '{deviceViewModel.Location}'");
            if (device.SerialNumber != deviceViewModel.SerialNumber) changes.Add($"Serial: '{device.SerialNumber}' → '{deviceViewModel.SerialNumber}'");
            if (device.FirmwareVersion != deviceViewModel.FirmwareVersion) changes.Add($"Firmware: '{device.FirmwareVersion}' → '{deviceViewModel.FirmwareVersion}'");
            if (device.ClientId != deviceViewModel.ClientId) changes.Add($"ClientId: '{device.ClientId}' → '{deviceViewModel.ClientId}'");
            if (device.MqttUsername != deviceViewModel.MqttUsername) changes.Add($"MQTT Username: '{device.MqttUsername}' → '{deviceViewModel.MqttUsername}'");

            device.Name = deviceViewModel.Name;
            device.DeviceType = deviceViewModel.DeviceType;
            device.Description = deviceViewModel.Description;
            device.Location = deviceViewModel.Location;
            device.SerialNumber = deviceViewModel.SerialNumber;
            device.FirmwareVersion = deviceViewModel.FirmwareVersion;
            device.ManufacturerName = deviceViewModel.ManufacturerName;
            device.ModelNumber = deviceViewModel.ModelNumber;
            device.ClientId = deviceViewModel.ClientId;
            device.MqttUsername = deviceViewModel.MqttUsername;
            
            // Only update password if a new one is provided
            if (!string.IsNullOrEmpty(deviceViewModel.MqttPassword))
            {
                device.MqttPasswordHash = HashPassword(deviceViewModel.MqttPassword);
                changes.Add("MQTT Password: Updated");
            }
            
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update telemetry values if provided
            var telemetryUpdates = new Dictionary<string, object>();

            if (deviceViewModel.BatteryLevel.HasValue)
                telemetryUpdates["battery_level"] = deviceViewModel.BatteryLevel.Value;

            if (deviceViewModel.Temperature.HasValue)
                telemetryUpdates["temperature"] = deviceViewModel.Temperature.Value;

            if (deviceViewModel.Humidity.HasValue)
                telemetryUpdates["humidity"] = deviceViewModel.Humidity.Value;

            if (telemetryUpdates.Any())
            {
                await _telemetryService.StoreBatchValuesAsync(device.Id, telemetryUpdates, "Good", "Manual device update");
                await _logService.LogActionAsync(device.Id, "TelemetryUpdated", $"Sensor values updated: {string.Join(", ", telemetryUpdates.Keys)}", "Success");
            }

            if (changes.Any())
            {
                await _logService.LogActionAsync(device.Id, "Updated", $"Device information updated: {string.Join("; ", changes)}", "Success");
            }

            _logger.LogInformation("Device updated successfully with ID {DeviceId}", device.Id);

            return await ConvertToViewModelWithTelemetryAsync(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating device with ID {DeviceId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDeviceAsync(int id)
    {
        try
        {
            var device = await _context.IoTDevices.FindAsync(id);
            if (device == null || !device.IsActive)
            {
                return false;
            }

            // Get telemetry count for logging
            var telemetryCount = await _context.Telemetries.CountAsync(t => t.DeviceId == id);
            var logCount = await _context.DeviceLogs.CountAsync(l => l.DeviceId == id);

            await _logService.LogActionAsync(device.Id, "Deleted", $"Device '{device.Name}' deleted (had {telemetryCount} telemetry records, {logCount} log entries)", "Success");

            // Soft delete - mark as inactive instead of hard delete
            device.IsActive = false;
            device.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Device soft-deleted successfully with ID {DeviceId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting device with ID {DeviceId}", id);
            throw;
        }
    }

    public async Task<bool> ToggleDeviceStatusAsync(int id)
    {
        try
        {
            var device = await _context.IoTDevices.FindAsync(id);
            if (device == null || !device.IsActive)
            {
                return false;
            }

            var oldStatus = device.IsOnline;
            device.IsOnline = !device.IsOnline;
            device.LastSeen = DateTime.UtcNow;
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var newStatus = device.IsOnline ? "online" : "offline";
            var oldStatusText = oldStatus ? "online" : "offline";

            // Store status change in telemetry
            await _telemetryService.StoreValueAsync(device.Id, "device_status", newStatus, "Good", $"Status toggled from {oldStatusText}");

            await _logService.LogActionAsync(device.Id, "ToggleStatus", $"Device status changed from {oldStatusText} to {newStatus}", "Success");

            _logger.LogInformation("Device status toggled successfully for ID {DeviceId}. {OldStatus} → {NewStatus}", id, oldStatusText, newStatus);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while toggling device status for ID {DeviceId}", id);
            throw;
        }
    }

    public async Task<bool> UpdateDeviceStatusAsync(int id, bool isOnline)
    {
        try
        {
            var device = await _context.IoTDevices.FindAsync(id);
            if (device == null || !device.IsActive)
            {
                return false;
            }

            var oldStatus = device.IsOnline;
            device.IsOnline = isOnline;
            device.LastSeen = DateTime.UtcNow;
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = isOnline ? "online" : "offline";

            // Store status in telemetry
            await _telemetryService.StoreValueAsync(device.Id, "device_status", status, "Good", "Status update via MQTT");

            if (oldStatus != isOnline)
            {
                await _logService.LogActionAsync(device.Id, "StatusUpdate", $"Device went {status}", "Success");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating device status for ID {DeviceId}", id);
            throw;
        }
    }

    public async Task<bool> UpdateDeviceSensorDataAsync(int id, double? temperature, double? humidity, double? batteryLevel)
    {
        try
        {
            var device = await _context.IoTDevices.FindAsync(id);
            if (device == null || !device.IsActive)
            {
                return false;
            }

            device.LastSeen = DateTime.UtcNow;
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Store sensor data in telemetry
            var telemetryValues = new Dictionary<string, object>();
            var updatedSensors = new List<string>();

            if (temperature.HasValue)
            {
                telemetryValues["temperature"] = temperature.Value;
                updatedSensors.Add($"temperature: {temperature.Value:F1}°C");
            }

            if (humidity.HasValue)
            {
                telemetryValues["humidity"] = humidity.Value;
                updatedSensors.Add($"humidity: {humidity.Value:F1}%");
            }

            if (batteryLevel.HasValue)
            {
                telemetryValues["battery_level"] = batteryLevel.Value;
                updatedSensors.Add($"battery: {batteryLevel.Value:F1}%");
            }

            if (telemetryValues.Any())
            {
                await _telemetryService.StoreBatchValuesAsync(device.Id, telemetryValues, "Good", "Sensor data update via MQTT");
                await _logService.LogActionAsync(device.Id, "SensorUpdate", $"Sensor data updated: {string.Join(", ", updatedSensors)}", "Success");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating sensor data for device ID {DeviceId}", id);
            throw;
        }
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        try
        {
            var totalDevices = await _context.IoTDevices.CountAsync(d => d.IsActive);
            var onlineDevices = await _context.IoTDevices.CountAsync(d => d.IsOnline && d.IsActive);
            var offlineDevices = totalDevices - onlineDevices;

            var recentDevices = await _context.IoTDevices
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentDeviceViewModels = new List<DeviceViewModel>();
            foreach (var device in recentDevices)
            {
                var viewModel = await ConvertToViewModelWithTelemetryAsync(device);
                recentDeviceViewModels.Add(viewModel);
            }

            var recentLogs = await _context.DeviceLogs
                .Include(l => l.Device)
                .Where(l => l.Device.IsActive)
                .OrderByDescending(l => l.Timestamp)
                .Take(10)
                .Select(l => new DeviceLogViewModel
                {
                    Id = l.Id,
                    DeviceId = l.DeviceId,
                    DeviceName = l.Device.Name,
                    Action = l.Action,
                    Description = l.Description,
                    Timestamp = l.Timestamp,
                    Status = l.Status
                })
                .ToListAsync();

            var deviceTypeStats = await _context.IoTDevices
                .Where(d => d.IsActive)
                .GroupBy(d => d.DeviceType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);

            var locationStats = await _context.IoTDevices
                .Where(d => d.IsActive)
                .GroupBy(d => d.Location)
                .Select(g => new { Location = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Location, x => x.Count);

            // Get devices with low battery using telemetry
            var lowBatteryDevices = new List<DeviceViewModel>();
            var allDevices = await _context.IoTDevices.Where(d => d.IsActive).ToListAsync();

            foreach (var device in allDevices)
            {
                var batteryLevel = await _telemetryService.GetBatteryLevelAsync(device.Id);
                if (batteryLevel.HasValue && batteryLevel.Value < 25)
                {
                    var viewModel = await ConvertToViewModelWithTelemetryAsync(device);
                    lowBatteryDevices.Add(viewModel);
                }
            }

            return new DashboardViewModel
            {
                TotalDevices = totalDevices,
                OnlineDevices = onlineDevices,
                OfflineDevices = offlineDevices,
                RecentDevices = recentDeviceViewModels,
                RecentLogs = recentLogs,
                DeviceTypeStats = deviceTypeStats,
                LocationStats = locationStats,
                LowBatteryDevices = lowBatteryDevices
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving dashboard data");
            throw;
        }
    }

    public async Task<List<string>> GetDeviceTypesAsync()
    {
        return await _context.IoTDevices
            .Where(d => d.IsActive)
            .Select(d => d.DeviceType)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }

    public async Task<List<string>> GetLocationsAsync()
    {
        return await _context.IoTDevices
            .Where(d => d.IsActive)
            .Select(d => d.Location)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();
    }

    public async Task<bool> DeviceExistsAsync(int id)
    {
        return await _context.IoTDevices.AnyAsync(d => d.Id == id && d.IsActive);
    }

    public async Task<bool> DeviceNameExistsAsync(string name, int? excludeId = null)
    {
        var query = _context.IoTDevices.Where(d => d.Name == name && d.IsActive);

        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    #region Private Helper Methods

    private async Task<DeviceViewModel> ConvertToViewModelWithTelemetryAsync(IoTDevice device)
    {
        // Get latest telemetry values
        var batteryLevel = await _telemetryService.GetBatteryLevelAsync(device.Id);
        var temperature = await _telemetryService.GetTemperatureAsync(device.Id);
        var humidity = await _telemetryService.GetHumidityAsync(device.Id);

        return new DeviceViewModel
        {
            Id = device.Id,
            Name = device.Name,
            DeviceType = device.DeviceType,
            Description = device.Description,
            Location = device.Location,
            IsOnline = device.IsOnline,
            LastSeen = device.LastSeen,
            CreatedAt = device.CreatedAt,
            UpdatedAt = device.UpdatedAt,
            SerialNumber = device.SerialNumber,
            FirmwareVersion = device.FirmwareVersion,
            ManufacturerName = device.ManufacturerName,
            ModelNumber = device.ModelNumber,
            ClientId = device.ClientId,
            MqttUsername = device.MqttUsername,
            ApiKey = device.ApiKey,
            LastMqttConnection = device.LastMqttConnection,
            ConnectionStatus = device.ConnectionStatus,
            BatteryLevel = batteryLevel,
            Temperature = temperature,
            Humidity = humidity
        };
    }

    /// <summary>
    /// Hash password using SHA256 (for simple hashing - consider using bcrypt/Argon2 for production)
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Generate a random API key for device authentication
    /// </summary>
    private string GenerateApiKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    #endregion
}