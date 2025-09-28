using System.Xml.Linq;
using System.Xml.Serialization;
using ModelLayer;
using ModelLayer.DeviceStatus;

namespace FileParserService;

public class FileParser(string filePath)
{
    private readonly string _filePath = filePath;

    public InstrumentStatus ParseInstrumentStatus()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("File not found", _filePath);
        
        var doc = XDocument.Load(_filePath);

        var instrumentalSerializer = new XmlSerializer(typeof(InstrumentStatus));
        using var reader = doc.CreateReader();
        if (instrumentalSerializer.Deserialize(reader) is InstrumentStatus status)
        {
            if (doc.Root != null)
            {
                status.Namespaces = new XmlSerializerNamespaces(
                    doc.Root.Attributes()
                        .Where(a => a.IsNamespaceDeclaration)
                        .Select(a =>
                        {
                            // xmlns="..." -> empty prefix
                            string prefix = a.Name.LocalName == "xmlns" ? "" : a.Name.LocalName;
                            return new System.Xml.XmlQualifiedName(prefix, a.Value);
                        }).ToArray()
                );

                return status;
            }
        }

        throw new Exception($"The structure inside the \"{_filePath}\" file does not match the data structure of \"InstrumentStatus\"");
    }

    static public RapidControlStatus ParseRapidControlStatus(Type moduleType, string rapidControlStatusXml)
    {
        using (var stringReader = new StringReader(rapidControlStatusXml))
        {
            XDocument doc = XDocument.Load(stringReader);
            var moduleSerializer = new XmlSerializer(moduleType);
           
            using (var reader = doc.CreateReader())
            {
                if (moduleSerializer.Deserialize(reader) is RapidControlStatus status)
                {
                    if (doc.Root != null)
                    {
                        status.Namespaces = new XmlSerializerNamespaces(
                            doc.Root.Attributes()
                                .Where(a => a.IsNamespaceDeclaration)
                                .Select(a =>
                                {
                                    // xmlns="..." -> empty prefix
                                    string prefix = a.Name.LocalName == "xmlns" ? "" : a.Name.LocalName;
                                    return new System.Xml.XmlQualifiedName(prefix, a.Value);
                                }).ToArray()
                        );

                        return status;
                    }
                }
            }
        }
        
        return Activator.CreateInstance(moduleType, rapidControlStatusXml) as RapidControlStatus;
    }
}