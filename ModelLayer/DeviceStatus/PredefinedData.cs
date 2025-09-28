using ModelLayer.DeviceStatus.Modules;

namespace ModelLayer.DeviceStatus;

public static class PredefinedData
{
    public static readonly Dictionary<string, Type> ModuleNameToType = new Dictionary<string, Type>()
    {
        { "SAMPLER", typeof(CombinedSamplerStatus) },
        { "QUATPUMP", typeof(CombinedPumpStatus) },
        { "COLCOMP", typeof(CombinedOvenStatus) }
    };
}