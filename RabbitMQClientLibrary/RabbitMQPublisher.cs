using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQClientLibrary
{
    public class RabbitMQPublisher<T> : IMessageQueuePublisher<T>
    {
        private readonly RabbitMQConfig _rabbitMQConfig;

        public RabbitMQPublisher(IOptions<RabbitMQConfig> rabbitMQConfig)
        {
            _rabbitMQConfig = rabbitMQConfig.Value;
        }

        public void Publish(T payload, string key)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQConfig.HostName,
                Port = _rabbitMQConfig.Port,
                UserName = _rabbitMQConfig.UserName,
                Password = _rabbitMQConfig.Password
            };

            var messageBody = JsonConvert.SerializeObject(payload);

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(_rabbitMQConfig.ExchangeName, _rabbitMQConfig.ExchangeType);
            channel.BasicPublish(_rabbitMQConfig.ExchangeName, key, null, Encoding.UTF8.GetBytes(messageBody));
        }
    }
}
