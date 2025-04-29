using System.Diagnostics.CodeAnalysis;

namespace easyLib.IO;

public sealed partial class LRUFileStream : Stream, IDestructible
{
    public const int DefaultBufferSize = 512;
    public const int DefaultBufferCount = 8;

    public LRUFileStream(string filePath,
                         int buffSize = DefaultBufferSize,
                         int buffCount = DefaultBufferCount)
    {
        require(!string.IsNullOrWhiteSpace(filePath));
        require(buffSize > 0);
        require(buffSize <= Array.MaxLength);
        require(buffCount > 0);

        FilePath = filePath;
        m_maxPageCount = buffCount;
        m_buffCapacity = buffSize;
        m_pages = new();

        DisposablesTracker.Add(this);
    }

    public string FilePath { get; }

    [MemberNotNullWhen(true, nameof(m_fs))]
    public bool IsConnected => m_fs != null;

    public bool IsDisposed { get; private set; }


    [MemberNotNullWhen(true, nameof(m_fs))]
    public override bool CanRead => IsConnected;


    [MemberNotNullWhen(true, nameof(m_fs))]
    public override bool CanSeek => IsConnected;


    [MemberNotNullWhen(true, nameof(m_fs))]
    public override bool CanWrite => IsConnected && m_fs.CanWrite;

    public int BufferCount
    {
        get => m_maxPageCount;
        set
        {
            require(value > 0);

            if (value < m_pages.Count)
            {
                if (IsConnected && m_fs.CanWrite)
                    Flush();

                ClearCache();
            }
            else
                m_cacheHitCount = m_fetchCount = 0;

            m_maxPageCount = value;
        }
    }

    public int BufferSize
    {
        get => m_buffCapacity;
        set
        {
            require(value > 0);
            require(value <= Array.MaxLength);

            if (m_buffCapacity != value)
            {
                if (IsConnected && m_fs.CanWrite)
                    Flush();

                ClearCache();
                m_buffCapacity = value;
            }
        }
    }

    public double CacheHitFactor => (double)m_cacheHitCount / m_fetchCount;

    public override long Length
    {
        get
        {
            require(IsConnected);

            return m_len;
        }
    }

    public override long Position
    {
        get
        {
            require(IsConnected);

            return m_pos;
        }
        set
        {
            require(IsConnected);
            require(value >= 0);

            m_pos = value;
        }
    }

    public void Create(bool shareReading = false)
    {
        require(!IsConnected);

        FileStreamOptions opts = new()
        {
            Mode = FileMode.Create,
            Access = FileAccess.ReadWrite,
            Share = shareReading ? FileShare.Read : FileShare.None,
        };

        m_fs = new(FilePath, opts);
        DisposablesTracker.Add(m_fs);

        m_pos = m_len = 0;
    }

    public void Connect(bool readOnly = false, bool shareReading = false)
    {
        require(!IsConnected);
        require(File.Exists(FilePath));

        FileStreamOptions opts = new()
        {
            Mode = FileMode.Open,
            Access = readOnly ? FileAccess.Read : FileAccess.ReadWrite,
            Share = shareReading ? FileShare.Read : FileShare.None,
        };

        m_fs = new(FilePath, opts);
        DisposablesTracker.Add(m_fs);

        m_len = m_fs.Length;
        m_pos = 0;
    }

    public void Disconnect()
    {
        require(IsConnected);

        if (m_fs.CanWrite)
            Flush();

        m_fs.Dispose();
        DisposablesTracker.Remove(m_fs);
        m_fs = null;

        ClearCache();
    }

