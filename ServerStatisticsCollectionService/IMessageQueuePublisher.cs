using ServerStatisticsCollectionLibrary.Models;

namespace ServerStatisticsCollectionService
{
    public interface IMessageQueuePublisher
    {
        void PublishMessage(ServerStatistics serverStatistic, string topic);
    }
}