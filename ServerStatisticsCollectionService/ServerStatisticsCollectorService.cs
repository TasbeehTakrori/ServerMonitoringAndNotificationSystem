using Microsoft.Extensions.Options;
using ServerStatisticsCollectionLibrary;

namespace ServerStatisticsCollectionService
{
    internal class ServerStatisticsCollectorService : IServerStatisticsCollectorService
    {
        private readonly ServerStatisticsConfig _config;
        private readonly IMessageQueuePublisher _publisher;
        private readonly IServerStatisticsCollector _statisticsCollector;
        public ServerStatisticsCollectorService(
            IOptions<ServerStatisticsConfig> config,
            IMessageQueuePublisher publisher,
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
                _publisher.PublishMessage(statistics, $"ServerStatistics.{_config.ServerIdentifier}");
                Thread.Sleep(TimeSpan.FromSeconds(_config.SamplingIntervalSeconds));
            }
        }
    }
}