    public override int Read(Span<byte> span)
    {
        require(IsConnected);

        int totalRead = 0;

        if (m_len > m_pos)
        {
            (long ndxStartPage, int offBuffer) = GetPositionInfo(m_pos);

            FetchPage(ndxStartPage);
            assert(m_pages.First != null);
            assert(m_pages.First.Value.Buffer.Count >= offBuffer);

            Buffer buff = m_pages.First.Value.Buffer;
            int nbToRead = span.Length;
            int cpyCount = Math.Min(nbToRead, buff.Count - offBuffer);
            buff.CopyTo(span[..cpyCount], offBuffer);

            long pos = m_pos + cpyCount;
            totalRead = cpyCount;

            if (totalRead < nbToRead && pos < m_len)
            {
                int offset = cpyCount;
                nbToRead -= cpyCount;
                long ndxLastPage = Math.Min(GetPageIndex(pos + nbToRead), m_len / m_buffCapacity);

                for (long ndx = ndxStartPage + 1; ndx <= ndxLastPage; ++ndx)
                {
                    FetchPage(ndx);

                    buff = m_pages.First.Value.Buffer;
                    cpyCount = Math.Min(nbToRead, buff.Count);
                    buff.CopyTo(span.Slice(offset, cpyCount));
                    pos += cpyCount;
                    offset += cpyCount;
                    totalRead += cpyCount;
                    nbToRead -= cpyCount;
                }
            }

            m_pos = pos;
        }

        return totalRead;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        require(IsConnected);
        require(buffer != null);
        require(count >= 0);
        require(offset >= 0);
        require(count <= buffer.Length - offset);

        return Read(new Span<byte>(buffer, offset, count));
    }

    public override int ReadByte()
    {
        require(IsConnected);

        if (m_len <= m_pos)
            return -1;

        (long ndxPge, int offBuffer) = GetPositionInfo(m_pos);
        FetchPage(ndxPge);
        ++m_pos;

        assert(m_pages.First != null);
        return m_pages.First.Value.Buffer[offBuffer];
    }

