using Microsoft.EntityFrameworkCore;

namespace IoTAssesment.Models;

/// <summary>
/// Entity Framework Database Context for IoT Device Management with Generic Telemetry Storage
/// </summary>
public class IoTDeviceContext : DbContext
{
    private readonly IConfiguration _configuration;

    public IoTDeviceContext(DbContextOptions<IoTDeviceContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<IoTDevice> IoTDevices { get; set; }
    public DbSet<DeviceLog> DeviceLogs { get; set; }
    public DbSet<KeyDictionary> KeyDictionaries { get; set; }
    public DbSet<Telemetry> Telemetries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureIoTDevice(modelBuilder);
        ConfigureDeviceLog(modelBuilder);
        ConfigureKeyDictionary(modelBuilder);
        ConfigureTelemetry(modelBuilder);
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Configure IoTDevice entity
    /// </summary>
    private static void ConfigureIoTDevice(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IoTDevice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DeviceType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.FirmwareVersion).HasMaxLength(50);
            entity.Property(e => e.ManufacturerName).HasMaxLength(100);
            entity.Property(e => e.ModelNumber).HasMaxLength(100);
            entity.Property(e => e.ConfigurationJson).HasMaxLength(1000);

            // Configure timestamp columns for PostgreSQL
            entity.Property(e => e.LastSeen).HasColumnType("timestamp with time zone");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");

            // Indexes for better query performance
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.DeviceType);
            entity.HasIndex(e => e.IsOnline);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Location);
        });
    }

    /// <summary>
    /// Configure DeviceLog entity
    /// </summary>
    private static void ConfigureDeviceLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(100);

            // Configure timestamp for PostgreSQL
            entity.Property(e => e.Timestamp).HasColumnType("timestamp with time zone");

            // Configure foreign key relationship
            entity.HasOne(d => d.Device)
                  .WithMany(p => p.DeviceLogs)
                  .HasForeignKey(d => d.DeviceId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Indexes for better query performance
            entity.HasIndex(e => e.DeviceId);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.Timestamp);
        });
    }

    /// <summary>
    /// Configure KeyDictionary entity
    /// </summary>
    private static void ConfigureKeyDictionary(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeyDictionary>(entity =>
        {
            entity.HasKey(e => e.KeyId);
            entity.Property(e => e.KeyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DataType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.DefaultValue).HasMaxLength(1000);
            entity.Property(e => e.ValidationRules).HasMaxLength(1000);

            // Configure decimal precision for min/max values
            entity.Property(e => e.MinValue).HasColumnType("decimal(18,4)");
            entity.Property(e => e.MaxValue).HasColumnType("decimal(18,4)");

            // Configure timestamp columns for PostgreSQL
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");

            // Indexes for better query performance
            entity.HasIndex(e => e.KeyName).IsUnique();
            entity.HasIndex(e => e.DataType);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
        });
    }

    /// <summary>
    /// Configure Telemetry entity with partitioning support
    /// </summary>
    private void ConfigureTelemetry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DblValue).HasColumnType("decimal(18,4)");
            entity.Property(e => e.StrValue).HasMaxLength(1000);
            entity.Property(e => e.JsonValue).HasColumnType("jsonb"); // PostgreSQL JSONB for better performance
            entity.Property(e => e.Quality).HasMaxLength(50);
            entity.Property(e => e.Context).HasMaxLength(500);

            // Configure timestamp columns for PostgreSQL
            entity.Property(e => e.Timestamp).HasColumnType("timestamp with time zone");
            entity.Property(e => e.PartitionDate).HasColumnType("date");

            // Configure foreign key relationships
            entity.HasOne(t => t.Device)
                  .WithMany(d => d.Telemetries)
                  .HasForeignKey(t => t.DeviceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Key)
                  .WithMany(k => k.Telemetries)
                  .HasForeignKey(t => t.KeyId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Composite indexes for better query performance
            entity.HasIndex(e => new { e.DeviceId, e.KeyId, e.Timestamp });
            entity.HasIndex(e => new { e.PartitionDate, e.DeviceId });
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Quality);

            // Configure partitioning based on settings
            var partitioningConfig = _configuration.GetSection("Telemetry:Partitioning");
            var enablePartitioning = partitioningConfig.GetValue<bool>("Enabled", true);
            var partitioningType = partitioningConfig.GetValue<string>("Type", "Monthly");

            if (enablePartitioning)
            {
                // Note: Actual partition creation will be handled in migrations
                // This is configuration for the entity mapping
                entity.ToTable("Telemetries");
            }
        });
    }

    /// <summary>
    /// Seeds initial data for development and testing
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed KeyDictionary with common IoT device attributes
        var keyDictionaries = new List<KeyDictionary>
        {
            new KeyDictionary { KeyId = 1, KeyName = "battery_level", Description = "Device battery level percentage", DataType = "double", Unit = "%", Category = "sensor", MinValue = 0, MaxValue = 100, IsRequired = false },
            new KeyDictionary { KeyId = 2, KeyName = "temperature", Description = "Temperature reading", DataType = "double", Unit = "°C", Category = "sensor", MinValue = -50, MaxValue = 100, IsRequired = false },
            new KeyDictionary { KeyId = 3, KeyName = "humidity", Description = "Humidity percentage", DataType = "double", Unit = "%", Category = "sensor", MinValue = 0, MaxValue = 100, IsRequired = false },
            new KeyDictionary { KeyId = 4, KeyName = "signal_strength", Description = "Signal strength indicator", DataType = "long", Unit = "dBm", Category = "status", MinValue = -120, MaxValue = 0, IsRequired = false },
            new KeyDictionary { KeyId = 5, KeyName = "firmware_version", Description = "Current firmware version", DataType = "string", Category = "metadata", IsRequired = false },
            new KeyDictionary { KeyId = 6, KeyName = "device_status", Description = "Current device operational status", DataType = "string", Category = "status", IsRequired = false },
            new KeyDictionary { KeyId = 7, KeyName = "uptime", Description = "Device uptime in seconds", DataType = "long", Unit = "seconds", Category = "status", MinValue = 0, IsRequired = false },
            new KeyDictionary { KeyId = 8, KeyName = "memory_usage", Description = "Memory usage percentage", DataType = "double", Unit = "%", Category = "performance", MinValue = 0, MaxValue = 100, IsRequired = false },
            new KeyDictionary { KeyId = 9, KeyName = "cpu_usage", Description = "CPU usage percentage", DataType = "double", Unit = "%", Category = "performance", MinValue = 0, MaxValue = 100, IsRequired = false },
            new KeyDictionary { KeyId = 10, KeyName = "error_count", Description = "Number of errors since last reset", DataType = "long", Category = "status", MinValue = 0, IsRequired = false },
            new KeyDictionary { KeyId = 11, KeyName = "location_coordinates", Description = "GPS coordinates", DataType = "json", Category = "location", IsRequired = false },
            new KeyDictionary { KeyId = 12, KeyName = "configuration", Description = "Device configuration settings", DataType = "json", Category = "config", IsRequired = false }
        };

        modelBuilder.Entity<KeyDictionary>().HasData(keyDictionaries);

        // Seed IoT Devices
        var devices = new List<IoTDevice>
        {
            new IoTDevice
            {
                Id = 1,
                Name = "Temperature Sensor - Warehouse A",
                DeviceType = "Temperature Sensor",
                Description = "Monitors temperature in warehouse storage area",
                Location = "Warehouse A - Zone 1",
                IsOnline = true,
                SerialNumber = "TEMP-001-WHA",
                FirmwareVersion = "1.2.3",
                ManufacturerName = "TechSensors Inc",
                ModelNumber = "TS-100",
                LastSeen = DateTime.UtcNow.AddMinutes(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-5),
                IsActive = true
            },
            new IoTDevice
            {
                Id = 2,
                Name = "Smart Lock - Main Entrance",
                DeviceType = "Access Control",
                Description = "Electronic lock for main building entrance",
                Location = "Building Main Entrance",
                IsOnline = true,
                SerialNumber = "LOCK-002-ME",
                FirmwareVersion = "2.1.0",
                ManufacturerName = "SecureTech",
                ModelNumber = "SL-200",
                LastSeen = DateTime.UtcNow.AddMinutes(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-2),
                IsActive = true
            },
            new IoTDevice
            {
                Id = 3,
                Name = "Humidity Monitor - Server Room",
                DeviceType = "Environmental Sensor",
                Description = "Monitors humidity levels in the server room",
                Location = "Server Room - Floor 2",
                IsOnline = false,
                SerialNumber = "HUM-003-SR",
                FirmwareVersion = "1.0.8",
                ManufacturerName = "EnviroSense",
                ModelNumber = "ES-300",
                LastSeen = DateTime.UtcNow.AddHours(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow.AddHours(-2),
                IsActive = true
            }
        };

        modelBuilder.Entity<IoTDevice>().HasData(devices);

        // Seed Device Logs
        var logs = new List<DeviceLog>
        {
            new DeviceLog { Id = 1, DeviceId = 1, Action = "Created", Description = "Device created and registered", Timestamp = DateTime.UtcNow.AddDays(-30), Status = "Success" },
            new DeviceLog { Id = 2, DeviceId = 1, Action = "StatusUpdate", Description = "Device came online", Timestamp = DateTime.UtcNow.AddMinutes(-5), Status = "Success" },
            new DeviceLog { Id = 3, DeviceId = 2, Action = "Created", Description = "Device created and registered", Timestamp = DateTime.UtcNow.AddDays(-45), Status = "Success" },
            new DeviceLog { Id = 4, DeviceId = 2, Action = "FirmwareUpdate", Description = "Firmware updated to version 2.1.0", Timestamp = DateTime.UtcNow.AddDays(-10), Status = "Success" },
            new DeviceLog { Id = 5, DeviceId = 3, Action = "Created", Description = "Device created and registered", Timestamp = DateTime.UtcNow.AddDays(-60), Status = "Success" },
            new DeviceLog { Id = 6, DeviceId = 3, Action = "StatusUpdate", Description = "Device went offline - low battery", Timestamp = DateTime.UtcNow.AddHours(-2), Status = "Warning" }
        };

        modelBuilder.Entity<DeviceLog>().HasData(logs);

        // Seed some sample telemetry data
        var baseTime = DateTime.UtcNow.AddHours(-1);
        var telemetries = new List<Telemetry>();

        // Device 1 - Temperature readings
        for (int i = 0; i < 10; i++)
        {
            telemetries.Add(new Telemetry
            {
                Id = (i * 3) + 1,
                DeviceId = 1,
                KeyId = 2, // temperature
                DblValue = 22.5 + (i * 0.5),
                Timestamp = baseTime.AddMinutes(i * 5),
                PartitionDate = baseTime.Date,
                Quality = "Good"
            });

            telemetries.Add(new Telemetry
            {
                Id = (i * 3) + 2,
                DeviceId = 1,
                KeyId = 1, // battery_level
                DblValue = 85.5 - (i * 0.1),
                Timestamp = baseTime.AddMinutes(i * 5),
                PartitionDate = baseTime.Date,
                Quality = "Good"
            });
        }

        // Device 2 - Status readings
        for (int i = 0; i < 5; i++)
        {
            telemetries.Add(new Telemetry
            {
                Id = 50 + i,
                DeviceId = 2,
                KeyId = 6, // device_status
                StrValue = "Operational",
                Timestamp = baseTime.AddMinutes(i * 10),
                PartitionDate = baseTime.Date,
                Quality = "Good"
            });
        }

        modelBuilder.Entity<Telemetry>().HasData(telemetries);
    }
}