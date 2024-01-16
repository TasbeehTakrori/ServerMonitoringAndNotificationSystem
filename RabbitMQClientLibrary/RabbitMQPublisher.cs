using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQClientLibrary
{
    public class RabbitMQPublisher<T> : IMessageQueuePublisher<T>, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _exchangeType;

        public RabbitMQPublisher(IOptions<RabbitMQConfig> rabbitMQConfig)
        {
            var _rabbitMQConfig = rabbitMQConfig.Value;

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQConfig.HostName,
                Port = _rabbitMQConfig.Port,
                UserName = _rabbitMQConfig.UserName,
                Password = _rabbitMQConfig.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = _rabbitMQConfig.ExchangeName;
            _exchangeType = _rabbitMQConfig.ExchangeType;
            _channel.ExchangeDeclare(_exchangeName, _exchangeType);
        }

        public void PublishMessage(T payload, string key)
        {
            var messageBody = JsonConvert.SerializeObject(payload);

            _channel.BasicPublish(_exchangeName, key, null, Encoding.UTF8.GetBytes(messageBody));
        }
        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

    }
}
