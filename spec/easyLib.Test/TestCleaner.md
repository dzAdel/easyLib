# interface ITestCleaner
```csharp
interface ITestCleaner
{
  void Add(IDisposable disposable);
}
```
## Add(IDisposable)
```csharp
public void Add(IDisposable disposable)
{
  require
  {
    disposable != null;
  }
}
```
# class TestCleaner
```csharp
class TestCleaner: ITestCleaner
{
  public void Add(IDisposable disposable);  
  void Clean();
}
```