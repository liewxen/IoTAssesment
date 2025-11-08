using Microsoft.EntityFrameworkCore;
using IoTAssesment.Models;
using IoTAssesment.ViewModels;
using IoTAssesment.Interfaces;
namespace IoTAssesment.Services;

/// <summary>
/// Service class for Device Log operations with business logic
/// </summary>
public class DeviceLogService : IDeviceLogService
{
    private readonly IoTDeviceContext _context;
    private readonly ILogger<DeviceLogService> _logger;

    public DeviceLogService(IoTDeviceContext context, ILogger<DeviceLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogActionAsync(int deviceId, string action, string? description = null, string? status = null, string? userAgent = null)
    {
        try
        {
            var log = new DeviceLog
            {
                DeviceId = deviceId,
                Action = action,
                Description = description,
                Status = status ?? "Success",
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };

            _context.DeviceLogs.Add(log);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Action '{Action}' logged for device ID {DeviceId}", action, deviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging action '{Action}' for device ID {DeviceId}", action, deviceId);
            // Don't throw here as logging should not break the main operation
        }
    }

    public async Task<DeviceLogListViewModel> GetLogsAsync(
        int page = 1,
        int pageSize = 20,
        int? deviceIdFilter = null,
        string? actionFilter = null,
        string? statusFilter = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string sortBy = "Timestamp",
        string sortDirection = "desc")
    {
        try
        {
            var query = _context.DeviceLogs.Include(l => l.Device).AsQueryable();

            // Apply filters
            if (deviceIdFilter.HasValue)
            {
                query = query.Where(l => l.DeviceId == deviceIdFilter.Value);
            }

            if (!string.IsNullOrEmpty(actionFilter))
            {
                query = query.Where(l => l.Action == actionFilter);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(l => l.Status == statusFilter);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var toDateEnd = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.Timestamp <= toDateEnd);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "timestamp" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(l => l.Timestamp) : query.OrderBy(l => l.Timestamp),
                "action" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(l => l.Action) : query.OrderBy(l => l.Action),
                "status" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(l => l.Status) : query.OrderBy(l => l.Status),
                "devicename" => sortDirection.ToLower() == "desc" ? query.OrderByDescending(l => l.Device.Name) : query.OrderBy(l => l.Device.Name),
                _ => query.OrderByDescending(l => l.Timestamp)
            };

            var totalCount = await query.CountAsync();

            var logs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new DeviceLogViewModel
                {
                    Id = l.Id,
                    DeviceId = l.DeviceId,
                    DeviceName = l.Device.Name,
                    Action = l.Action,
                    Description = l.Description,
                    Timestamp = l.Timestamp,
                    Status = l.Status,
                    UserAgent = l.UserAgent
                })
                .ToListAsync();

            var devices = await _context.IoTDevices
                .OrderBy(d => d.Name)
                .Select(d => new DeviceViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToListAsync();

            var actions = await _context.DeviceLogs
                .Select(l => l.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            var statuses = await _context.DeviceLogs
                .Where(l => l.Status != null)
                .Select(l => l.Status!)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return new DeviceLogListViewModel
            {
                Logs = logs,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                DeviceIdFilter = deviceIdFilter,
                ActionFilter = actionFilter,
                StatusFilter = statusFilter,
                FromDate = fromDate,
                ToDate = toDate,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Devices = devices,
                Actions = actions,
                Statuses = statuses
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving device logs");
            throw;
        }
    }

    public async Task<List<DeviceLogViewModel>> GetLogsByDeviceIdAsync(int deviceId, int count = 10)
    {
        try
        {
            return await _context.DeviceLogs
                .Include(l => l.Device)
                .Where(l => l.DeviceId == deviceId)
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .Select(l => new DeviceLogViewModel
                {
                    Id = l.Id,
                    DeviceId = l.DeviceId,
                    DeviceName = l.Device.Name,
                    Action = l.Action,
                    Description = l.Description,
                    Timestamp = l.Timestamp,
                    Status = l.Status,
                    UserAgent = l.UserAgent
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving logs for device ID {DeviceId}", deviceId);
            throw;
        }
    }

    public async Task<List<DeviceLogViewModel>> GetRecentLogsAsync(int count = 10)
    {
        try
        {
            return await _context.DeviceLogs
                .Include(l => l.Device)
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .Select(l => new DeviceLogViewModel
                {
                    Id = l.Id,
                    DeviceId = l.DeviceId,
                    DeviceName = l.Device.Name,
                    Action = l.Action,
                    Description = l.Description,
                    Timestamp = l.Timestamp,
                    Status = l.Status,
                    UserAgent = l.UserAgent
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving recent logs");
            throw;
        }
    }

    public async Task<List<string>> GetActionsAsync()
    {
        return await _context.DeviceLogs
            .Select(l => l.Action)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<List<string>> GetStatusesAsync()
    {
        return await _context.DeviceLogs
            .Where(l => l.Status != null)
            .Select(l => l.Status!)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();
    }

    public async Task<int> GetLogCountForDeviceAsync(int deviceId)
    {
        return await _context.DeviceLogs
            .Where(l => l.DeviceId == deviceId)
            .CountAsync();
    }
}