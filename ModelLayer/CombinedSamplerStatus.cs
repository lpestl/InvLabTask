using System.Xml.Serialization;

namespace ModelLayer;

[XmlRoot(ElementName="CombinedSamplerStatus")]
public class CombinedSamplerStatus
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
    
    [XmlElement(ElementName="Status")] 
    public int Status  { get; set; }
    
    [XmlElement(ElementName="Vial")] 
    public string Vial {  get; set; }
    
    [XmlElement(ElementName="Volume")] 
    public int Volume  { get; set; }
    
    [XmlElement(ElementName="MaximumInjectionVolume")] 
    public int MaximumInjectionVolume  { get; set; }
    
    [XmlElement(ElementName="RackL")] 
    public string RackL {  get; set; }
    
    [XmlElement(ElementName="RackR")] 
    public string RackR { get; set; }
    
    [XmlElement(ElementName="RackInf")] 
    public int RackInf {  get; set; }
    
    [XmlElement(ElementName="Buzzer")] 
    public bool Buzzer { get; set; }
    
    [XmlIgnore]
    public XmlSerializerNamespaces  Namespaces { get; set; }
}