using MessageProcessing;
using MessageProcessing.AnomalyDetection;
using MessageProcessing.MessageHandling;
using MessageProcessing.Repository;
using SignalRServer.AlertHubHandling;

internal class MessageHandler : IMessageHandler
{
    private readonly IRepository _repository;
    private readonly IAnomalyDetector _anomalyDetector;

    public MessageHandler(
        IRepository repository,
        IAnomalyDetector anomalyDetector)
    {
        _repository = repository;
        _anomalyDetector = anomalyDetector;
    }

    public async Task HandleMessage(ServerStatistics serverStatistics)
    {
        try
        {
            var previousStatistic = await _repository.GetLastRecordForServer(serverStatistics.ServerIdentifier);
            await _repository.SaveAsync(serverStatistics);
            await _anomalyDetector.DetectAnomalies(
                currentStatistics: serverStatistics,
                previousStatistics: previousStatistic);
            Console.WriteLine("Message sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling message: {ex}");
        }
    }
}