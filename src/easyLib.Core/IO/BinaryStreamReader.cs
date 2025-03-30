using easyLib.IO.Exceptions;
using System.Numerics;
using System.Text;

namespace easyLib.IO;

public class BinaryStreamReader : IBinaryReader, IDestructible
{
    public BinaryStreamReader(Stream srcStream, ByteOrder endianness = ByteOrder.System)
    {
        require(srcStream != null);
        require(srcStream.CanRead);
        require(Enum.IsDefined(endianness));

        InputStream = srcStream;
        ByteOrder = endianness;
        m_buffer = ArrayPool.Rent<byte>(sizeof(decimal));

        DisposablesTracker.Add(this);
    }

    public bool IsDisposed { get; private set; }

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

    public IEnumerable<byte> ReadBytes()
    {
        int n = InputStream.ReadByte();

        while (n >= 0)
        {
            yield return (byte)n;

            n = InputStream.ReadByte();
        }
    }

    public byte ReadByte()
    {
        int n = InputStream.ReadByte();

        return n < 0 ? throw new EndOfStreamException() : (byte)n;
    }

    public void ReadBytes(Span<byte> dest) => ReadFromStream(dest);

    public sbyte ReadSByte() => (sbyte)ReadByte();

    public void ReadSBytes(Span<sbyte> dest) => ReadFromStream(dest);

    public bool ReadBool() => ReadByte() != 0;

    public void ReadBools(Span<bool> dest) => ReadFromStream(dest);

    public char ReadChar()
    {
        if (Load(sizeof(char)) != sizeof(char))
            throw new EndOfStreamException();

        char c;

        unsafe
        {
            byte* ptrDest = (byte*)&c;

            if (m_needReorder)
            {
                ptrDest[0] = m_buffer[1];
                ptrDest[1] = m_buffer[0];
            }
            else
            {
                ptrDest[0] = m_buffer[0];
                ptrDest[1] = m_buffer[1];
            }
        }

        return c;
    }

