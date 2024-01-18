namespace MessageProcessing.MessageHandling
{
    internal interface IMessageHandler
    {
        Task HandleMessage(ServerStatistics serverStatistics);
    }
}