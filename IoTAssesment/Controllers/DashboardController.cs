using Microsoft.AspNetCore.Mvc;
using IoTAssesment.Services;
using IoTAssesment.Interfaces;

namespace IoTAssesment.Controllers;

/// <summary>
/// Controller for dashboard and statistics functionality
/// </summary>
public class DashboardController : Controller
{
    private readonly IDeviceService _deviceService;
    private readonly IDeviceLogService _logService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDeviceService deviceService,
        IDeviceLogService logService,
        ILogger<DashboardController> logger)
    {
        _deviceService = deviceService;
        _logService = logService;
        _logger = logger;
    }

    /// <summary>
    /// Display the main dashboard
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var dashboardData = await _deviceService.GetDashboardDataAsync();
            return View(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            TempData["Error"] = "Error loading dashboard data. Please try again.";
            return View();
        }
    }

    /// <summary>
    /// API endpoint for dashboard data
    /// </summary>
    [HttpGet("api/dashboard")]
    public async Task<IActionResult> GetDashboardData()
    {
        try
        {
            var data = await _deviceService.GetDashboardDataAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetDashboardData API");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// API endpoint for device statistics
    /// </summary>
    [HttpGet("api/dashboard/statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            var dashboardData = await _deviceService.GetDashboardDataAsync();

            var statistics = new
            {
                TotalDevices = dashboardData.TotalDevices,
                OnlineDevices = dashboardData.OnlineDevices,
                OfflineDevices = dashboardData.OfflineDevices,
                OnlinePercentage = Math.Round(dashboardData.OnlinePercentage, 1),
                OfflinePercentage = Math.Round(dashboardData.OfflinePercentage, 1),
                LowBatteryCount = dashboardData.LowBatteryCount,
                DeviceTypeStats = dashboardData.DeviceTypeStats,
                LocationStats = dashboardData.LocationStats
            };

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetStatistics API");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}