using Common.Messaging;
using PaymentService.Messaging;
using PaymentService.Models;
using PaymentService.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();

builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddHostedService<RabbitMqConsumer>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();*/

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
