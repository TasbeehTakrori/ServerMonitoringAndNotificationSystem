using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ServerStatisticsCollectionLibrary.Models;
using System.Text;

namespace ServerStatisticsCollectionService
{
    public class RabbitMQPublisher : IMessageQueuePublisher
    {
        private readonly RabbitMQConfig _rabbitMQConfig;

        public RabbitMQPublisher(IOptions<RabbitMQConfig> rabbitMQConfig)
        {
            _rabbitMQConfig = rabbitMQConfig.Value;
        }

        public void PublishMessage(ServerStatistics statistics, string topic)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQConfig.HostName,
                Port = _rabbitMQConfig.Port,
                UserName = _rabbitMQConfig.UserName,
                Password = _rabbitMQConfig.Password
            };

            var messageBody = JsonConvert.SerializeObject(statistics);

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(_rabbitMQConfig.ExchangeName, _rabbitMQConfig.ExchangeType);
            channel.BasicPublish(_rabbitMQConfig.ExchangeName, topic, null, Encoding.UTF8.GetBytes(messageBody));
        }
    }
}
