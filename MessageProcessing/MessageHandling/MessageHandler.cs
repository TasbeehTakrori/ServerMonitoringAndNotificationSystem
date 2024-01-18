using MessageProcessing.MessageHandling;
using MessageProcessing.Repository;
using SignalRServer.AlertHubHandling;

internal class MessageHandler<T> : IMessageHandler<T>
{
    private readonly IRepository<T> _repository;
    private readonly IHubConnection _hubConnection;

    public MessageHandler(IRepository<T> repository, IHubConnection hubConnection)
    {
        _repository = repository;
        _hubConnection = hubConnection;
    }

    public async Task HandleMessage(T message)
    {
        try
        {
            await _repository.SaveAsync(message);
            await _hubConnection.SendAsync("SendHighUsageAlertMessage", "***");
            Console.WriteLine("Message sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling message: {ex}");
        }
    }
}