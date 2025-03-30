namespace easyLib.Test.Internal;

abstract class FailureInfo : ITestResult
{
    public string Caption { get; }
    public bool IsFailure => true;
    public string? CallerName { get; init; }
    public string? SourceFile { get; init; }
    public int LineNumber { get; init; }

    public IEnumerable<string> Report
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(CallerName))
                yield return $"In: {CallerName}";

            if (!string.IsNullOrWhiteSpace(SourceFile))
                yield return $"File: {SourceFile}";

            if (LineNumber > 0)
                yield return $"Line Number: {LineNumber}";

            foreach (string ln in GetReport())
            {
                string[] msg = ln.Split(Environment.NewLine,
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in msg)
                    yield return s;
            }
        }
    }

    //protected:
    protected FailureInfo(string caption)
    {
        require(!string.IsNullOrEmpty(caption));

        Caption = caption;
    }

    protected abstract IEnumerable<string> GetReport();
}
