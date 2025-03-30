namespace easyLib.Disposables;

public sealed class ConcurrentDisposableCollection : IDisposableCollection, IDestructible
{
    public ConcurrentDisposableCollection()
    {
        m_disposables = new();

        DisposablesTracker.Add(this);
    }

    public bool IsDisposed => m_disposables.IsDisposed;

    public bool Contains(IDisposable? disposable)
    {
        lock (m_disposables)
            return m_disposables.Contains(disposable);
    }

    public void Add(IDisposable disposable)
    {
        require(disposable != null);
        require(!Contains(disposable));

        lock (m_disposables)
            m_disposables.Add(disposable);
    }

    public void Clear(bool disposeAll)
    {
        lock (m_disposables)
            m_disposables.Clear(disposeAll);
    }

    public void Dispose()
    {
        if (!m_disposables.IsDisposed)
            lock (m_disposables)
                m_disposables.Dispose();

        DisposablesTracker.Remove(this);
    }

    //private:
    readonly DisposableCollection m_disposables; //lock
}

