using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Messaging
{
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IConfiguration _config;

        public RabbitMqMessagePublisher(IConfiguration config)
        {
            _config = config;
        }

        public Task PublishAsync<T>(T message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish("", queueName, null, body);
            return Task.CompletedTask;
        }
    }

}
