using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ModelLayer;
using ModelLayer.DeviceTypes;

namespace FileParserService;

public class FileParser
{
    private readonly FileInfo _xmlFileInfo;
    public InstrumentStatus? InstrumentStatus { get; set; }
    public FileParser(FileInfo xmlFileInfo)
    {
        _xmlFileInfo = xmlFileInfo;
    }
    
    #region ~ Public methods ~

    public InstrumentStatus ParseFile()
    {
        if (_xmlFileInfo == null)
            throw new Exception("The xml file could not be parsed. Xml File is null.");

        if (!_xmlFileInfo.Exists)
            throw new Exception("The xml file could not be parsed. Xml File is not exist.");
        
        var status = ParseInstrumentStatus(_xmlFileInfo.FullName);

        foreach (var deviceStatus in status.DeviceStatus)
        {
            if (PredefinedData.ModuleNameToType.ContainsKey(deviceStatus.ModuleCategoryID))
            {
                deviceStatus.RapidControlStatus = ParseRapidControlStatus(
                    PredefinedData.ModuleNameToType[deviceStatus.ModuleCategoryID],
                    deviceStatus.RapidControlStatusXmlString);
            }
            else
                throw new Exception(
                    $"Predefined type for ModuleCategoryID \"{deviceStatus.ModuleCategoryID}\" not found");
        }
        
        InstrumentStatus = status;
        return status;
    }
    
    public void ChangeModuleStateProperties()
    {
        if (InstrumentStatus == null)
            throw new Exception("The instrument status could not be parsed. InstrumentStatus is null.");
        
        Random rnd = new Random();
        foreach (var deviceStatus in InstrumentStatus.DeviceStatus)
        {
            int value = rnd.Next(0, 4);
            deviceStatus.RapidControlStatus.ModuleState = (ModuleState)value;
        }
    }
    
    #endregion
    
    #region ~ Inner logic ~
    
    private T Parse<T>(StringReader xmlReader)
    {
        var serializer = new XmlSerializer(typeof(T));
        if (serializer.Deserialize(xmlReader) is T data)
        {
            return data;
        }
        
        //return default;
        throw new Exception("Xml File is not of type " + typeof(T).Name);
    }

    private Dictionary<string, string> GetNamespaces(string xmlString)
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

    private InstrumentStatus ParseInstrumentStatus(string filePath)
    {
        string xmlString = File.ReadAllText(filePath);
        using (var xmlReader = new StringReader(xmlString))
        {
            var status = Parse<InstrumentStatus>(xmlReader);
            status.Namespaces = GetNamespaces(xmlString);
            return status;
        }
    }

    private RapidControlStatus ParseRapidControlStatus(Type moduleType, string rapidControlStatusXml)
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
                                    return new XmlQualifiedName(prefix, a.Value);
                                }).ToArray()
                        );

                        return status;
                    }
                }
            }
        }

        //return default;
        throw new InvalidOperationException();
    }
    
    #endregion
}