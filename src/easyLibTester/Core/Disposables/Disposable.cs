namespace easyLibTester.Core.Disposables;

sealed class Disposable : IDisposable
{
    public bool IsDisposed { get; private set; }
    public void Dispose() => IsDisposed = true;
}
