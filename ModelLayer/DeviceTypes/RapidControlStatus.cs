using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ModelLayer.DeviceTypes;

public class RapidControlStatus
{
    [XmlElement(ElementName="ModuleState")] 
    public ModuleState ModuleState { get; set; }
    
    [XmlElement(ElementName="IsBusy")] 
    public bool IsBusy { get; set; }
    
    [XmlElement(ElementName="IsReady")] 
    public bool IsReady { get; set; }
    
    [XmlElement(ElementName="IsError")] 
    public bool IsError { get; set; }
    
    [XmlElement(ElementName="KeyLock")] 
    public bool KeyLock { get; set; }
    
    [XmlIgnore]
    [JsonIgnore]
    public XmlSerializerNamespaces Namespaces { get; set; }
}