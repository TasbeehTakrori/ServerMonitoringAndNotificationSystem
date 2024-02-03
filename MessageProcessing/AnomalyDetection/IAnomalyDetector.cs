using MessageProcessing.AnomalyDetection.Enum;

namespace MessageProcessing.AnomalyDetection
{
    internal interface IAnomalyDetector
    {
        List<AnomalyType> DetectAnomalies(ServerStatistics currentStatistics, ServerStatistics previousStatistics);
    }
}