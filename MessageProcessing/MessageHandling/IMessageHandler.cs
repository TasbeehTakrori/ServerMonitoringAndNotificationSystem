namespace MessageProcessing.MessageHandling
{
    internal interface IMessageHandler<T>
    {
        Task HandleMessage(T message);
    }
}