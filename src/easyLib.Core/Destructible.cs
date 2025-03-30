namespace easyLib;

public interface IDestructible : IDisposable
{
    bool IsDisposed { get; }
}
