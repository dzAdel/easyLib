namespace easyLib.Test;

public interface ITest: IDisposable
{
    string Name { get; }
    bool BreakOnFailure { get; set; }

    IEnumerable<ITestResult> Run();
}
