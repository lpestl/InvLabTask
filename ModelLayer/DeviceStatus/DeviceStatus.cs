using System.Xml.Serialization;

namespace ModelLayer.DeviceStatus;

[XmlRoot(ElementName="DeviceStatus")]
public class DeviceStatus { 

    [XmlElement(ElementName="ModuleCategoryID")] 
    public string ModuleCategoryID { get; set; } 

    [XmlElement(ElementName="IndexWithinRole")] 
    public int IndexWithinRole { get; set; } 

    [XmlElement(ElementName="RapidControlStatus")] 
    public string RapidControlStatusXmlString { get; set; } 
    
    [XmlIgnore]
    public RapidControlStatus RapidControlStatus { get; set; }
}