    public override void Write(ReadOnlySpan<byte> span)
    {
        require(CanWrite);

        if (span.Length > 0)
        {
            if (m_len < m_pos)
            {
                PrepareCacheForExpansion(m_pos);

                assert(m_pages.All(p => p.Index <= GetPageIndex(m_pos)));
                assert(m_pages.Where(p => p.Index != GetPageIndex(m_pos)).All(p => p.Buffer.IsFull));
            }

            int nbToWrite = span.Length;
            long pos = m_pos;
            int offset = 0;

            do
            {
                (long ndxPage, int offBuffer) = GetPositionInfo(pos);

                FetchPage(ndxPage); //TODO: reading from file is not needed if nbToWrite == m_bufferCapacity
                assert(m_pages.First != null);

                int cpyCount = Math.Min(nbToWrite, m_buffCapacity - offBuffer);
                Page pge = m_pages.First.Value;
                pge.Buffer.CopyFrom(span.Slice(offset, cpyCount), offBuffer);
                pge.IsDirty = true;
                pos += cpyCount;
                offset += cpyCount;
                nbToWrite -= cpyCount;

            } while (nbToWrite > 0);

            m_pos = pos;

            if (m_len < m_pos)
                m_len = m_pos;

            assert(m_pages.First.Value.IsDirty);
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        require(CanWrite);
        require(buffer != null);
        require(offset >= 0);
        require(count >= 0);
        require(count <= buffer.Length - offset);

        Write(new ReadOnlySpan<byte>(buffer, offset, count));
    }

    public override void WriteByte(byte value)
    {
        require(CanWrite);

        if (m_len < m_pos)
            PrepareCacheForExpansion(m_pos);


        (long ndxPage, int offBuffer) = GetPositionInfo(m_pos);
        FetchPage(ndxPage);

        assert(m_pages.First != null);
        Page page = m_pages.First.Value;
        page.Buffer[offBuffer] = value;
        page.IsDirty = true;
        ++m_pos;

        if (m_len < m_pos)
            m_len = m_pos;
    }

    public override void Flush()
    {
        require(CanWrite);

        foreach (Page page in m_pages.Where(p => p.IsDirty).OrderBy(p => p.Index))
            SavePage(page);

        assert(m_pages.All(p => !p.IsDirty));

        m_fs.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        require(IsConnected);
        require(Enum.IsDefined(origin));
        require(origin != SeekOrigin.Begin || offset >= 0);
        require(origin == SeekOrigin.Begin || Position + offset >= 0);

        switch (origin)
        {
            case SeekOrigin.Begin:
                m_pos = offset;
                break;

            case SeekOrigin.Current:
                m_pos += offset;
                break;

            case SeekOrigin.End:
                m_pos = Length + offset;
                break;
        }

        return m_pos;
    }

    public override void SetLength(long len)
    {
        require(CanWrite);
        require(len >= 0);

        if (len == m_len)
            return;

        if (len < m_len)
        {
            LinkedListNode<Page>? node = m_pages.First;

            while (node != null)
            {
                Page page = node.Value;
                long pagePos = GetPagePosition(page.Index);

                LinkedListNode<Page>? nextNode = node.Next;

                if (pagePos >= len)
                    m_pages.Remove(node);
                else if (pagePos + page.Buffer.Count >= len)
                    page.Buffer.Strip((int)(len - pagePos));

                node = nextNode;
            }
        }
        else //m_len < len
        {
            PrepareCacheForExpansion(len);
            assert(m_fs.Length <= len);
        }

        m_fs.SetLength(len);
        m_len = len;

        if (m_pos > m_len)
            m_pos = m_len;
    }


    //protected:
    protected override void Dispose(bool disposing)
    {
        if (disposing && !IsDisposed)
        {
            if (IsConnected)
                try
                {
                    Disconnect();
                }
                catch (Exception ex)
                {
                    ex.WriteDebugMessage("Smothered exception");
                    assert(false);
                }

            IsDisposed = true;
            DisposablesTracker.Remove(this);
        }

        base.Dispose(disposing);
    }

    //private:
    readonly LinkedList<Page> m_pages;
    FileStream? m_fs;
    long m_pos;
    long m_len;
    int m_buffCapacity;
    int m_maxPageCount;
    uint m_fetchCount;
    uint m_cacheHitCount;

    (long ndxPage, int offBuffer) GetPositionInfo(long pos)
    {
        long ndxPage = pos / m_buffCapacity;
        int offsetInPage = (int)(pos - (ndxPage * m_buffCapacity));

        return (ndxPage, offsetInPage);
    }

    long GetPagePosition(long index) => index * m_buffCapacity;

    long GetPageIndex(long pos) => pos / m_buffCapacity;

    void FetchPage(long ndxPage)
    {
        ++m_fetchCount;
        LinkedListNode<Page>? node = m_pages.First;

        while (node != null)
        {
            if (node.Value.Index == ndxPage)
            {
                if (m_pages.First != node)
                {
                    m_pages.Remove(node);
                    m_pages.AddFirst(node);
                }

                ++m_cacheHitCount;
                return;
            }

            node = node.Next;
        }


        if (m_pages.Count == m_maxPageCount)
        {
            assert(m_pages.Last != null);
            node = m_pages.Last;
            Page page = node.Value;

            if (page.IsDirty)
                SavePage(page);

            m_pages.Remove(node);
            m_pages.AddFirst(node);
            page.Buffer.Clear();
        }
        else
        {
            node = new(new Page(ndxPage, m_buffCapacity));
            m_pages.AddFirst(node);
        }

        long pos = GetPagePosition(ndxPage);

        if (pos < m_len)
        {
            assert(IsConnected);
            m_fs.Position = pos;

            node.Value.Buffer.CopyFrom(m_fs, m_buffCapacity);
        }
    }

    void PrepareCacheForExpansion(long pos)
    {
        assert(pos > m_len);

        Page? page = m_pages.SingleOrDefault(p => !p.Buffer.IsFull);

        if (page != null)
        {
            var (ndxPage, offset) = GetPositionInfo(pos);

            //the last 2 assertions ensure that the non full page is the last one.
            assert(page.Index <= ndxPage);
            assert(page.Index != ndxPage || page.Buffer.Count < offset);
            assert(page.Index != ndxPage || m_len == (page.Index * m_buffCapacity) + page.Buffer.Count);

            if (page.Index != ndxPage)
                page.Buffer.Fill(0, m_buffCapacity - page.Buffer.Count, page.Buffer.Count);
            else
                page.Buffer.Fill(0, offset - page.Buffer.Count, page.Buffer.Count);

            // If the stream is expanded, the contents of the stream between the old and the new
            // length are not defined.(Stream.SetLength() ref.)
            // => save the garbage in [old page.Size, page.Size] to be synchronized with the underling storage
            SavePage(page);

            //adjust the stream length
            m_len = (page.Index * m_buffCapacity) + page.Buffer.Count;
            assert(m_fs != null && m_fs.Length == m_len);
        }
    }

    void SavePage(Page page)
    {
        assert(IsConnected);
        m_fs.Position = GetPagePosition(page.Index);

        page.Buffer.CopyTo(m_fs, page.Buffer.Count);
        page.IsDirty = false;
    }

    void ClearCache()
    {
        foreach (Page p in m_pages)
            p.Dispose();

        m_pages.Clear();
        m_cacheHitCount = m_fetchCount = 0;
    }
}

