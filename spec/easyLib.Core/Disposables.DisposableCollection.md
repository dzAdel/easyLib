# interface IDisposableCollection
```csharp
interface IDisposableCollection
{  
  bool IsDisposed {get;}
  void Add(IDisposable disposable);
  bool Contains(IDisposable? disposable);  
}
```
## Invariant
```csharp
void Invariant()
{ 
  !Contains(null);
}
```
## Add(IDisposable)
```csharp
void Add(IDisposable disposable)
{
  require
  {
    disposable != null;
    !Contains(disposable);
  }
  ensure
  {    
    Contains(disposable);
  }
}
```
# class DisposableCollection
```csharp
class DisposableCollection: IDisposableCollection, IDestructible
{  
  public bool IsDisposed {get;}
  public void Add(IDisposable disposable);
  public bool Contains(IDisposable? disposable);
  public void Clear(bool disposeAll);
  public void Dispose();
}
```
## Dispose()
```csharp
public void Dispose()
{
  ensure
  {
    IsDisposed;
  }
}
```
# class ConcurrentDisposableCollection
```csharp
class ConcurrentDisposableCollection: IDisposableCollection, IDestructible
{
  public bool IsDisposed {get;}
  public void Add(IDisposable disposable);
  public void Clear(bool disposeAll);
  public bool Contains(IDisposable? disposable);
  public void Dispose();
}
```
## Dispose()
```csharp
public void Dispose()
{
  ensure
  {
    IsDisposed;
  }
}
```