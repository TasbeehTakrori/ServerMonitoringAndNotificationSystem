using MessageProcessing.MessageHandling;
using MessageProcessing.Repository;
using RabbitMQClientLibrary;

namespace MessageProcessing
{
    internal class MessageProcessor : IMessageProcessor
    {
        private readonly IMessageQueueConsumer<ServerStatistics> _consumer;
        private readonly IMessageHandler _messageHandler;

        public MessageProcessor(
            IMessageQueueConsumer<ServerStatistics> consumer,
            IMessageHandler messageHandler)
        {
            _consumer = consumer;
            _messageHandler = messageHandler;
        }

        public void Run()
        {
            _consumer.StartConsumingMessages("ServerStatistics.*", _messageHandler.HandleMessage);
        }
    }
}
