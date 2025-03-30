namespace easyLib.Test.Internal;

sealed class InvariantFailureInfo : FailureInfo
{
    public InvariantFailureInfo() :
        base("Invariant test failed")
    { }

    public List<(string? expression, int line)> Expressions { get; } = new();

    //protected:
    protected override IEnumerable<string> GetReport()
    {
        foreach (var (exp, ln) in Expressions)
            yield return $"Invariant Line: {ln}, Expression: {exp}";
    }
}
