using ModelLayer.DeviceTypes.Modules;

namespace ModelLayer.DeviceTypes;

public static class PredefinedData
{
    public static readonly Dictionary<string, Type> ModuleNameToType = new Dictionary<string, Type>()
    {
        { "SAMPLER", typeof(CombinedSamplerStatus) },
        { "QUATPUMP", typeof(CombinedPumpStatus) },
        { "COLCOMP", typeof(CombinedOvenStatus) }
    };
}