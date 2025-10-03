using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ModelLayer;
using ModelLayer.DeviceStatus;

namespace FileParserService;

public class FileParser
{
    private static T? Parse<T>(StringReader xmlReader)
    {
        var serializer = new XmlSerializer(typeof(T));
        if (serializer.Deserialize(xmlReader) is T data)
        {
            return data;
        }
        
        return default;
    }

    private static Dictionary<string, string> GetNamespaces(string xmlString)
    {
        var namespaces = new Dictionary<string, string>();
        using var xmlStringReader = new StringReader(xmlString);
        var doc = XDocument.Load(xmlStringReader);
        if (doc.Root != null)
        {
            foreach (var attribute in doc.Root.Attributes())
            {
                if (attribute.IsNamespaceDeclaration)
                {
                    string prefix = attribute.Name.LocalName == "xmlns" ? "" : attribute.Name.LocalName;
                    namespaces[prefix] = attribute.Value;
                }
            }
        }

        return namespaces;
    }

    public static List<FileInfo> GetXmlFiles(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory \"{directoryPath}\" does not exist");

        var xmlDir = new DirectoryInfo(directoryPath);
        return xmlDir.GetFiles("*.xml").ToList();
    }
    
    public InstrumentStatus? ParseInstrumentStatus(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);
        
        string xmlString = File.ReadAllText(filePath);
        using (var xmlReader = new StringReader(xmlString))
        {
            var status = Parse<InstrumentStatus>(xmlReader);

            if (status != null)
            {
                status.Namespaces = GetNamespaces(xmlString);
            }
            
            return status;
        }
    }

    public static RapidControlStatus ParseRapidControlStatus(Type moduleType, string rapidControlStatusXml)
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