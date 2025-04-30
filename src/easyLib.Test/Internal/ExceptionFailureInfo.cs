namespace easyLib.Test.Internal;

sealed class ExceptionFailureInfo : FailureInfo
{
    public ExceptionFailureInfo(string caption) :
        base(caption)
    { }

    public string? Action { get; init; }
    public Exception? CatchedException { get; init; }

    public Type? ExpectedExceptionType
    {
        get => m_exWanted;
        init
        {
            require(value == null || value.IsAssignableTo(typeof(Exception)));

            m_exWanted = value;
        }
    }

    //protected:
    protected override IEnumerable<string> GetReport()
    {
        if (!string.IsNullOrWhiteSpace(Action))
            yield return $"Tested action: {Action}";

        yield return $"Expected exception: {ExpectedExceptionType?.ToString() ?? "None"}";
        yield return $"Exception got: {CatchedException?.GetType().ToString() ?? "None"}";

        if (CatchedException != null)
            yield return $"Exception message: {CatchedException.Message}";
    }

    //private:
    readonly Type? m_exWanted;
}
