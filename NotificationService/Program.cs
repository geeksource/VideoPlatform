// File: Program.cs

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService;
using NotificationService.RabbitMQConsumer;
using NotificationService.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddHostedService<PaymentLinkGeneratedConsumer>();
        // Register email sender
        services.AddScoped<IEmailService, EmailService>();

        // Optionally, register config if needed elsewhere
        services.AddSingleton<IConfiguration>(hostContext.Configuration);
    })
    .Build();

await host.RunAsync();
