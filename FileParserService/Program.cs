using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using FileParserService;
using ModelLayer;

string TestXmlPath = "E:\\Hobby\\InvLabTask\\Task\\status.xml";

var parser = new FileParser(TestXmlPath);

try
{
    var status = parser.ParseInstrumentStatus();

    Console.WriteLine($"{status.SchemaVersion}, {status.PackageID}");
    foreach (var qn in status.Namespaces.ToArray())
        Console.WriteLine($"ns: {qn.Name} = {qn.Namespace}");
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.Source}: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"\t{ex.InnerException.Source}: {ex.InnerException.Message}");
    }
}
