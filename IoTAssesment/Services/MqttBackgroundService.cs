using IoTAssesment.Interfaces;

namespace IoTAssesment.Services;

/// <summary>
/// Background service to automatically start and maintain MQTT connection
/// </summary>
public class MqttBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MqttBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    public MqttBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MqttBackgroundService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MQTT Background Service is starting");

        // Wait a bit for the application to fully start
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();

                    // Check if MQTT is enabled in configuration
                    var brokerHost = _configuration.GetValue<string>("MQTT:BrokerHost");
                    if (string.IsNullOrEmpty(brokerHost) || brokerHost == "localhost")
                    {
                        _logger.LogWarning("MQTT broker not configured or using localhost. Waiting 60 seconds before retry...");
                        await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                        continue;
                    }

                    // Try to connect to MQTT
                    var isConnected = await mqttService.IsConnectedAsync();
                    if (!isConnected)
                    {
                        _logger.LogInformation("MQTT is not connected. Attempting to start...");
                        var started = await mqttService.StartAsync();

                        if (started)
                        {
                            _logger.LogInformation("MQTT service started successfully");
                        }
                        else
                        {
                            _logger.LogWarning("Failed to start MQTT service. Will retry in 30 seconds");
                        }
                    }
                }

                // Check connection every 30 seconds
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MQTT Background Service");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        _logger.LogInformation("MQTT Background Service is stopping");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MQTT Background Service is stopping");

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();
                await mqttService.StopAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping MQTT service");
        }

        await base.StopAsync(stoppingToken);
    }
}
