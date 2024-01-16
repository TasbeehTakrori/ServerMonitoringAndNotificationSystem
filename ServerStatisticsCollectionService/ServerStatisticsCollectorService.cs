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
        //TODO => Add Logging
        public ServerStatisticsCollectorService(
            IOptions<ServerStatisticsConfig> config,
            IMessageQueuePublisher<ServerStatistics> publisher,
            IServerStatisticsCollector statisticsCollector)
        {
            _config = config.Value;
            _publisher = publisher;
            _statisticsCollector = statisticsCollector;
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var statistics = _statisticsCollector.CollectStatistics();
                _publisher.PublishMessage(statistics, $"ServerStatistics.{_config.ServerIdentifier}");
                Console.WriteLine("Publish...");

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_config.SamplingIntervalSeconds), cancellationToken);
                }
                catch (TaskCanceledException)
                { }
            }
        }
    }
}
