using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Common.Messaging.Events;
using NotificationService.Services;
using System.Net.Mail;
using Common.Messaging.Queues;

namespace NotificationService.RabbitMQConsumer
{
    public class PaymentLinkGeneratedConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly IPaymentService _paymentService;
        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _config;

        public PaymentLinkGeneratedConsumer(IServiceProvider serviceProvider, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _config = config;

            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: RabbitMqQueues.Subscription_Requested, durable: true, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var paymentLinkEvent = JsonSerializer.Deserialize<PaymentLinkGeneratedEvent>(messageJson);

                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var subject = "Your Subscription Payment Link";
                var bodyHtml = $"<p>Click <a href='{paymentLinkEvent.CheckoutUrl}'>here</a> to complete your subscription.</p>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:From"]),
                    Subject = subject,
                    Body = bodyHtml,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(paymentLinkEvent.Email);

                var result = emailService.SendEmail(mailMessage);
            };

            _channel.BasicConsume(queue: RabbitMqQueues.Payment_Link_Generated, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }

}
