using MessageProcessing;
using MessageProcessing.AnomalyDetection;
using Microsoft.Extensions.Options;
using SignalRServer.AlertHubHandling;

internal class AnomalyDetector : IAnomalyDetector
{
    private readonly AnomalyDetectionConfig _anomalyDetectionConfig;
    private readonly IHubConnection _hubConnection;

    public AnomalyDetector(
        IOptions<AnomalyDetectionConfig> anomalyDetectionConfig,
        IHubConnection hubConnection)
    {
        _anomalyDetectionConfig = anomalyDetectionConfig.Value;
        _hubConnection = hubConnection;
    }

    public async Task DetectAnomalies(ServerStatistics currentStats, ServerStatistics previousStats)
    {
        if (IsMemoryUsageAnomaly(currentStats.MemoryUsage, previousStats.MemoryUsage))
        {
            await SendAnomalyAlert("Memory Usage", currentStats);
        }

        if (IsCpuUsageAnomaly(currentStats.CpuUsage, previousStats.CpuUsage))
        {
            await SendAnomalyAlert("CPU Usage", currentStats);
        }

        if (IsHighMemoryUsage(currentStats))
        {
            await SendHighUsageAlert("Memory", currentStats);
        }

        if (IsHighCpuUsage(currentStats.CpuUsage))
        {
            await SendHighUsageAlert("CPU", currentStats);
        }
    }

    public bool IsMemoryUsageAnomaly(double currentMemoryUsage, double previousMemoryUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.MemoryUsageAnomalyThresholdPercentage;
        return currentMemoryUsage > (previousMemoryUsage * (1 + thresholdPercentage));
    }

    public bool IsCpuUsageAnomaly(double currentCpuUsage, double previousCpuUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.CpuUsageAnomalyThresholdPercentage;
        return currentCpuUsage > (previousCpuUsage * (1 + thresholdPercentage));
    }

    public bool IsHighMemoryUsage(ServerStatistics currentStats)
    {
        double thresholdPercentage = _anomalyDetectionConfig.MemoryUsageThresholdPercentage;
        return (currentStats.MemoryUsage / (currentStats.MemoryUsage + currentStats.AvailableMemory)) > thresholdPercentage;
    }

    public bool IsHighCpuUsage(double currentCpuUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.CpuUsageThresholdPercentage;
        return currentCpuUsage > thresholdPercentage;
    }

    private async Task SendAnomalyAlert(string alertType, ServerStatistics currentStats)
    {
        await _hubConnection.SendAsync("SendAnomalyAlertMessage", $"{alertType} Anomaly Detected! Server: {currentStats.ServerIdentifier}, Timestamp: {currentStats.Timestamp}");
    }

    private async Task SendHighUsageAlert(string usageType, ServerStatistics currentStats)
    {
        await _hubConnection.SendAsync("SendHighUsageAlertMessage", $"High {usageType} Usage Detected! Server: {currentStats.ServerIdentifier}, Timestamp: {currentStats.Timestamp}");
    }
}
