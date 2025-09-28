using System.Xml.Serialization;

namespace ModelLayer.DeviceStatus.Modules;

[XmlRoot(ElementName="CombinedOvenStatus")]
public class CombinedOvenStatus : RapidControlStatus
{
    [XmlElement(ElementName="UseTemperatureControl")] 
    public bool UseTemperatureControl { get; set; }
    
    [XmlElement(ElementName="OvenOn")] 
    public bool OvenOn { get; set; }
    
    [XmlElement(ElementName="Temperature_Actual")] 
    public float Temperature_Actual { get; set; }
    
    [XmlElement(ElementName="Temperature_Room")] 
    public float Temperature_Room { get; set; }
    
    [XmlElement(ElementName="MaximumTemperatureLimit")] 
    public float MaximumTemperatureLimit { get; set; }
    
    [XmlElement(ElementName="Valve_Position")] 
    public float Valve_Position { get; set; }
    
    [XmlElement(ElementName="Valve_Rotations")] 
    public float Valve_Rotations { get; set; }
    
    [XmlElement(ElementName="Buzzer")] 
    public bool Buzzer { get; set; }
}

// Example
// <?xml version="1.0" encoding="utf-16"?>
// <CombinedOvenStatus
//      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
//      xmlns:xsd="http://www.w3.org/2001/XMLSchema">
//
//     <ModuleState>Online</ModuleState>
//     <IsBusy>false</IsBusy>
//     <IsReady>true</IsReady>
//     <IsError>false</IsError>
//     <KeyLock>false</KeyLock>
//     <UseTemperatureControl>false</UseTemperatureControl>
//     <OvenOn>false</OvenOn>
//     <Temperature_Actual>26.22</Temperature_Actual>
//     <Temperature_Room>27.81</Temperature_Room>
//     <MaximumTemperatureLimit>0</MaximumTemperatureLimit>
//     <Valve_Position>0</Valve_Position>
//     <Valve_Rotations>0</Valve_Rotations>
//     <Buzzer>true</Buzzer>
// </CombinedOvenStatus>