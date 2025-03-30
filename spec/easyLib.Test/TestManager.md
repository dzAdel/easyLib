# class TestManager
```c# 
class TestManager
{
  public bool BreakOnFailure {get; set;}
  public IEnumerable<ITest> Tests {get;}
  public void AddTest(ITest test);
  public void Execute(int passCount = 1);
  public static string FormatTime(long ms);
}
```
## AddTest(ITest)
```csharp
public void AddTest(ITest test)
{
  require
  {
    test != null;
  }
  ensure
  {
    test.BreakOnFailure == BreakOnFailure;
  }
}
```
## BreakOnFailure
```csharp
public bool BreakOnFailure
{
  set
  {
    ensure
    {
      BreakOnFailure == value;
      Tests.All(BreakOnFailure == value);
    }
  }
}
```
## Execute(int)
```csharp
public void Execute(int passCount = 1)
{
  require
  {
    passCount >= 0;
  }
}
```
## FormatTime(long)
```csharp
public static string FormatTime(long ms)
{
  require
  {
    ms >= 0;
  }
  ensure
  {
    !string.IsNullOrWhiteSpace(Result);
  }
}
```