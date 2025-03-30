# interface ITest
```csharp
interface ITest: IDisposable
{
  string Name { get; }
  bool BreakOnFailure {get; set;}
  IEnumerable<ITestResult> Run();
}
```