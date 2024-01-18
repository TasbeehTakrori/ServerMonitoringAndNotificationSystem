using MessageProcessing;
using MessageProcessing.AnomalyDetection;
using MessageProcessing.MessageHandling;
using MessageProcessing.Repository;
using Microsoft.Extensions.Logging;

internal class MessageHandler : IMessageHandler
{
    private readonly IRepository _repository;
    private readonly IAnomalyDetector _anomalyDetector;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(
        IRepository repository,
        IAnomalyDetector anomalyDetector,
        ILogger<MessageHandler> logger)
    {
        _repository = repository;
        _anomalyDetector = anomalyDetector;
        _logger = logger;
    }

    public async Task HandleMessage(ServerStatistics serverStatistics, string routingKey)
    {
        try
        {
            string serverIdentifier = GetServerIdentifier(routingKey);
            serverStatistics.ServerIdentifier = serverIdentifier;
            var previousStatistic = await _repository.GetLastRecordForServer(serverIdentifier);
            await _repository.SaveAsync(serverStatistics);
            await _anomalyDetector.DetectAnomalies(
                currentStatistics: serverStatistics,
                previousStatistics: previousStatistic);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling message: {ex.Message}");
        }
    }

    private string GetServerIdentifier(string routingKey)
    {
        string[] parts = routingKey.Split('.');
        string serverIdentifier = parts.Length > 1 ? parts[1] : string.Empty;
        return serverIdentifier;
    }
}
