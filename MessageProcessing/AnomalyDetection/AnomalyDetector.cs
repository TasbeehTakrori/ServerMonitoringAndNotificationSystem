using MessageProcessing;
using MessageProcessing.AnomalyDetection;
using MessageProcessing.AnomalyDetection.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class AnomalyDetector : IAnomalyDetector
{
    private readonly AnomalyDetectionConfig _anomalyDetectionConfig;

    public AnomalyDetector(
        IOptions<AnomalyDetectionConfig> anomalyDetectionConfig)
    {
        _anomalyDetectionConfig = anomalyDetectionConfig.Value;
    }

    public List<AnomalyType> DetectAnomalies(ServerStatistics currentStatistics, ServerStatistics previousStatistics)
    {
        List<AnomalyType> anomalies = new();

        if (IsMemoryUsageAnomaly(currentStatistics.MemoryUsage, previousStatistics.MemoryUsage))
        {
            anomalies.Add(AnomalyType.MemoryUsageAnomaly);
        }

        if (IsCpuUsageAnomaly(currentStatistics.CpuUsage, previousStatistics.CpuUsage))
        {
            anomalies.Add(AnomalyType.CPUUsageAnomaly);
        }

        if (IsHighMemoryUsage(currentStatistics))
        {
            anomalies.Add(AnomalyType.HighMemoryUsage);
        }

        if (IsHighCpuUsage(currentStatistics.CpuUsage))
        {
            anomalies.Add(AnomalyType.HighCPUUsage);
        }
        return anomalies;
    }

    public bool IsMemoryUsageAnomaly(double currentMemoryUsage, double previousMemoryUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.MemoryUsageAnomalyThresholdPercentage;
        var isAnomaly = currentMemoryUsage > (previousMemoryUsage * (1 + thresholdPercentage));
        return isAnomaly;
    }

    public bool IsCpuUsageAnomaly(double currentCpuUsage, double previousCpuUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.CpuUsageAnomalyThresholdPercentage;
        var isAnomaly = currentCpuUsage > (previousCpuUsage * (1 + thresholdPercentage));
        return isAnomaly;
    }

    public bool IsHighMemoryUsage(ServerStatistics currentStats)
    {
        double thresholdPercentage = _anomalyDetectionConfig.MemoryUsageThresholdPercentage;
        var isHighUsage = (currentStats.MemoryUsage / (currentStats.MemoryUsage + currentStats.AvailableMemory)) > thresholdPercentage;
        return isHighUsage;
    }

    public bool IsHighCpuUsage(double currentCpuUsage)
    {
        double thresholdPercentage = _anomalyDetectionConfig.CpuUsageThresholdPercentage;
        var isHighUsage = currentCpuUsage > thresholdPercentage;
        return isHighUsage;
    }
}
