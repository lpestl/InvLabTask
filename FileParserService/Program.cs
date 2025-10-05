using System.Text;
using System.Text.Json;
using FileParserService;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;

// --- Entry point -------------------

int _isRunning = 0;

// Read config
var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .Build();

var appSettings = new AppSettings();
config.GetSection("AppSettings").Bind(appSettings);

var publisherSettings = new PublisherSettings();
config.GetSection("PublisherSettings").Bind(publisherSettings);

// Create and setup Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

// Create service endless loop
var timer = new Timer(ServiceLoop, null, TimeSpan.Zero, TimeSpan.FromSeconds(appSettings.UpdateInterval));
while (!IsServiceExitRequested())
{
    // Update tick
}

Log.CloseAndFlush();

// -----------------------------------

// --- Dummy function for endless loop
bool IsServiceExitRequested()
{
    return timer == null;
}

// --- Main loop function
void ServiceLoop(object? state)
{
    // Lock flag for detect already running thread
    if (Interlocked.Exchange(ref _isRunning, 1) == 1)
    {
        // already running. Skip current iteration
        return;
    }
    
    Log.Information("--- Starting parser process...");

    // Get files list in directory
    List<FileInfo> xmls = new List<FileInfo>();
    try
    {
        // Get all files in path
        xmls = GetXmlFiles(appSettings.PathToXmlDir);
    }
    catch (Exception ex)
    {
        HandleException(ex);
    }

    // Create own thread for each files
    List<Task> tasks = new List<Task>();
    foreach (var xml in xmls)
    {
        tasks.Add(HandleFileAsync(xml));
    }
    Task.WaitAll(tasks.ToArray());
    
    Log.Information("--- Parsing finished");
    
    // Release lock
    Interlocked.Exchange(ref _isRunning, 0);
}

async Task HandleFileAsync(FileInfo xmlFileInfo)
{
    // Parse DeviceStatus
    var parser = new FileParser(xmlFileInfo);

    // Run async parsing 
    await Task.Run(() =>
    {
        try
        {
            Log.Information("Start parse xml file \"{ObjFullName}\"", xmlFileInfo.FullName);
            parser.ParseFile();
            Log.Information("Successfully parsed \"{ObjFullName}\"", xmlFileInfo.FullName);
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    });

    if (parser.InstrumentStatus != null)
    {
        try
        {
            // Random change of ModuleState property 
            parser.ChangeModuleStateProperties();
            // Converting data to JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonStr = JsonSerializer.Serialize(parser.InstrumentStatus,
                parser.InstrumentStatus.GetType(), options);

            // Sent to DataProcessorService using RabbitMQ
            await SentToQueue(jsonStr);
        }
        catch (Exception e)
        {
            HandleException(e);
        }
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

// --- Collect xml files by path
List<FileInfo> GetXmlFiles(string directoryPath)
{
    if (!Directory.Exists(directoryPath))
        throw new DirectoryNotFoundException($"Directory \"{directoryPath}\" does not exist");

    var xmlDir = new DirectoryInfo(directoryPath);
    return xmlDir.GetFiles("*.xml").ToList();
}

// -- Sending message to queue using RabbitMQ
async Task SentToQueue(string message)
{
    // Connect to RabbitMQ
    var factory = new ConnectionFactory()
    {
        HostName = publisherSettings.HostName,
        Port = publisherSettings.Port,
        UserName = publisherSettings.UserName,
        Password = publisherSettings.Password,
    };
    
    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();
    
    // Create a queue (if it doesn't exist)
    await channel.QueueDeclareAsync(
        queue: publisherSettings.QueueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);
    
    var messageBody = Encoding.UTF8.GetBytes(message);
    
    await channel.BasicPublishAsync(
        exchange: "",
        routingKey: publisherSettings.QueueName,
        body: messageBody);
    
    Log.Information("Sent JSON message to queue");
}