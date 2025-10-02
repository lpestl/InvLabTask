using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using FileParserService;
using Microsoft.Extensions.Configuration;
using ModelLayer.DeviceStatus;
using Serilog;

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

// Create parser and pars XML
try
{
    Log.Information("--- Starting parser process...");
    var parser = new FileParser();

    var xmls = parser.GetXmlFiles(settings.PathToXmlDir);

    foreach (var xmlFileInfo in xmls)
    {
        var status = parser.ParseInstrumentStatus(xmlFileInfo.FullName);

        if (status != null)
        {
            Log.Information($"PackageId: {status.PackageID}:");
            Log.Information("DeviceStatuses: ");
            foreach (var deviceStatus in status.DeviceStatuses)
            {
                if (PredefinedData.ModuleNameToType.ContainsKey(deviceStatus.ModuleCategoryID))
                {
                    deviceStatus.RapidControlStatus = FileParser.ParseRapidControlStatus(
                        PredefinedData.ModuleNameToType[deviceStatus.ModuleCategoryID],
                        deviceStatus.RapidControlStatusXmlString);

                    Log.Information($"\tIndexWithinRole: {deviceStatus.IndexWithinRole}");
                    Log.Information($"\tModuleCategotyID: {deviceStatus.ModuleCategoryID}");
                    Log.Information("\tRapidControlStatus:");
                    Log.Information($"\t\t{deviceStatus.RapidControlStatus.GetType()}: {deviceStatus.RapidControlStatus.ModuleState}");
                }
                else
                    throw new Exception(
                        $"Predefined type for ModuleCategoryID \"{deviceStatus.ModuleCategoryID}\" not found");
            }
        }
    }
    Log.Information("--- Parsing finished");
}
catch (Exception ex)
{
    Log.Error("Runtime error:");
    Log.Error($"\t[{ex.Source}]: {ex.Message}");
    if (ex.InnerException != null)
    {
        Log.Error($"\t\t[{ex.InnerException.Source}]: {ex.InnerException.Message}");
    }
}
finally
{
    Log.CloseAndFlush();
}