    public void ReadChars(Span<char> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public short ReadShort() => (short)ReadUShort();

    public void ReadShorts(Span<short> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public ushort ReadUShort()
    {
        if (Load(sizeof(ushort)) != sizeof(ushort))
            throw new EndOfStreamException();

        ushort us;

        unsafe
        {
            byte* ptrDest = (byte*)&us;

            if (m_needReorder)
            {
                ptrDest[0] = m_buffer[1];
                ptrDest[1] = m_buffer[0];
            }
            else
            {
                ptrDest[0] = m_buffer[0];
                ptrDest[1] = m_buffer[1];
            }
        }

        return us;
    }

    public void ReadUShorts(Span<ushort> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public int ReadInt() => (int)ReadUInt();

    public void ReadInts(Span<int> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public uint ReadUInt()
    {
        if (Load(sizeof(uint)) != sizeof(uint))
            throw new EndOfStreamException();

        uint ui;

        unsafe
        {
            byte* ptrDest = (byte*)&ui;

            if (m_needReorder)
            {
                ptrDest += sizeof(uint) - 1;

                for (int i = 0; i < sizeof(uint); ++i)
                    *(ptrDest - i) = m_buffer[i];
            }
            else
                for (int i = 0; i < sizeof(uint); ++i)
                    *(ptrDest + i) = m_buffer[i];
        }

        return ui;
    }

    public void ReadUInts(Span<uint> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public long ReadLong() => (long)ReadULong();

    public void ReadLongs(Span<long> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public ulong ReadULong()
    {
        if (Load(sizeof(ulong)) != sizeof(ulong))
            throw new EndOfStreamException();

        ulong ul;

        unsafe
        {
            byte* ptrDest = (byte*)&ul;

            if (m_needReorder)
            {
                ptrDest += sizeof(ulong) - 1;

                for (int i = 0; i < sizeof(ulong); ++i)
                    *(ptrDest - i) = m_buffer[i];
            }
            else
                for (int i = 0; i < sizeof(ulong); ++i)
                    *(ptrDest + i) = m_buffer[i];
        }

        return ul;
    }

    public void ReadULongs(Span<ulong> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public float ReadFloat()
    {
        if (Load(sizeof(float)) != sizeof(float))
            throw new EndOfStreamException();

        float f;

        unsafe
        {
            byte* ptrDest = (byte*)&f;

            if (m_needReorder)
            {
                ptrDest += sizeof(float) - 1;

                for (int i = 0; i < sizeof(float); ++i)
                    *(ptrDest - i) = m_buffer[i];
            }
            else
                for (int i = 0; i < sizeof(float); ++i)
                    *(ptrDest + i) = m_buffer[i];
        }

        return f;
    }

    public void ReadFloats(Span<float> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public double ReadDouble()
    {
        if (Load(sizeof(double)) != sizeof(double))
            throw new EndOfStreamException();

        double d;

        unsafe
        {
            byte* ptrDest = (byte*)&d;

            if (m_needReorder)
            {
                ptrDest += sizeof(double) - 1;

                for (int i = 0; i < sizeof(double); ++i)
                    *(ptrDest - i) = m_buffer[i];
            }
            else
                for (int i = 0; i < sizeof(double); ++i)
                    *(ptrDest + i) = m_buffer[i];
        }

        return d;
    }

    public void ReadDoubles(Span<double> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public decimal ReadDecimal()
    {
        if (Load(sizeof(decimal)) != sizeof(decimal))
            throw new EndOfStreamException();

        decimal d;

        unsafe
        {
            byte* ptrDest = (byte*)&d;

            if (m_needReorder)
            {
                ptrDest += sizeof(decimal) - 1;

                for (int i = 0; i < sizeof(decimal); ++i)
                    *(ptrDest - i) = m_buffer[i];
            }
            else
                for (int i = 0; i < sizeof(decimal); ++i)
                    *(ptrDest + i) = m_buffer[i];
        }

        return d;
    }

    public void ReadDecimals(Span<decimal> dest)
    {
        if (m_needReorder)
            ReorderReadFromStream(dest);
        else
            ReadFromStream(dest);
    }

    public string ReadString()
    {
        int sz = MultiByteIntCodec.GetInt(ReadBytes());

        if (sz < 0 || sz > Array.MaxLength)
            throw new CorruptedStreamException();

        if (sz == 0)
            return "";


        if (sz > MAX_BUFFER_SIZE)
        {
            byte[] bytes = ArrayPool.Rent<byte>(sz);

            try
            {
                load(bytes, sz);

                return Decoder.GetString(bytes, 0, sz);
            }
            catch (ArgumentException ex)
            {
                throw new CorruptedStreamException(ex);
            }
            finally
            {
                ArrayPool.Return(bytes);
            }
        }


        if (m_buffer.Length < sz)
            ResizeBuffer(sz);

        try
        {
            load(m_buffer, sz);

            return Decoder.GetString(m_buffer, 0, sz);
        }
        catch (ArgumentException ex)
        {
            throw new CorruptedStreamException(ex);
        }

        //local:
        void load(byte[] dest, int size)
        {
            int nbRead = 0;

            while (nbRead != size)
            {
                int n = InputStream.Read(dest, nbRead, size - nbRead);

                if (n == 0)
                    break;

                nbRead += n;
            }

            if (nbRead != size)
                throw new CorruptedStreamException();
        }
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
    protected Stream InputStream { get; }
    protected UTF8Encoding Decoder => m_encoding ??= new UTF8Encoding(false, true);
    protected virtual void DoDispose() => ArrayPool.Return(m_buffer);

    //private:
    const int MAX_BUFFER_SIZE = ushort.MaxValue;

    byte[] m_buffer;
    UTF8Encoding? m_encoding;
    ByteOrder m_endianness;
    bool m_needReorder;

    void ResizeBuffer(int size)
    {
        int len = m_buffer.Length;

        assert(len < size);

        if (len < MAX_BUFFER_SIZE)
        {
            size = size < MAX_BUFFER_SIZE ? (int)BitOperations.RoundUpToPowerOf2((uint)size) : MAX_BUFFER_SIZE;
            assert(size <= MAX_BUFFER_SIZE);

            byte[] newBuffer = ArrayPool.Rent<byte>(size);
            ArrayPool.Return(m_buffer);
            m_buffer = newBuffer;
        }
    }

    int Load(int count)
    {
        assert(m_buffer.Length >= count);

        int nbRead = 0;
        while (nbRead != count)
        {
            int n = InputStream.Read(m_buffer, nbRead, count - nbRead);

            if (n == 0)
                break;

            nbRead += n;
        }

        return nbRead;
    }

    unsafe void ReadFromStream<T>(Span<T> dest) where T : unmanaged
    {
        long size = (long)dest.Length * sizeof(T);

        if (m_buffer.Length < size)
            ResizeBuffer(size > MAX_BUFFER_SIZE ? MAX_BUFFER_SIZE : (int)size);

        long count = 0;

        while (count < size)
        {
            int nbToRead = (int)Math.Min(m_buffer.Length, size - count);

            if (Load(nbToRead) != nbToRead)
                throw new EndOfStreamException();

            fixed (T* ptrSpan = dest)
            fixed (byte* ptrBuffer = m_buffer)
            {
                byte* ptrDest = (byte*)ptrSpan + count;
                byte* ptrSrc = ptrBuffer;
                int sz = 0;

                while (sz++ < nbToRead)
                    *ptrDest++ = *ptrSrc++;
            }

            count += nbToRead;
        }
    }

    unsafe void ReorderReadFromStream<T>(Span<T> dest) where T : unmanaged
    {
        int szItem = sizeof(T);
        long size = (long)dest.Length * szItem;

        if (m_buffer.Length < size)
            ResizeBuffer(size > MAX_BUFFER_SIZE ? MAX_BUFFER_SIZE : (int)size);

        assert(m_buffer.Length % szItem == 0);

        long count = 0;

        while (count < size)
        {
            int nbToRead = (int)Math.Min(m_buffer.Length, size - count);

            if (Load(nbToRead) != nbToRead)
                throw new EndOfStreamException();

            fixed (T* ptrSpan = dest)
            fixed (byte* ptrBuffer = m_buffer)
            {
                byte* ptrDest = (byte*)ptrSpan + count;
                byte* ptrSrc = ptrBuffer;

                int n = 0;

                while (n < nbToRead)
                {
                    ptrDest += szItem;
                    int sz = 0;

                    while (sz++ < szItem)
                        *--ptrDest = *ptrSrc++;

                    ptrDest += szItem;
                    n += szItem;
                }
            }

            count += nbToRead;
        }
    }
}

