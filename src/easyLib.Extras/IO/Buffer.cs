using easyLib.IO;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace easyLib.Extras.IO;

public sealed partial class Buffer : IReadOnlyBuffer, IDestructible
{
    public Buffer(int maxLen, bool reserve = false)
    {
        require(maxLen >= 0);
        require(maxLen <= Array.MaxLength);

        m_buffer = reserve ? ArrayPool.Rent<byte>(maxLen) : ArrayPool.Rent<byte>(Math.Min(maxLen, sizeof(decimal)));

        Capacity = maxLen;

        DisposablesTracker.Add(this);
    }

    public Buffer() :
        this(Array.MaxLength, false)
    { }

    public Buffer(Buffer other)
    {
        require(other != null);

        m_buffer = ArrayPool.Rent<byte>(other.m_buffer.Length);

        unsafe
        {
            fixed (byte* ptrSrc = other.m_buffer)
                CopyFrom(ptrSrc, other.Count, 0);
        }

        Count = other.Count;
        Capacity = other.Capacity;

        DisposablesTracker.Add(this);
    }

    public bool IsDisposed { get; private set; }
    public int Capacity { get; private set; }
    public int Count { get; private set; }
    public bool IsEmpty => Count == 0;
    public bool IsFull => Count == Capacity;
    public IEnumerator<byte> GetEnumerator() => m_buffer.Take(Count).GetEnumerator();

    public byte this[int ndx]
    {
        get
        {
            require(ndx >= 0);
            require(ndx < Count);

            return m_buffer[ndx];
        }
        set
        {
            require(ndx >= 0);
            require(ndx <= Count);
            require(ndx != Count || !IsFull);

            if (ndx == Count)
            {
                if (m_buffer.Length == Count)
                    ResizeBuffer(Count + 1);

                m_buffer[Count++] = value;
            }
            else
                m_buffer[ndx] = value;
        }
    }

    public ReadOnlySpan<byte> AsReadOnlySpan() => new(m_buffer, 0, Count);

    public ReadOnlySpan<byte> AsReadOnlySpan(int count, int ndxStart = 0)
    {
        require(count >= 0);
        require(ndxStart >= 0);
        require(count <= Count - ndxStart);

        return new(m_buffer, ndxStart, count);
    }

    public Span<byte> AsSpan() => new(m_buffer, 0, Count);

    public Span<byte> AsSpan(int count, int ndxStart = 0)
    {
        require(count >= 0);
        require(ndxStart >= 0);
        require(count <= Count - ndxStart);

        return new(m_buffer, ndxStart, count);
    }

    public void Add(byte b)
    {
        require(!IsFull);

        if (m_buffer.Length == Count)
            ResizeBuffer(Count + 1);

        m_buffer[Count++] = b;
    }

    public int Add(IEnumerable<byte> bytes)
    {
        require(bytes != null);

        return Put(bytes, Count);
    }

    public int Put(IEnumerable<byte> src, int ndxTo = 0)
    {
        require(src != null);
        require(ndxTo >= 0);
        require(ndxTo <= Count);

        if (src is byte[] bytes)
        {
            int sz = bytes.Length + ndxTo;

            if (sz > Capacity)
                throw new OverflowException();

            if (m_buffer.Length < sz)
                ResizeBuffer(sz);

            unsafe
            {
                fixed (byte* ptr = bytes)
                    CopyFrom(ptr, bytes.Length, ndxTo);
            }

            Count = Math.Max(Count, sz);

            return bytes.Length;
        }

        using IEnumerator<byte> enumerator = src.GetEnumerator();

        if (src.TryGetNonEnumeratedCount(out int n))
        {
            int sz = n + ndxTo;

            if (sz > Capacity)
                throw new OverflowException();

            if (m_buffer.Length < sz)
                ResizeBuffer(sz);

            while (enumerator.MoveNext())
                m_buffer[ndxTo++] = enumerator.Current;

            Count = Math.Max(Count, ndxTo);

            return n;
        }


        n = ndxTo;

        while (enumerator.MoveNext())
        {
            if (n == Capacity)
                throw new OverflowException();

            if (n == m_buffer.Length)
            {
                Count = n;
                ResizeBuffer(n + 1);
            }

            m_buffer[n++] = enumerator.Current;
        }

        Count = Math.Max(Count, n);

        return n - ndxTo;
    }

