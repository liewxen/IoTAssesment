using Microsoft.AspNetCore.Mvc;
using IoTAssesment.Services;
using IoTAssesment.Interfaces;

namespace IoTAssesment.Controllers;

/// <summary>
/// Controller for device logs functionality
/// </summary>
public class LogsController : Controller
{
    private readonly IDeviceLogService _logService;
    private readonly IDeviceService _deviceService;
    private readonly ILogger<LogsController> _logger;

    public LogsController(
        IDeviceLogService logService,
        IDeviceService deviceService,
        ILogger<LogsController> logger)
    {
        _logService = logService;
        _deviceService = deviceService;
        _logger = logger;
    }

    /// <summary>
    /// Display logs index page
    /// </summary>
    public async Task<IActionResult> Index(
        int page = 1,
        int? deviceId = null,
        string? filteredAction = null,
        string? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string sortBy = "Timestamp",
        string sortDirection = "desc")
    {
        try
        {
            var model = await _logService.GetLogsAsync(
                page, 20, deviceId, filteredAction, status, fromDate, toDate, sortBy, sortDirection);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading logs index page");
            TempData["Error"] = "Error loading logs. Please try again.";
            return View(new IoTAssesment.ViewModels.DeviceLogListViewModel());
        }
    }

    /// <summary>
    /// API endpoint to get logs with pagination and filtering
    /// </summary>
    [HttpGet("api/logs")]
    public async Task<IActionResult> GetLogsApi(
        int page = 1,
        int pageSize = 20,
        int? deviceId = null,
        string? action = null,
        string? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string sortBy = "Timestamp",
        string sortDirection = "desc")
    {
        try
        {
            var logs = await _logService.GetLogsAsync(
                page, pageSize, deviceId, action, status, fromDate, toDate, sortBy, sortDirection);

            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetLogsApi");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// API endpoint to get logs for a specific device
    /// </summary>
    [HttpGet("api/devices/{deviceId}/logs")]
    public async Task<IActionResult> GetDeviceLogsApi(int deviceId, int count = 10)
    {
        try
        {
            if (!await _deviceService.DeviceExistsAsync(deviceId))
            {
                return NotFound(new { message = "Device not found" });
            }

            var logs = await _logService.GetLogsByDeviceIdAsync(deviceId, count);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetDeviceLogsApi for device {DeviceId}", deviceId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// API endpoint to get recent logs across all devices
    /// </summary>
    [HttpGet("api/logs/recent")]
    public async Task<IActionResult> GetRecentLogsApi(int count = 10)
    {
        try
        {
            var logs = await _logService.GetRecentLogsAsync(count);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetRecentLogsApi");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}