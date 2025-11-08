using IoTAssesment.ViewModels;

namespace IoTAssesment.Interfaces;

/// <summary>
/// Interface for Device Log Service operations
/// </summary>
public interface IDeviceLogService
{
    // Log operations
    Task LogActionAsync(int deviceId, string action, string? description = null, string? status = null, string? userAgent = null);

    // Log retrieval
    Task<DeviceLogListViewModel> GetLogsAsync(
        int page = 1,
        int pageSize = 20,
        int? deviceIdFilter = null,
        string? actionFilter = null,
        string? statusFilter = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string sortBy = "Timestamp",
        string sortDirection = "desc");

    Task<List<DeviceLogViewModel>> GetLogsByDeviceIdAsync(int deviceId, int count = 10);
    Task<List<DeviceLogViewModel>> GetRecentLogsAsync(int count = 10);

    // Statistics
    Task<List<string>> GetActionsAsync();
    Task<List<string>> GetStatusesAsync();
    Task<int> GetLogCountForDeviceAsync(int deviceId);
}