    public void Strip(int count)
    {
        require(count >= 0);
        require(count <= Count);

        Count -= count;
    }

    public void Clear() => Count = 0;

    public void Fill(byte b, int count, int ndxStart = 0)
    {
        require(count >= 0);
        require(ndxStart >= 0);
        require(ndxStart <= Count);
        require(count <= Capacity - ndxStart);

        int newSize = count + ndxStart;

        if (m_buffer.Length < newSize)
            ResizeBuffer(newSize);

        unsafe
        {
            fixed (byte* ptrBuffer = m_buffer)
            {
                byte* ptr = ptrBuffer + ndxStart;

                while (count-- > 0)
                    *ptr++ = b;
            }
        }

        Count = Math.Max(newSize, Count);
    }

    public void Reverse(int count, int ndxStart = 0)
    {
        require(count >= 0);
        require(ndxStart >= 0);
        require(count <= Count - ndxStart);

        unsafe
        {
            fixed (byte* ptrBuffer = m_buffer)
            {
                byte* ptrBegin = ptrBuffer + ndxStart;
                byte* ptrEnd = ptrBegin + count - 1;

                while (ptrBegin < ptrEnd)
                {
                    byte b = *ptrBegin;
                    *ptrBegin++ = *ptrEnd;
                    *ptrEnd-- = b;
                }
            }
        }
    }

    public void ReverseSlice(int chunkCount, int szChunk, int ndxStart = 0)
    {
        require(szChunk > 0);
        require(chunkCount >= 0);
        require(ndxStart >= 0);
        require(szChunk * chunkCount >= 0);
        require(szChunk * chunkCount <= Count - ndxStart);

        int n = 0;

        unsafe
        {
            fixed (byte* ptrBuffer = m_buffer)
            {
                byte* ptrChunck = ptrBuffer + ndxStart;

                while (n < chunkCount)
                {
                    byte* ptrStart = ptrChunck;
                    byte* ptrEnd = ptrStart + szChunk - 1;

                    while (ptrStart < ptrEnd)
                    {
                        byte tmp = *ptrStart;
                        *ptrStart++ = *ptrEnd;
                        *ptrEnd-- = tmp;
                    }

                    ++n;
                    ptrChunck += szChunk;
                }
            }
        }
    }

    public void CopyTo(Span<byte> dest, int ndxFrom = 0)
    {
        require(ndxFrom >= 0);
        require(dest.Length <= Count - ndxFrom);

        int len = dest.Length;

        if (len > 0)
            unsafe
            {
                fixed (byte* ptr = dest)
                    CopyTo(ptr, len, ndxFrom);
            }
    }

    public void CopyTo(Buffer dest, int count, int ndxTo = 0, int ndxFrom = 0)
    {
        require(dest != null);
        require(count >= 0);
        require(ndxFrom >= 0);
        require(count <= Count - ndxFrom);
        require(ndxTo >= 0);
        require(ndxTo <= dest.Count);
        require(count <= dest.Capacity - ndxTo);

        int sz = ndxTo + count;

        if (dest.m_buffer.Length < sz)
            dest.ResizeBuffer(sz);

        unsafe
        {
            fixed (byte* ptr = dest.m_buffer)
                CopyTo(ptr + ndxTo, count, ndxFrom);
        }

        dest.Count = Math.Max(dest.Count, sz);
    }

    public void CopyTo(Stream dest, int count, int ndxFrom = 0)
    {
        require(dest != null);
        require(dest.CanWrite);
        require(count >= 0);
        require(ndxFrom >= 0);
        require(count <= Count - ndxFrom);

        dest.Write(m_buffer, ndxFrom, count);
    }

    public void CopyFrom(ReadOnlySpan<byte> src, int ndxTo = 0)
    {
        require(ndxTo >= 0);
        require(ndxTo <= Count);
        require(src.Length <= Capacity - ndxTo);

        int len = src.Length;

        if (len > 0)
        {
            int size = len + ndxTo;

            if (m_buffer.Length < size)
                ResizeBuffer(size);

            unsafe
            {
                fixed (byte* ptr = src)
                    CopyFrom(ptr, len, ndxTo);
            }

            Count = Math.Max(Count, size);
        }
    }

