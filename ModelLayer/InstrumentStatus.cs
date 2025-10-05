using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ModelLayer;

[XmlRoot(ElementName="InstrumentStatus")]
public class InstrumentStatus { 

    [XmlElement(ElementName="PackageID")] 
    public string PackageID { get; set; } 

    [XmlElement(ElementName="DeviceStatus")] 
    public List<DeviceTypes.DeviceStatus> DeviceStatus { get; set; } 

    [XmlAttribute(AttributeName="schemaVersion")] 
    public string SchemaVersion { get; set; }
    
    [XmlIgnore]
    [JsonIgnore]
    public Dictionary<string, string> Namespaces { get; set; }
}