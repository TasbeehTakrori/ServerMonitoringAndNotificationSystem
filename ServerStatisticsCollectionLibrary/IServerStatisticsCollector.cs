using ServerStatisticsCollectionLibrary.Models;

namespace ServerStatisticsCollectionLibrary
{
    public interface IServerStatisticsCollector
    {
        ServerStatistics CollectStatistics();
    }
}