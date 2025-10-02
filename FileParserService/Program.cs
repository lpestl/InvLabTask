using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using FileParserService;
using ModelLayer;
using ModelLayer.DeviceStatus;

string testXmlPath = "E:\\Hobby\\InvLabTask\\Task\\status.xml";

var parser = new FileParser(testXmlPath);

try
{
    var status = parser.ParseInstrumentStatus();

    if (status != null)
    {
        Console.WriteLine($"PackageId: {status.PackageID}\nDeviceStatuses: ");
        foreach (var deviceStatus in status.DeviceStatuses)
        {
            if (PredefinedData.ModuleNameToType.ContainsKey(deviceStatus.ModuleCategoryID))
            {
                deviceStatus.RapidControlStatus = FileParser.ParseRapidControlStatus(
                    PredefinedData.ModuleNameToType[deviceStatus.ModuleCategoryID], deviceStatus.RapidControlStatusXmlString);
                
                Console.WriteLine($"\tIndexWithinRole: {deviceStatus.IndexWithinRole}\n\tModuleCategotyID: " +
                                  $"{deviceStatus.ModuleCategoryID}\n\tRapidControlStatus:");
                Console.WriteLine($"\t\t{deviceStatus.RapidControlStatus.GetType()}: {deviceStatus.RapidControlStatus.ModuleState}");
            }
            else
                throw new Exception(
                    $"Predefined type for ModuleCategoryID \"{deviceStatus.ModuleCategoryID}\" not found");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Runtime error:");
    Console.WriteLine($"\t[{ex.Source}]: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"\t\t[{ex.InnerException.Source}]: {ex.InnerException.Message}");
    }
}
