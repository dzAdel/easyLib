using easyLib.Debug.Internal;
using System.Diagnostics;

namespace easyLib.Debug;

public sealed class DisposableTracker
{
    [Conditional("DEBUG")]
    public void Add(IDisposable disposable)
    {
        ArgumentNullException.ThrowIfNull(disposable);

        DisposbaleInfo dispInfo = new(disposable, disposable.GetType().Name, Environment.StackTrace);

        lock (m_lock)
            if (!Contains(disposable))
            {
                if (m_disposables.Length == m_ndxInsert)
                    Array.Resize(ref m_disposables, m_ndxInsert << 1);

                m_disposables[m_ndxInsert++] = dispInfo;

                if(m_topCount < m_ndxInsert)
                    m_topCount = m_ndxInsert;
            }
    }

    [Conditional("DEBUG")]
    public void Remove(IDisposable disposable)
    {
        ArgumentNullException.ThrowIfNull(disposable);

        lock (m_lock)
            for (int i = m_ndxInsert - 1; i >= 0; --i)
            {
                DisposbaleInfo? dispInfo = m_disposables[i];

                if (dispInfo?.Item == disposable)
                {
                    m_disposables[i] = null;

                    if (i == m_ndxInsert - 1)
                        do
                            --m_ndxInsert;
                        while (m_ndxInsert > 0 && m_disposables[m_ndxInsert - 1] == null);

                    break;
                }
            }
    }

    [Conditional("DEBUG")]
    public void AssertEmpty(TextWriter? logger = null)
    {
        List<string> msg = new()
        {
            $"# {nameof(DisposableTracker)}:",
            $"Container top count: {m_topCount:n0}"
        };

        int count = 0;

        lock (m_lock)
        {
            for (int i = m_ndxInsert - 1; i >= 0; --i)
            {
                DisposbaleInfo? dispInfo = m_disposables[i];

                if (dispInfo == null)
                    continue;

                ++count;

                msg.Add($"{dispInfo.ItemType} object non disposed:" +
                    $"\nStack trace:\n{dispInfo.StackTrace}");
            }

            if (count > 0)
            {
                msg.Add($"{count} object(s) nondisposed.");
                DisposeAll();
            }
            else
                msg.Add("All objects properly disposed.");
        }


        string str = "\n" + MessageFormatter.Format(0, msg.ToArray());

        logger?.WriteLine(str);
        System.Diagnostics.Debug.WriteLine(str);
    }


    //private:
    record DisposbaleInfo(IDisposable Item, string ItemType, string StackTrace);

    readonly object m_lock = new();
    DisposbaleInfo?[] m_disposables = new DisposbaleInfo?[4];
    int m_ndxInsert;
    int m_topCount;

    bool Contains(IDisposable disposable)
    {
        for (int i = 0; i < m_ndxInsert; ++i)
        {
            DisposbaleInfo? dispInfo = m_disposables[i];

            if (ReferenceEquals(dispInfo?.Item, disposable))
                return true;
        }

        return false;
    }

    void DisposeAll()
    {
        for (int i = m_ndxInsert - 1; i >= 0; --i)
            m_disposables[i]?.Item.Dispose();
    }
}
