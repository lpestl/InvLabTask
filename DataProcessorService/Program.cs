using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ModelLayer;
using System.Text.Json;
using DataProcessorService;
using Microsoft.Extensions.Configuration;
using Serilog;

// --- Entry point -------------------

int _isRunning = 0;
bool _isRabbitMQAlive = false;

// Read config
var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .Build();

var appSettings = new AppSettings();
config.GetSection("AppSettings").Bind(appSettings);

var receiverSettings = new ReceiverSettings();
config.GetSection("ReceiverSettings").Bind(receiverSettings);

var dbSettings = new DatabaseSettings();
config.GetSection("DatabaseSettings").Bind(dbSettings);

// Create and setup Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

// Create service endless loop
Log.Information("--- Starting data process...");

while (!IsServiceExitRequested())
{
    await ConnectToRabbitMQ();
    
    Thread.Sleep(TimeSpan.FromSeconds(appSettings.UpdateInterval));
    Log.Warning("- Reconnect to RabbitMQ");
}

Log.Information("--- Data processing finished");

Log.CloseAndFlush();

// -----------------------------------

// --- Dummy function for endless loop
bool IsServiceExitRequested()
{
    return false;
}

// --- Main loop function
async Task ReceivedMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Log.Information("Received JSON message");
    try
    {
        // Deserialize as InstrumentStatus
        var data = JsonSerializer.Deserialize<InstrumentStatus>(message);

        if (data != null)
        {
            Console.WriteLine($"Deserialized data: {data.PackageID}");
            // TODO:
        }
    }
    catch (Exception ex)
    {
        HandleException(ex);
    }
}

// --- Connect to RabbitMQ for receiving messages
async Task ConnectToRabbitMQ()
{
    try
    {
        // Connect to RabbitMQ
        var factory = new ConnectionFactory()
        {
            HostName = receiverSettings.HostName,
            Port = receiverSettings.Port,
            UserName = receiverSettings.UserName,
            Password = receiverSettings.Password
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        var queueName = receiverSettings.QueueName;
        await channel.QueueDeclareAsync(
            queue: queueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += ReceivedMessageAsync;
        consumer.ShutdownAsync += async (sender, ea) =>
        {
            _isRabbitMQAlive = false;
        };
        await channel.BasicConsumeAsync(
            queue: queueName, 
            autoAck: true, 
            consumer: consumer);
        
        _isRabbitMQAlive = true;
        
        // Run endless loop for wait disconnect
        do
        {
            Thread.Sleep(TimeSpan.FromSeconds(appSettings.UpdateInterval));
        }
        while (_isRabbitMQAlive);
    }
    catch (Exception ex)
    {
        HandleException(ex);
    }
}

// --- Helper for handling exceptions
void HandleException(Exception ex)
{
    Log.Error("Runtime error:");
    Log.Error("\t[{ExSource}]: {ExMessage}", ex.Source, ex.Message);
    if (ex.InnerException != null)
    {
        Log.Error("\t\t[{InnerExceptionSource}]: {InnerExceptionMessage}", ex.InnerException.Source, ex.InnerException.Message);
    }
}
