using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQClientLibrary
{
    public class RabbitMQConsumer<T> : IMessageQueueConsumer<T>, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _exchangeType;
        private readonly string _queueName;
        private readonly ILogger<RabbitMQConsumer<T>> _logger;

        public RabbitMQConsumer(
            IOptions<RabbitMQConfig> rabbitMQConfig,
            ILogger<RabbitMQConsumer<T>> logger)
        {
            var _rabbitMQConfig = rabbitMQConfig.Value;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQConfig.HostName,
                Port = _rabbitMQConfig.Port,
                UserName = _rabbitMQConfig.UserName,
                Password = _rabbitMQConfig.Password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _exchangeName = _rabbitMQConfig.ExchangeName;
            _exchangeType = _rabbitMQConfig.ExchangeType;

            _channel.ExchangeDeclare(_exchangeName, _exchangeType);

            _queueName = _channel.QueueDeclare().QueueName;
        }

        public void StartConsumingMessages(string key, Func<T, string, Task> handleMessage)
        {

            _channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: key);

            var consumerAsync = new AsyncEventingBasicConsumer(_channel);

            consumerAsync.Received += async (model, ea) =>
            {
                try
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var payload = JsonConvert.DeserializeObject<T>(message);

                    await handleMessage(payload, ea.RoutingKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error handling RabbitMQ message: {ex.Message}", ex);
                }
            };

            _channel.BasicConsume(
                queue: _queueName,
                     autoAck: true,
                     consumer: consumerAsync);

            _logger.LogInformation($"RabbitMQConsumer started consuming messages. Exchange: {_exchangeName}, Queue: {_queueName}, Time: {DateTime.UtcNow}");
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
