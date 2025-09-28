using System.Xml.Serialization;

namespace ModelLayer.DeviceStatus.Modules;

[XmlRoot(ElementName="CombinedSamplerStatus")]
public class CombinedSamplerStatus : RapidControlStatus
{
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
}

// Example
// <?xml version="1.0" encoding="utf-16"?>
// <CombinedSamplerStatus 
//      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
//      xmlns:xsd="http://www.w3.org/2001/XMLSchema">
//
//     <ModuleState>Online</ModuleState>
//     <IsBusy>false</IsBusy>
//     <IsReady>true</IsReady>
//     <IsError>false</IsError>
//     <KeyLock>false</KeyLock>
//     <Status>0</Status>
//     <Vial>L-A-1</Vial>
//     <Volume>0</Volume>
//     <MaximumInjectionVolume>0</MaximumInjectionVolume>
//     <RackL>Tray54C</RackL>
//     <RackR>Tray54C</RackR>
//     <RackInf>0</RackInf>
//     <Buzzer>true</Buzzer>
//
// </CombinedSamplerStatus>