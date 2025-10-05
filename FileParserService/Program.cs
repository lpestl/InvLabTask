using System.Text.Json;
using FileParserService;
using Microsoft.Extensions.Configuration;
using Serilog;

// --- Entry point -------------------

int _isRunning = 0;

// Read config
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var settings = new AppSettings();
config.GetSection("AppSettings").Bind(settings);

// Create and setup Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

// Create service endless loop
while (!IsServiceExitRequested())
{
    var timer = new Timer(ServiceLoop, null, TimeSpan.Zero, TimeSpan.FromSeconds(settings.UpdateInterval));
    Thread.Sleep(TimeSpan.FromSeconds(settings.UpdateInterval));
}

Log.CloseAndFlush();

// -----------------------------------

// Dummy function for endless loop
bool IsServiceExitRequested()
{
    return false;
}

// Main loop function
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
        xmls = FileParser.GetXmlFiles(settings.PathToXmlDir);
    }
    catch (Exception ex)
    {
        HandleException(ex);
    }

    // Create own thread for each files
    Parallel.ForEach(xmls, xmlFileInfo =>
        {
            try
            {
                // Parse DeviceStatus
                Log.Information("Start parse xml file \"{ObjFullName}\"", xmlFileInfo.FullName);
                var status = FileParser.ParseFile(xmlFileInfo.FullName);
                Log.Information("Successfully parsed \"{ObjFullName}\"", xmlFileInfo.FullName);
                if (status != null)
                {
                    // Random change of ModuleState property 
                    FileParser.ChangeModuleStateProperties(status);
                    // Converting data to JSON
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        TypeInfoResolver =  new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver()
                    };
                    string jsonStr = JsonSerializer.Serialize(status, status.GetType(), options);
                    Log.Information(jsonStr);
                }
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }
    );
    Log.Information("--- Parsing finished");
    
    // Release lock
    Interlocked.Exchange(ref _isRunning, 0);
}

void HandleException(Exception ex)
{
    Log.Error("Runtime error:");
    Log.Error("\t[{ExSource}]: {ExMessage}", ex.Source, ex.Message);
    if (ex.InnerException != null)
    {
        Log.Error("\t\t[{InnerExceptionSource}]: {InnerExceptionMessage}", ex.InnerException.Source, ex.InnerException.Message);
    }
}