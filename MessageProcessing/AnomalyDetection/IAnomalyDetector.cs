namespace MessageProcessing.AnomalyDetection
{
    internal interface IAnomalyDetector
    {
        Task DetectAnomalies(ServerStatistics currentStatistics, ServerStatistics previousStatistics);
    }
}