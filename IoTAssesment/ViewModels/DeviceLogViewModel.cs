using System.ComponentModel.DataAnnotations;

namespace IoTAssesment.ViewModels;

/// <summary>
/// ViewModel for Device Log operations
/// </summary>
public class DeviceLogViewModel
{
    public int Id { get; set; }

    [Display(Name = "Device")]
    public int DeviceId { get; set; }

    [Display(Name = "Device Name")]
    public string DeviceName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Action is required")]
    [StringLength(50, ErrorMessage = "Action cannot exceed 50 characters")]
    [Display(Name = "Action")]
    public string Action { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Timestamp")]
    public DateTime Timestamp { get; set; }

    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    [Display(Name = "Status")]
    public string? Status { get; set; }

    [StringLength(100, ErrorMessage = "User agent cannot exceed 100 characters")]
    [Display(Name = "User Agent")]
    public string? UserAgent { get; set; }

    // Additional properties for UI display
    [Display(Name = "Time Ago")]
    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTime.UtcNow - Timestamp;

            return timeSpan.TotalDays switch
            {
                >= 365 => $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) != 1 ? "s" : "")} ago",
                >= 30 => $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) != 1 ? "s" : "")} ago",
                >= 1 => $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago",
                _ => timeSpan.TotalHours switch
                {
                    >= 1 => $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago",
                    _ => timeSpan.TotalMinutes switch
                    {
                        >= 1 => $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago",
                        _ => "Just now"
                    }
                }
            };
        }
    }

    [Display(Name = "Status Badge Class")]
    public string StatusBadgeClass
    {
        get
        {
            return Status?.ToLower() switch
            {
                "success" => "bg-green-100 text-green-800",
                "warning" => "bg-yellow-100 text-yellow-800",
                "error" => "bg-red-100 text-red-800",
                "critical" => "bg-red-100 text-red-800",
                "info" => "bg-blue-100 text-blue-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }
    }

    [Display(Name = "Action Badge Class")]
    public string ActionBadgeClass
    {
        get
        {
            return Action.ToLower() switch
            {
                "created" => "bg-green-100 text-green-800",
                "updated" => "bg-blue-100 text-blue-800",
                "deleted" => "bg-red-100 text-red-800",
                "statusupdate" => "bg-purple-100 text-purple-800",
                "firmwareupdate" => "bg-indigo-100 text-indigo-800",
                "togglestatus" => "bg-yellow-100 text-yellow-800",
                _ => "bg-gray-100 text-gray-800"
            };
        }
    }
}

/// <summary>
/// ViewModel for device log listing with pagination and filtering
/// </summary>
public class DeviceLogListViewModel
{
    public List<DeviceLogViewModel> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    // Filtering properties
    public int? DeviceIdFilter { get; set; }
    public string? ActionFilter { get; set; }
    public string? StatusFilter { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // Sorting properties
    public string SortBy { get; set; } = "Timestamp";
    public string SortDirection { get; set; } = "desc";

    // Additional data for dropdowns
    public List<DeviceViewModel> Devices { get; set; } = new();
    public List<string> Actions { get; set; } = new();
    public List<string> Statuses { get; set; } = new();
}

/// <summary>
/// ViewModel for dashboard statistics
/// </summary>
public class DashboardViewModel
{
    public int TotalDevices { get; set; }
    public int OnlineDevices { get; set; }
    public int OfflineDevices { get; set; }
    public double OnlinePercentage => TotalDevices > 0 ? (double)OnlineDevices / TotalDevices * 100 : 0;
    public double OfflinePercentage => TotalDevices > 0 ? (double)OfflineDevices / TotalDevices * 100 : 0;

    public List<DeviceViewModel> RecentDevices { get; set; } = new();
    public List<DeviceLogViewModel> RecentLogs { get; set; } = new();

    // Device type statistics
    public Dictionary<string, int> DeviceTypeStats { get; set; } = new();

    // Location statistics
    public Dictionary<string, int> LocationStats { get; set; } = new();

    // Battery level warnings
    public List<DeviceViewModel> LowBatteryDevices { get; set; } = new();
    public int LowBatteryCount => LowBatteryDevices.Count;

    // Connection quality statistics
    public int ExcellentConnectionCount { get; set; }
    public int GoodConnectionCount { get; set; }
    public int FairConnectionCount { get; set; }
    public int PoorConnectionCount { get; set; }
}