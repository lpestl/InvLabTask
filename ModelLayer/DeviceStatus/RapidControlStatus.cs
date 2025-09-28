using System.Xml.Serialization;

namespace ModelLayer.DeviceStatus;

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
    public XmlSerializerNamespaces Namespaces { get; set; }
}