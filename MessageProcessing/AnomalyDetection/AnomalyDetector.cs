using MessageProcessing;
using MessageProcessing.AnomalyDetection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SignalRServer.AlertHubHandling;

internal class AnomalyDetector : IAnomalyDetector
{
    private readonly AnomalyDetectionConfig _anomalyDetectionConfig;
    private readonly IHubConnection _hubConnection;
    private readonly ILogger<AnomalyDetector> _logger;

    public AnomalyDetector(
        IOptions<AnomalyDetectionConfig> anomalyDetectionConfig,
        IHubConnection hubConnection,
        ILogger<AnomalyDetector> logger)
    {
        _anomalyDetectionConfig = anomalyDetectionConfig.Value;
        _hubConnection = hubConnection;
        _logger = logger;
    }

    public async Task DetectAnomalies(ServerStatistics currentStatistics, ServerStatistics previousStatistics)
    {
        if (IsMemoryUsageAnomaly(currentStatistics.MemoryUsage, previousStatistics.MemoryUsage))
        {
            await SendAnomalyAlert("Memory Usage", currentStatistics);
        }

        if (IsCpuUsageAnomaly(currentStatistics.CpuUsage, previousStatistics.CpuUsage))
        {
            await SendAnomalyAlert("CPU Usage", currentStatistics);
        }

        if (IsHighMemoryUsage(currentStatistics))
        {
            await SendHighUsageAlert("Memory", currentStatistics);
        }

        if (IsHighCpuUsage(currentStatistics.CpuUsage))
        {
            await SendHighUsageAlert("CPU", currentStatistics);
        }
    }

    public bool IsMemoryUsageAnomaly(double currentMemoryUsage, double previousMemoryUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.MemoryUsageAnomalyThresholdPercentage;
        var isAnomaly = currentMemoryUsage > (previousMemoryUsage * (1 + thresholdPercentage));
        if (isAnomaly)
        {
            _logger.LogInformation("Memory Usage Anomaly detected.");
        }
        return isAnomaly;
    }

    public bool IsCpuUsageAnomaly(double currentCpuUsage, double previousCpuUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.CpuUsageAnomalyThresholdPercentage;
        var isAnomaly = currentCpuUsage > (previousCpuUsage * (1 + thresholdPercentage));
        if (isAnomaly)
        {
            _logger.LogInformation("CPU Usage Anomaly detected.");
        }
        return isAnomaly;
    }

    public bool IsHighMemoryUsage(ServerStatistics currentStats)
    {
        double thresholdPercentage = _anomalyDetectionConfig.MemoryUsageThresholdPercentage;
        var isHighUsage = (currentStats.MemoryUsage / (currentStats.MemoryUsage + currentStats.AvailableMemory)) > thresholdPercentage;
        if (isHighUsage)
        {
            _logger.LogInformation("High Memory Usage detected.");
        }
        return isHighUsage;
    }

    public bool IsHighCpuUsage(double currentCpuUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.CpuUsageThresholdPercentage;
        var isHighUsage = currentCpuUsage > thresholdPercentage;

        if (isHighUsage)
        {
            _logger.LogInformation("High CPU Usage detected.");
        }
        return isHighUsage;
    }

    private async Task SendAnomalyAlert(string alertType, ServerStatistics currentStats)
    {
        await _hubConnection.SendAsync("SendAnomalyAlertMessage", $"{alertType} Anomaly Detected! Server: {currentStats.ServerIdentifier}, Timestamp: {currentStats.Timestamp}");
        _logger.LogInformation($"Anomaly Alert sent for {alertType}.");
    }

    private async Task SendHighUsageAlert(string usageType, ServerStatistics currentStats)
    {
        await _hubConnection.SendAsync("SendHighUsageAlertMessage", $"High {usageType} Usage Detected! Server: {currentStats.ServerIdentifier}, Timestamp: {currentStats.Timestamp}");
        _logger.LogInformation($"High Usage Alert sent for {usageType}.");
    }
}
