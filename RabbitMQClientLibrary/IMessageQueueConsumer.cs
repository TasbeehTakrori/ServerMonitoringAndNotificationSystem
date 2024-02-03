namespace RabbitMQClientLibrary
{
    public interface IMessageQueueConsumer<T>
    {
        void StartConsumingMessages(string key, Func<T, string, Task> handleMessage);
    }
}