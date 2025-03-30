namespace easyLib.Test.Internal;

class AssertionFailureInfo : FailureInfo
{
    public AssertionFailureInfo(string caption) :
        base(caption)
    { }

    public string? TestExpression { get; init; }

    //protected:
    protected override IEnumerable<string> GetReport()
    {
        if (!string.IsNullOrWhiteSpace(TestExpression))
            yield return $"Test expression: {TestExpression}";
    }
}
