using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Net.Mail;
using System.Net;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Common.Messaging.Events;
using System.Text.Json;
using Common.Messaging.Queues;
using NotificationService.Services;

namespace NotificationService.RabbitMQConsumer
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private IModel _channel;
        private IConnection _connection;
        private readonly IEmailService _emailService;
        public Worker(IConfiguration configuration,IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            InitializeRabbitMq();
        }

        private void InitializeRabbitMq()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                UserName = _configuration["RabbitMq:Username"],
                Password = _configuration["RabbitMq:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: RabbitMqQueues.User_Registered, durable: true, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<UserRegisteredEvent>(json);

                if (message != null)
                {
                    await SendConfirmationEmailAsync(message.Email, message.ConfirmationLink);
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: RabbitMqQueues.User_Registered, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"]),
                Subject = "Confirm your email",
                Body = $"Click to confirm: <a href='{confirmationLink}'>{confirmationLink}</a>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            _emailService.SendEmail(mailMessage);
        }

    }
}
