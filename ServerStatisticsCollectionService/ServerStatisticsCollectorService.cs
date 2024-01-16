using Microsoft.Extensions.Options;
using RabbitMQClientLibrary;
using ServerStatisticsCollectionLibrary;
using ServerStatisticsCollectionLibrary.Models;

namespace ServerStatisticsCollectionService
{
    internal class ServerStatisticsCollectorService : IServerStatisticsCollectorService
    {
        private readonly ServerStatisticsConfig _config;
        private readonly IMessageQueuePublisher<ServerStatistics> _publisher;
        private readonly IServerStatisticsCollector _statisticsCollector;
        public ServerStatisticsCollectorService(
            IOptions<ServerStatisticsConfig> config,
            IMessageQueuePublisher<ServerStatistics> publisher,
            IServerStatisticsCollector statisticsCollector)
        {
            _config = config.Value;
            _publisher = publisher;
            _statisticsCollector = statisticsCollector;
        }
        public void Run()
        {
            while (true)
            {
                var statistics = _statisticsCollector.CollectStatistics();
                _publisher.Publish(statistics, $"ServerStatistics.{_config.ServerIdentifier}");
                Thread.Sleep(TimeSpan.FromSeconds(_config.SamplingIntervalSeconds));
            }
        }
    }
}
