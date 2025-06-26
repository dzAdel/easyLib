namespace easyLib.Disposables;

public interface IDisposableCollection
{
    bool IsDisposed { get; }

    void Add(IDisposable disposable);
    bool Contains(IDisposable? disposable);
}
//-------------------------------------------------------------

public sealed class DisposableCollection : IDisposableCollection, IDestructible
{
    public DisposableCollection()
    {
        m_disposables = new();

        DisposablesTracker.Add(this);
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            Clear(true);
            IsDisposed = true;

            DisposablesTracker.Remove(this);
        }
    }

    public bool IsDisposed { get; private set; }

    public bool Contains(IDisposable? disposable) => disposable != null && m_disposables.Contains(disposable);

    public void Add(IDisposable disposable)
    {
        require(disposable != null);
        require(!Contains(disposable));

        m_disposables.Push(disposable);
    }

    public void Clear(bool disposeAll)
    {
        if (disposeAll)
            while (m_disposables.Count > 0)
                m_disposables.Pop().Dispose();
        else
            m_disposables.Clear();
    }


    //private:
    readonly Stack<IDisposable> m_disposables;
}

