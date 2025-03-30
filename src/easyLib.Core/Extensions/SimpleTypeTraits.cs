namespace easyLib.Extensions;

public interface ISimpleTypeTraits
{
    bool IsNumeric { get; }
    bool IsFloatingPoint { get; }
    bool IsIntegral { get; }
    int Size { get; }
}
