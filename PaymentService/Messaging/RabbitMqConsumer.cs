
using Common.Messaging;
using Common.Messaging.Events;
using Common.Messaging.Queues;
using PaymentService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PaymentService.Messaging
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly IPaymentService _paymentService;
        private IConnection _connection;
        private IModel _channel;
        public RabbitMqConsumer(IServiceProvider serviceProvider/*,IPaymentService paymentService*/)
        {
            _serviceProvider = serviceProvider;
            //_paymentService = paymentService;

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
                var eventObj = JsonSerializer.Deserialize<SubscriptionRequestedEvent>(messageJson);

                using var scope = _serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

               string CheckoutSessionUrl= await paymentService.CreateCheckoutSessionAsync(eventObj);

               var messagePublisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
               await messagePublisher.PublishAsync(new PaymentLinkGeneratedEvent
                {
                   Email=eventObj.UserEmail,
                   CheckoutUrl = CheckoutSessionUrl,
                   UserId = eventObj.UserId,
                   PlanId = eventObj.PlanId,
                   SubscriptionId = "1",

               }, RabbitMqQueues.Payment_Link_Generated);

            };

            _channel.BasicConsume(queue: RabbitMqQueues.Subscription_Requested, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
