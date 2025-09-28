using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using FileParserService;
using ModelLayer;
using ModelLayer.DeviceStatus;

string TestXmlPath = "E:\\Hobby\\InvLabTask\\Task\\status.xml";

var parser = new FileParser(TestXmlPath);

try
{
    var status = parser.ParseInstrumentStatus();

    foreach (var deviceStatus in status.DeviceStatus)
    {
        if (PredefinedData.ModuleNameToType.ContainsKey(deviceStatus.ModuleCategoryID))
        {
            RapidControlStatus moduleStatus = FileParser.ParseRapidControlStatus(
                PredefinedData.ModuleNameToType[deviceStatus.ModuleCategoryID], deviceStatus.RapidControlStatus);
            
            Console.WriteLine(moduleStatus.ModuleState);
        }
        else
            throw new Exception($"Predefined type for ModuleCategoryID \"{deviceStatus.ModuleCategoryID}\" not found");
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
