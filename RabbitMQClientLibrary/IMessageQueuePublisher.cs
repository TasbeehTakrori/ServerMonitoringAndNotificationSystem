
namespace RabbitMQClientLibrary
{
    public interface IMessageQueuePublisher<T>
    {
        void PublishMessage(T payload, string key);
    }
}