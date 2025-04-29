using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace easyLib.IO;

public class BinaryStreamWriter : IBinaryWriter, IDestructible
{
    public BinaryStreamWriter(Stream destStream, ByteOrder endianness = ByteOrder.System)
    {
        require(destStream != null);
        require(destStream.CanWrite);
        require(Enum.IsDefined(endianness));

        OutputStream = destStream;
        ByteOrder = endianness;
        m_buffer = ArrayPool.Alloc<byte>(sizeof(decimal));

        DisposablesTracker.Add(this);
    }

    public ByteOrder ByteOrder
    {
        get => m_endianness;

        set
        {
            require(Enum.IsDefined(value));

            m_endianness = value.Normalize();
            m_needReorder = BitConverter.IsLittleEndian != (m_endianness == ByteOrder.LittleEndian);
        }
    }

    public bool IsDisposed { get; private set; }

    public void WriteByte(byte b) => OutputStream.WriteByte(b);
    public void WriteBytes(ReadOnlySpan<byte> bytes) => WriteToStream(bytes);
    public void WriteSByte(sbyte sb) => OutputStream.WriteByte((byte)sb);
    public void WriteSBytes(ReadOnlySpan<sbyte> sbytes) => WriteToStream(sbytes);
    public void WriteBool(bool b) => OutputStream.WriteByte((byte)(b ? 1 : 0));
    public void WriteBools(ReadOnlySpan<bool> bools) => WriteToStream(bools);

    public void WriteChar(char c)
    {
        assert(m_buffer.Length >= sizeof(char));

        unsafe
        {
            byte* ptrSrc = (byte*)&c;

            if (m_needReorder)
            {
                m_buffer[0] = ptrSrc[1];
                m_buffer[1] = ptrSrc[0];
            }
            else
            {
                m_buffer[0] = ptrSrc[0];
                m_buffer[1] = ptrSrc[1];
            }
        }

        OutputStream.Write(m_buffer, 0, sizeof(char));
    }

    public void WriteChars(ReadOnlySpan<char> chars)
    {
        if (m_needReorder)
            ReorderWriteToStream(chars);
        else
            WriteToStream(chars);
    }

    public void WriteShort(short s) => WriteUShort((ushort)s);

    public void WriteShorts(ReadOnlySpan<short> shorts)
    {
        if (m_needReorder)
            ReorderWriteToStream(shorts);
        else
            WriteToStream(shorts);
    }

    public void WriteUShort(ushort us)
    {
        assert(m_buffer.Length >= sizeof(ushort));

        unsafe
        {
            byte* ptrSrc = (byte*)&us;

            if (m_needReorder)
            {
                m_buffer[0] = ptrSrc[1];
                m_buffer[1] = ptrSrc[0];
            }
            else
            {
                m_buffer[0] = ptrSrc[0];
                m_buffer[1] = ptrSrc[1];
            }
        }

        OutputStream.Write(m_buffer, 0, sizeof(ushort));
    }

    public void WriteUShorts(ReadOnlySpan<ushort> ushorts)
    {
        if (m_needReorder)
            ReorderWriteToStream(ushorts);
        else
            WriteToStream(ushorts);
    }

    public void WriteInt(int i) => WriteUInt((uint)i);

    public void WriteInts(ReadOnlySpan<int> ints)
    {
        if (m_needReorder)
            ReorderWriteToStream(ints);
        else
            WriteToStream(ints);
    }

    public void WriteUInt(uint ui)
    {
        assert(m_buffer.Length >= sizeof(uint));

        unsafe
        {
            byte* ptrSrc = (byte*)&ui;

            if (m_needReorder)
            {
                m_buffer[0] = ptrSrc[3];
                m_buffer[1] = ptrSrc[2];
                m_buffer[2] = ptrSrc[1];
                m_buffer[3] = ptrSrc[0];
            }
            else
            {
                m_buffer[0] = ptrSrc[0];
                m_buffer[1] = ptrSrc[1];
                m_buffer[2] = ptrSrc[2];
                m_buffer[3] = ptrSrc[3];
            }
        }

        OutputStream.Write(m_buffer, 0, sizeof(uint));
    }

    public void WriteUInts(ReadOnlySpan<uint> uints)
    {
        if (m_needReorder)
            ReorderWriteToStream(uints);
        else
            WriteToStream(uints);
    }

    public void WriteLong(long l) => WriteULong((ulong)l);

    public void WriteLongs(ReadOnlySpan<long> longs)
    {
        if (m_needReorder)
            ReorderWriteToStream(longs);
        else
            WriteToStream(longs);
    }

    public void WriteULong(ulong ul)
    {
        assert(m_buffer.Length >= sizeof(ulong));

        unsafe
        {
            byte* ptrSrc = (byte*)&ul;

            if (m_needReorder)
                for (int i = sizeof(ulong) - 1; i >= 0; --i)
                    m_buffer[i] = *ptrSrc++;
            else
                for (int i = 0; i < sizeof(ulong); ++i)
                    m_buffer[i] = *(ptrSrc + i);
        }

        OutputStream.Write(m_buffer, 0, sizeof(ulong));
    }

    public void WriteULongs(ReadOnlySpan<ulong> ulongs)
    {
        if (m_needReorder)
            ReorderWriteToStream(ulongs);
        else
            WriteToStream(ulongs);
    }

