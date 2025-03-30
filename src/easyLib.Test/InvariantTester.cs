using System.Runtime.CompilerServices;

namespace easyLib.Test;

public interface IInvariantTester
{
    IInvariantTester this[bool exp,
                         [CallerLineNumber] int line = 0,
                         [CallerArgumentExpression(nameof(exp))] string? testExp = null]
    { get; }
}
