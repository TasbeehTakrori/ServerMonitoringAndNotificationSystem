
namespace RabbitMQClientLibrary
{
    public interface IMessageQueuePublisher<T>
    {
        void Publish(T payload, string key);
    }
}