using System.Xml.Serialization;

namespace ModelLayer;

[XmlRoot(ElementName="InstrumentStatus")]
public class InstrumentStatus { 

    [XmlElement(ElementName="PackageID")] 
    public string PackageID { get; set; } 

    [XmlElement(ElementName="DeviceStatus")] 
    public List<DeviceStatus> DeviceStatus { get; set; } 

    [XmlAttribute(AttributeName="xsi")] 
    public string Xsi { get; set; } 

    [XmlAttribute(AttributeName="xsd")] 
    public string Xsd { get; set; } 

    [XmlAttribute(AttributeName="schemaVersion")] 
    public string SchemaVersion { get; set; } 

    [XmlText] 
    public string Text { get; set; } 
}