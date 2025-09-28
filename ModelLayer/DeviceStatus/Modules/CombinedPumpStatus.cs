using System.Xml.Serialization;

namespace ModelLayer.DeviceStatus.Modules;

[XmlRoot(ElementName="CombinedPumpStatus")]
public class CombinedPumpStatus : RapidControlStatus
{
    [XmlElement(ElementName="Mode")] 
    public string Mode { get; set; }
    
    [XmlElement(ElementName="Flow")] 
    public int Flow { get; set; }
    
    [XmlElement(ElementName="PercentB")] 
    public float PercentB { get; set; }
    
    [XmlElement(ElementName="PercentC")] 
    public float PercentC { get; set; }
    
    [XmlElement(ElementName="PercentD")] 
    public float PercentD { get; set; }
    
    [XmlElement(ElementName="MinimumPressureLimit")] 
    public float MinimumPressureLimit { get; set; }
    
    [XmlElement(ElementName="MaximumPressureLimit")] 
    public float MaximumPressureLimit { get; set; }
    
    [XmlElement(ElementName="Pressure")] 
    public float Pressure { get; set; }
    
    [XmlElement(ElementName="PumpOn")] 
    public bool PumpOn { get; set; }
    
    [XmlElement(ElementName="Channel")] 
    public int Channel { get; set; }
}

// Example 
// <?xml version="1.0" encoding="utf-16"?>
// <CombinedPumpStatus 
//     xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
//     xmlns:xsd="http://www.w3.org/2001/XMLSchema">
//
//     <ModuleState>Online</ModuleState>
//     <IsBusy>false</IsBusy>
//     <IsReady>true</IsReady>
//     <IsError>false</IsError>
//     <KeyLock>false</KeyLock>
//     <Mode>ISO</Mode>
//     <Flow>0</Flow>
//     <PercentB>0</PercentB>
//     <PercentC>0</PercentC>
//     <PercentD>0</PercentD>
//     <MinimumPressureLimit>0</MinimumPressureLimit>
//     <MaximumPressureLimit>400.0330947751624</MaximumPressureLimit>
//     <Pressure>0</Pressure>
//     <PumpOn>false</PumpOn>
//     <Channel>0</Channel>
//
// </CombinedPumpStatus>
