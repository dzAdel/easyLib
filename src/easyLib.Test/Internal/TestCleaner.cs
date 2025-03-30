namespace easyLib.Test.Internal;

sealed class TestCleaner : ITestCleaner
{
    public void Add(IDisposable disposable)
    {
        require(disposable != null);

        lock (m_disposables)
            if (!m_disposables.Contains(disposable))
                m_disposables.Push(disposable);
    }

    public void Clean()
    {
        lock (m_disposables)
            while (m_disposables.Count > 0)
                m_disposables.Pop().Dispose();
    }

    //private:
    readonly Stack<IDisposable> m_disposables = new();  //lock
}
