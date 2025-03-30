namespace easyLib.Test;

public interface ITestResult
{
    string Caption { get; }
    bool IsFailure { get; }
    IEnumerable<string> Report { get; }
}
