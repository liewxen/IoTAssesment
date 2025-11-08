using Microsoft.EntityFrameworkCore;
using IoTAssesment.Models;
using IoTAssesment.Services;
using IoTAssesment.Interfaces;

namespace IoTAssesment.Extensions;

/// <summary>
/// Extension methods for configuring services in dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds IoT Device Management services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddIoTDeviceManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Npgsql to handle DateTime properly with PostgreSQL
        // This enables legacy timestamp behavior to avoid DateTime.Kind issues
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Add Entity Framework and PostgreSQL
        services.AddDatabaseServices(configuration);

        // Add business logic services
        services.AddBusinessServices();

        // Add MQTT services
        services.AddMqttServices();

        // Add hosted services for background tasks
        services.AddHostedServices();

        return services;
    }

    /// <summary>
    /// Adds database services including Entity Framework and PostgreSQL
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Configure Entity Framework with PostgreSQL
        services.AddDbContext<IoTDeviceContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });

            // Enable sensitive data logging in development
            options.EnableSensitiveDataLogging(false);

            // Enable detailed errors in development
            options.EnableDetailedErrors(true);
        });

        // Add health checks for database
        services.AddHealthChecks();

        return services;
    }

    /// <summary>
    /// Adds business logic services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Register business services
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IDeviceLogService, DeviceLogService>();
        services.AddScoped<ITelemetryService, TelemetryService>();

        return services;
    }

    /// <summary>
    /// Adds MQTT services for device communication
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddMqttServices(this IServiceCollection services)
    {
        // Register MQTT service as scoped instead of singleton to avoid DI issues
        services.AddScoped<IMqttService, MqttService>();

        return services;
    }

    /// <summary>
    /// Adds hosted services for background tasks
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        // Add MQTT service as hosted service to start automatically
        services.AddHostedService<MqttBackgroundService>();

        return services;
    }

    /// <summary>
    /// Adds CORS policies for frontend integration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowVueApp", policy =>
            {
                policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:8080")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });

            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    /// <summary>
    /// Adds logging configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.AddDebug();
        });

        return services;
    }
}