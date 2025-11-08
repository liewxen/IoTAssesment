using IoTAssesment.Models;
using IoTAssesment.ViewModels;

namespace IoTAssesment.Interfaces;

/// <summary>
/// Interface for IoT Device Service operations
/// </summary>
public interface IDeviceService
{
    // Device CRUD operations
    Task<DeviceListViewModel> GetDevicesAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? deviceTypeFilter = null,
        bool? onlineStatusFilter = null,
        string? locationFilter = null,
        string sortBy = "Name",
        string sortDirection = "asc");

    Task<DeviceViewModel?> GetDeviceByIdAsync(int id);
    Task<DeviceViewModel?> GetDeviceByClientIdAsync(string clientId);
    Task<DeviceViewModel> CreateDeviceAsync(DeviceViewModel deviceViewModel);
    Task<DeviceViewModel?> UpdateDeviceAsync(int id, DeviceViewModel deviceViewModel);
    Task<bool> DeleteDeviceAsync(int id);

    // Device status operations
    Task<bool> ToggleDeviceStatusAsync(int id);
    Task<bool> UpdateDeviceStatusAsync(int id, bool isOnline);
    Task<bool> UpdateDeviceSensorDataAsync(int id, double? temperature, double? humidity, double? batteryLevel);

    // Statistics and dashboard
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task<List<string>> GetDeviceTypesAsync();
    Task<List<string>> GetLocationsAsync();

    // Utility methods
    Task<bool> DeviceExistsAsync(int id);
    Task<bool> DeviceNameExistsAsync(string name, int? excludeId = null);
}