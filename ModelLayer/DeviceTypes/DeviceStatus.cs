using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ModelLayer.DeviceTypes;

[XmlRoot(ElementName="DeviceStatus")]
public class DeviceStatus { 

    [XmlElement(ElementName="ModuleCategoryID")] 
    public string ModuleCategoryID { get; set; } 

    [XmlElement(ElementName="IndexWithinRole")] 
    public int IndexWithinRole { get; set; } 

    [JsonIgnore]
    [XmlElement(ElementName="RapidControlStatus")] 
    public string RapidControlStatusXmlString { get; set; } 
    
    [XmlIgnore]
    [JsonConverter(typeof(PolymorphicConverter<RapidControlStatus>))]
    public RapidControlStatus RapidControlStatus { get; set; }
}