namespace easyLib.Disposables;

public sealed class AutoReleaser : IDestructible
{
    public AutoReleaser(Action releaser)
    {
        require(releaser != null);

        m_releaser = releaser;

        DisposablesTracker.Add(this);
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            try
            {
                m_releaser();
                IsDisposed = true;
            }
            catch (Exception ex)
            {
                ex.WriteDebugMessage("Smothered exception");
                assert(false, "Unexpected exception");
            }

            DisposablesTracker.Remove(this);
        }
    }

    public bool IsDisposed { get; private set; }
    public static AutoReleaser Empty => m_emptyReleaser ??= new(() => { });


    //private:
    static AutoReleaser? m_emptyReleaser;
    readonly Action m_releaser;
}
