using Microsoft.AspNetCore.Mvc;
using IoTAssesment.Services;
using IoTAssesment.ViewModels;
using IoTAssesment.Interfaces;

namespace IoTAssesment.Controllers;

/// <summary>
/// MVC Controller for IoT Device management with both API and View endpoints
/// </summary>
public class DevicesController : Controller
{
    private readonly IDeviceService _deviceService;
    private readonly IDeviceLogService _logService;
    private readonly IMqttService _mqttService;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(
        IDeviceService deviceService,
        IDeviceLogService logService,
        IMqttService mqttService,
        ILogger<DevicesController> logger)
    {
        _deviceService = deviceService;
        _logService = logService;
        _mqttService = mqttService;
        _logger = logger;
    }

    #region MVC Views

    /// <summary>
    /// Display the devices index page
    /// </summary>
    public async Task<IActionResult> Index(
        int page = 1,
        string? search = null,
        string? deviceType = null,
        bool? onlineStatus = null,
        string? location = null,
        string sortBy = "Name",
        string sortDirection = "asc")
    {
        try
        {
            var model = await _deviceService.GetDevicesAsync(
                page, 10, search, deviceType, onlineStatus, location, sortBy, sortDirection);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading devices index page");
            TempData["Error"] = "Error loading devices. Please try again.";
            return View(new DeviceListViewModel());
        }
    }

    /// <summary>
    /// Display device details page
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                TempData["Error"] = "Device not found.";
                return RedirectToAction(nameof(Index));
            }

            var logs = await _logService.GetLogsByDeviceIdAsync(id, 20);
            ViewBag.DeviceLogs = logs;

            return View(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading device details for ID {DeviceId}", id);
            TempData["Error"] = "Error loading device details.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Display create device form
    /// </summary>
    public IActionResult Create()
    {
        var model = new DeviceFormViewModel();
        return View(model);
    }

    /// <summary>
    /// Handle create device form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeviceFormViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Check if device name already exists
                if (await _deviceService.DeviceNameExistsAsync(model.Device.Name))
                {
                    ModelState.AddModelError("Device.Name", "A device with this name already exists.");
                    return View(model);
                }

                await _deviceService.CreateDeviceAsync(model.Device);
                TempData["Success"] = "Device created successfully.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating device");
            TempData["Error"] = "Error creating device. Please try again.";
        }

        return View(model);
    }

    /// <summary>
    /// Display edit device form
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                TempData["Error"] = "Device not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new DeviceFormViewModel { Device = device };
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit form for device ID {DeviceId}", id);
            TempData["Error"] = "Error loading device for editing.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handle edit device form submission
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DeviceFormViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Check if device name already exists (excluding current device)
                if (await _deviceService.DeviceNameExistsAsync(model.Device.Name, id))
                {
                    ModelState.AddModelError("Device.Name", "A device with this name already exists.");
                    return View(model);
                }

                var updatedDevice = await _deviceService.UpdateDeviceAsync(id, model.Device);
                if (updatedDevice == null)
                {
                    TempData["Error"] = "Device not found.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Device updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device with ID {DeviceId}", id);
            TempData["Error"] = "Error updating device. Please try again.";
        }

        return View(model);
    }

    /// <summary>
    /// Display delete confirmation page
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                TempData["Error"] = "Device not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(device);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for device ID {DeviceId}", id);
            TempData["Error"] = "Error loading device.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handle delete device confirmation
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var deleted = await _deviceService.DeleteDeviceAsync(id);
            if (deleted)
            {
                TempData["Success"] = "Device deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Device not found.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device with ID {DeviceId}", id);
            TempData["Error"] = "Error deleting device. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region API Endpoints

    /// <summary>
    /// API endpoint to toggle device online/offline status
    /// </summary>
    [HttpPost("api/devices/{id}/toggle-status")]
    public async Task<IActionResult> ToggleDeviceStatus(int id)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                return NotFound(new { success = false, message = "Device not found" });
            }

            var success = await _deviceService.ToggleDeviceStatusAsync(id);
            if (success)
            {
                var updatedDevice = await _deviceService.GetDeviceByIdAsync(id);
                await _logService.LogActionAsync(id, "ToggleStatus", 
                    $"Device status toggled to {(updatedDevice?.IsOnline == true ? "Online" : "Offline")}");
                
                return Ok(new 
                { 
                    success = true, 
                    message = $"Device status changed to {(updatedDevice?.IsOnline == true ? "Online" : "Offline")}",
                    device = updatedDevice 
                });
            }

            return BadRequest(new { success = false, message = "Failed to toggle device status" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling status for device {DeviceId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// API endpoint to fetch latest sensor data from telemetry database
    /// </summary>
    [HttpPost("api/devices/{id}/request-sensor-data")]
    public async Task<IActionResult> RequestSensorData(int id)
    {
        try
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            if (device == null)
            {
                return NotFound(new { success = false, message = "Device not found" });
            }

            // Fetch latest telemetry data from database
            var hasData = device.Temperature.HasValue || device.Humidity.HasValue || device.BatteryLevel.HasValue;
            
            if (hasData)
            {
                return Ok(new 
                { 
                    success = true, 
                    message = "Latest sensor data retrieved successfully",
                    data = new
                    {
                        temperature = device.Temperature,
                        humidity = device.Humidity,
                        batteryLevel = device.BatteryLevel,
                        lastUpdated = device.UpdatedAt
                    }
                });
            }
            else
            {
                return Ok(new 
                { 
                    success = true, 
                    message = "No sensor data available for this device yet",
                    data = new
                    {
                        temperature = (double?)null,
                        humidity = (double?)null,
                        batteryLevel = (double?)null,
                        lastUpdated = device.UpdatedAt
                    }
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sensor data for device {DeviceId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    #endregion
}