    public void WriteFloat(float f)
    {
        assert(m_buffer.Length >= sizeof(float));

        unsafe
        {
            byte* ptrSrc = (byte*)&f;

            if (m_needReorder)
                for (int i = sizeof(float) - 1; i >= 0; --i)
                    m_buffer[i] = *ptrSrc++;
            else
                for (int i = 0; i < sizeof(float); ++i)
                    m_buffer[i] = *(ptrSrc + i);
        }

        OutputStream.Write(m_buffer, 0, sizeof(float));
    }

    public void WriteFloats(ReadOnlySpan<float> floats)
    {
        if (m_needReorder)
            ReorderWriteToStream(floats);
        else
            WriteToStream(floats);
    }

    public void WriteDouble(double d)
    {
        assert(m_buffer.Length >= sizeof(double));

        unsafe
        {
            byte* ptrSrc = (byte*)&d;

            if (m_needReorder)
                for (int i = sizeof(double) - 1; i >= 0; --i)
                    m_buffer[i] = *ptrSrc++;
            else
                for (int i = 0; i < sizeof(double); ++i)
                    m_buffer[i] = *(ptrSrc + i);
        }

        OutputStream.Write(m_buffer, 0, sizeof(double));
    }

    public void WriteDoubles(ReadOnlySpan<double> doubles)
    {
        if (m_needReorder)
            ReorderWriteToStream(doubles);
        else
            WriteToStream(doubles);
    }

    public void WriteDecimal(decimal d)
    {
        assert(m_buffer.Length >= sizeof(decimal));

        unsafe
        {
            byte* ptrSrc = (byte*)&d;

            if (m_needReorder)
                for (int i = sizeof(decimal) - 1; i >= 0; --i)
                    m_buffer[i] = *ptrSrc++;
            else
                for (int i = 0; i < sizeof(decimal); ++i)
                    m_buffer[i] = *(ptrSrc + i);
        }

        OutputStream.Write(m_buffer, 0, sizeof(decimal));
    }

    public void WriteDecimals(ReadOnlySpan<decimal> decimals)
    {
        if (m_needReorder)
            ReorderWriteToStream(decimals);
        else
            WriteToStream(decimals);
    }

    public void WriteString(string str)
    {
        require(str != null);

        byte[] bytes = Encoder.GetBytes(str);

        assert(m_buffer.Length >= MultiByteIntCodec.MaxByteCount);
        int sz = MultiByteIntCodec.GetBytes(bytes.Length, m_buffer);

        OutputStream.Write(m_buffer, 0, sz);
        OutputStream.Write(bytes, 0, bytes.Length);
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            DoDispose();
            m_buffer = Array.Empty<byte>();

            IsDisposed = true;

            GC.SuppressFinalize(this);

            DisposablesTracker.Remove(this);
        }
    }


    //protected:
    protected Stream OutputStream { get; }
    protected UTF8Encoding Encoder => m_encoding ??= new UTF8Encoding(false, true);
    protected virtual void DoDispose() => ArrayPool.Free(m_buffer);

    //private:
    const int MAX_BUFFER_SIZE = ushort.MaxValue;

    byte[] m_buffer;
    UTF8Encoding? m_encoding;
    ByteOrder m_endianness;
    bool m_needReorder;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ResizeBuffer(int size)
    {
        int len = m_buffer.Length;

        assert(len < size);

        if (len < MAX_BUFFER_SIZE)
        {
            size = size < MAX_BUFFER_SIZE ? (int)BitOperations.RoundUpToPowerOf2((uint)size) : MAX_BUFFER_SIZE;
            assert(size <= MAX_BUFFER_SIZE);

            byte[] newBuffer = ArrayPool.Alloc<byte>(size);
            ArrayPool.Free(m_buffer);
            m_buffer = newBuffer;
        }
    }

    unsafe void ReorderWriteToStream<T>(ReadOnlySpan<T> src) where T : unmanaged
    {
        int szItem = sizeof(T);
        long size = (long)src.Length * szItem;

        if (m_buffer.Length < size)
            ResizeBuffer(size > MAX_BUFFER_SIZE ? MAX_BUFFER_SIZE : (int)size);

        assert(m_buffer.Length % szItem == 0);

        long count = 0;

        while (count < size)
        {
            int nbToWrite = (int)Math.Min(m_buffer.Length, size - count);

            fixed (T* ptrItems = src)
            fixed (byte* ptrBuffer = m_buffer)
            {
                byte* ptrSrc = (byte*)ptrItems + count;
                byte* ptrDest = ptrBuffer;

                int n = 0;

                while (n < nbToWrite)
                {
                    ptrDest += szItem;
                    int sz = 0;

                    while (sz++ < szItem)
                        *--ptrDest = *ptrSrc++;

                    ptrDest += szItem;
                    n += szItem;
                }
            }

            OutputStream.Write(m_buffer, 0, nbToWrite);
            count += nbToWrite;
        }
    }

    unsafe void WriteToStream<T>(ReadOnlySpan<T> src) where T : unmanaged
    {
        long size = (long)src.Length * sizeof(T);

        if (m_buffer.Length < size)
            ResizeBuffer(size > MAX_BUFFER_SIZE ? MAX_BUFFER_SIZE : (int)size);

        long count = 0;

        while (count < size)
        {
            int nbToWrite = (int)Math.Min(m_buffer.Length, size - count);

            fixed (T* ptrItems = src)
            fixed (byte* ptrBuffer = m_buffer)
            {
                byte* ptrSrc = (byte*)ptrItems + count;
                byte* ptrDest = ptrBuffer;
                int sz = 0;

                while (sz++ < nbToWrite)
                    *ptrDest++ = *ptrSrc++;
            }

            OutputStream.Write(m_buffer, 0, nbToWrite);
            count += nbToWrite;
        }
    }
}

