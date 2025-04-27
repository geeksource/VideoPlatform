
using Common.Messaging.Events;
using Common.Messaging.Queues;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SubscriptionService.Services;
using System.Text;
using System.Text.Json;

namespace SubscriptionService.Messaging
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

            _channel.QueueDeclare(queue: RabbitMqQueues.Payment_Completed, durable: true, exclusive: false, autoDelete: false);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var eventObj = JsonSerializer.Deserialize<PaymentCompletedEvent>(messageJson);

                    using var scope = _serviceProvider.CreateScope();
                    var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                    var result = await subscriptionService.ActivateSubscription(eventObj.ReferenceId);

                    //using var scope = _serviceProvider.CreateScope();
                    //var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                    //string CheckoutSessionUrl = await paymentService.CreateCheckoutSessionAsync(eventObj);

                    //var messagePublisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
                    //await messagePublisher.PublishAsync(new PaymentLinkGeneratedEvent
                    //{
                    //    Email = eventObj.UserEmail,
                    //    CheckoutUrl = CheckoutSessionUrl,
                    //    UserId = eventObj.UserId,
                    //    PlanId = eventObj.PlanId,
                    //    SubscriptionId = "1",

                    //}, RabbitMqQueues.Payment_Link_Generated);

                };

                _channel.BasicConsume(queue: RabbitMqQueues.Payment_Completed, autoAck: true, consumer: consumer);

            }
            catch(Exception ex)
            {

            }
            return Task.CompletedTask;
        }
    }
}
