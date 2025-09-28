using System.Xml;
using System.Xml.Serialization;

namespace ModelLayer;

[XmlRoot(ElementName="InstrumentStatus")]
public class InstrumentStatus { 

    [XmlElement(ElementName="PackageID")] 
    public string PackageID { get; set; } 

    [XmlElement(ElementName="DeviceStatus")] 
    public List<DeviceStatus.DeviceStatus> DeviceStatus { get; set; } 

    [XmlAttribute(AttributeName="schemaVersion")] 
    public string SchemaVersion { get; set; }
    
    [XmlIgnore]
    public XmlSerializerNamespaces  Namespaces { get; set; }
}