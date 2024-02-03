using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RabbitMQPublisher<T>> _logger;

        public RabbitMQPublisher(
            IOptions<RabbitMQConfig> rabbitMQConfig,
             ILogger<RabbitMQPublisher<T>> logger)
        {
            var _rabbitMQConfig = rabbitMQConfig.Value;
            _logger = logger;

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
            try
            {
                var messageBody = JsonConvert.SerializeObject(payload);

                _channel.BasicPublish(_exchangeName, key, null, Encoding.UTF8.GetBytes(messageBody));
                _logger.LogInformation($"New message published successfully, Message: {messageBody}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing message: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