    public void CopyFrom(IReadOnlyBuffer src, int count, int ndxTo = 0, int ndxFrom = 0)
    {
        require(src != null);
        require(count >= 0);
        require(ndxFrom >= 0);
        require(ndxTo >= 0);
        require(ndxTo <= Count);
        require(count <= Capacity - ndxTo);
        require(count <= src.Count - ndxFrom);

        if (m_buffer.Length < ndxTo + count)
            ResizeBuffer(ndxTo + count);

        src.CopyTo(m_buffer.AsSpan(ndxTo, count), ndxFrom);
        Count = Math.Max(Count, count + ndxTo);
    }

    public int CopyFrom(Stream src, int count, int ndxTo = 0)
    {
        require(src != null);
        require(src.CanRead);
        require(count >= 0);
        require(ndxTo >= 0);
        require(ndxTo <= Count);
        require(count <= Capacity - ndxTo);

        if (m_buffer.Length < ndxTo + count)
            ResizeBuffer(ndxTo + count);

        int nbRead = 0;

        while (nbRead != count)
        {
            int n = src.Read(m_buffer, nbRead + ndxTo, count - nbRead);

            if (n == 0)
                break;

            nbRead += n;
        }

        Count = Math.Max(Count, nbRead + ndxTo);
        return nbRead;
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            ArrayPool.Return(m_buffer);
            m_buffer = Array.Empty<byte>();

            IsDisposed = true;

            DisposablesTracker.Remove(this);
        }
    }

    public IBufferReader GetReader(int offset = 0, ByteOrder endianness = ByteOrder.System)
    {
        require(offset >= 0);
        require(offset <= Count);
        require(Enum.IsDefined(endianness));

        return new Reader(this, Count - offset, offset, endianness);
    }

    public IBufferReader GetReader(int offset, int len, ByteOrder endianness)
    {
        require(len >= 0);
        require(offset >= 0);
        require(Enum.IsDefined(endianness));
        require(len <= Count - offset);

        return new Reader(this, len, offset, endianness);
    }

    public IBufferWriter GetWriter(int offset = 0, ByteOrder endianness = ByteOrder.System)
    {
        require(offset >= 0);
        require(offset <= Count);
        require(Enum.IsDefined(endianness));

        return new Writer(this, Capacity - offset, offset, endianness);
    }

    public IBufferWriter GetWriter(int offset, int maxLen, ByteOrder endianness)
    {
        require(maxLen >= 0);
        require(offset >= 0);
        require(offset <= Count);
        require(maxLen <= Capacity - offset);
        require(Enum.IsDefined(endianness));

        return new Writer(this, maxLen, offset, endianness);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //private:
    byte[] m_buffer;
    UTF8Encoding? m_encoding;

    UTF8Encoding Encoding => m_encoding ??= new UTF8Encoding(false, true);

    void ResizeBuffer(int minSize)
    {
        assert(minSize > m_buffer.Length);
        assert(minSize <= Capacity);

        uint sz = BitOperations.RoundUpToPowerOf2((uint)minSize);

        if (sz <= Capacity)
            minSize = (int)sz;

        byte[] newBuffer = ArrayPool.Rent<byte>(minSize);

        unsafe
        {
            fixed (byte* ptrBuffer = newBuffer)
                CopyTo(ptrBuffer, Count, 0);
        }

        ArrayPool.Return(m_buffer);
        m_buffer = newBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe void CopyFrom(byte* ptrSrc, int count, int ndxTo)
    {
        assert(m_buffer.Length >= count + ndxTo);

        fixed (byte* ptrBuffer = m_buffer)
        {
            byte* ptrDest = ptrBuffer + ndxTo;

            while (count-- > 0)
                *ptrDest++ = *ptrSrc++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe void CopyTo(byte* ptrDest, int count, int ndxFrom)
    {
        assert(m_buffer.Length >= count + ndxFrom);

        fixed (byte* ptrBuffer = m_buffer)
        {
            byte* ptrSrc = ptrBuffer + ndxFrom;

            while (count-- > 0)
                *ptrDest++ = *ptrSrc++;
        }
    }
}

