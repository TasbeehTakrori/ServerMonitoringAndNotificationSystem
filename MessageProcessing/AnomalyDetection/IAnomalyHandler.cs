using MessageProcessing.AnomalyDetection.Enum;

namespace MessageProcessing.AnomalyDetection
{
    public interface IAnomalyHandler
    {
        Task HandleAnomalies(List<AnomalyType> anomalies, string serverIdentifier, DateTime timestamp);
    }
}