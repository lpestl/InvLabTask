using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using ModelLayer;

string TestXmlPath = "E:\\Hobby\\InvLabTask\\Task\\status.xml";

if (File.Exists(TestXmlPath))
{
    var doc = XDocument.Load(TestXmlPath);

    var serializer = new XmlSerializer(typeof(InstrumentStatus));
    InstrumentStatus status;
    using (var reader = doc.CreateReader())
    {
        status = (InstrumentStatus)serializer.Deserialize(reader);
    }

    var ns = new XmlSerializerNamespaces(
        doc.Root.Attributes()
            .Where(a => a.IsNamespaceDeclaration)
            .Select(a =>
            {
                // xmlns="..." -> пустой префикс
                string prefix = a.Name.LocalName == "xmlns" ? "" : a.Name.LocalName;
                return new System.Xml.XmlQualifiedName(prefix, a.Value);
            }).ToArray()
    );

    Console.WriteLine($"{status.SchemaVersion}, {status.PackageID}");
    foreach (var qn in ns.ToArray())
        Console.WriteLine($"ns: {qn.Name} = {qn.Namespace}");
}