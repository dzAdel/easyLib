# class UnitTest
```csharp
{
  public string Name {get;}
  public bool BreakOnFailure {get;set;}
  public IEnumerable<ITestResult> Run();
  public void Dispose();

  //protected:
  protected UnitTest(string name);
  protected ITestCleaner Cleaner {get;}
  protected abstract void Start();
  protected void Ensure(bool exp, string? caller, string? file, int line, string? testExp);
  protected void EnsureThrow<T>(Action act, string? caller, string? file, int line, string? actStr)
                                where T: Exception;
  protected void EnsureNoThrow(Action act, string? caller, string? file, int line, string? actStr);
  protected void EnsureIfThrow<T>(Action act, Func<bool> fun, string? caller, string? file, int line,
                                  string? actStr, string? expStr) where T: Exception;
  protected void EnsureIfThrow(Action act, Func<bool> fun, string? caller, string? file, int line,
                                string? actStr, string? expStr);
  protected IInvariantTester GetInvariantTester(string? caller, string? file, int line);
  protected void Trace(string msg, params string[] lines);
}
```
## UnitTest(string)
```csharp
protected UnitTest(string name)
{
  require
  {
    !string.IsNullOrWhiteSpace(name);
  }
  ensure
  {
    Name == name;
  }
}
```
## EnsureIfThrow(Action, Func\<bool>, string?, string?, int, string?, string?)
```csharp
protected void EnsureIfThrow(Action act, Func<bool> fun, string? caller, string? file, int line,
                                string? actStr, string? expStr)
{
  require
  {
    act != null;
    fun != null;
  }
}
```
## EnsureIfThrow<T>(Action, Func\<bool>, string?, string?, int, string?, string?)
```csharp
protected void EnsureIfThrow<T>(Action act, Func<bool> fun, string? caller, string? file, int line, 
                                string? actStr, string? expStr) where T: Exception 
{
  require
  {
    act != null;
    fun != null;
  }
}
```
## EnsureNoThrow(Action, string?, string?, int, string?)
```csharp
protected void EnsureNoThrow(Action act, string? caller, string? file, int line, string? actStr)
{
  require
  {
    act != null;
  }
}
```
## EnsureThrow\<T>(Action, string?, string?, int, string?)
```csharp
protected void EnsureThrow<T>(Action act, string? caller, string? file, int line, string? actStr)
   where T: Exception
{
  require
  {
     act != null; 
  }
}
```
## void Trace(string, params string[])
```csharp
protected void Trace(string msg, params string[] lines)
{
  require
  {
    !string.IsNullOrWhteSpace(mgs);
  }
}
```
## class InvariantTester
```csharp
class InvariantTester: IInvariantTester
{
  public InvariantTester(UnitTest unitTest);
  public string? CallSite { get; init;}
  public string? SourceFile { get; init;}
  public int LineNumber { get; init;}
  public IInvariantTester this [bool exp, int line, string? testExp] { get; }
```
### InvariantTester(UnitTest)
```csharp
public InvariantTester(UnitTest unitTest)
{
  require
  {
    unitTest != null;
  }
  ensure
  {
    !AnyFailure;
  }
}
```
# class UnitTest\<T>
```csharp
abstract class UnitTest<T> : UnitTest
{
  //protected:
  protected UnitTest(string name);
  protected abstract IInvariantTester DefineInvariant(T obj, IInvariantTester invTester);
  protected void TestInvariant(T obj, string? caller, string? file, int line);
}
```
## UnitTest(string)
```csharp
protected UnitTest(string name)
{
  require
  {
    !string.IsNullOrWhiteSpace(name);
  }
  ensure
  {
    Name == name;
  }
}
```
## IInvariantTester DefineInvariant(T, IInvariantTester)
```csharp
protected abstract IInvariantTester DefineInvariant(T obj, IInvariantTester invTester)
{
  require
  {
    invTester != null;
  }
}
```