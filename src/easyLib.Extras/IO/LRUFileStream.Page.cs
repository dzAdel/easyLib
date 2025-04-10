namespace easyLib.Extras.IO;

partial class LRUFileStream
{
    sealed class Page : IDestructible
    {
        public Page(long ndx, int buffMaxLen)
        {
            Index = ndx;
            Buffer = new(buffMaxLen, true);

            DisposablesTracker.Add(this);
        }

        public Buffer Buffer { get; }
        public long Index { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDisposed => Buffer.IsDisposed;

        public void Dispose()
        {
            if (!IsDisposed)
            {
                Buffer.Dispose();

                DisposablesTracker.Remove(this);
            }
        }
    }
}


