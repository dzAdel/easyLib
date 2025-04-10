using easyLib.IO;

namespace easyLib.Extras.IO;

partial class Buffer
{
    sealed class Reader : IBufferReader
    {
        public Reader(Buffer owner, int len, int offset, ByteOrder endianness)
        {
            require(owner != null);
            require(len >= 0);
            require(offset >= 0);
            require(Enum.IsDefined(endianness));
            require(len <= owner.Count - offset);

            m_owner = owner;
            Length = len;
            m_off = offset;
            m_pos = 0;
            ByteOrder = endianness.Normalize();
            m_needReorder = BitConverter.IsLittleEndian != (ByteOrder == ByteOrder.LittleEndian);
        }

        public long Position
        {
            get => m_pos;
            set
            {
                require(value >= 0);
                require(value <= Length);

                m_pos = (int)value;
            }
        }

        public long Length { get; }
        public bool IsExhausted => m_pos >= Length;
        public ByteOrder ByteOrder { get; }

        public IEnumerable<byte> ReadBytes()
        {
            while (m_pos < Length)
                yield return m_owner[m_off + m_pos++];
        }

        public byte ReadByte()
        {
            assert(!IsExhausted);
            return m_pos >= Length ? throw new OverflowException() : m_owner[m_off + m_pos++];
        }

        public void ReadBytes(Span<byte> bytes)
        {
            int len = bytes.Length;

            assert(len + m_pos <= Length);
            if (len + m_pos > Length)
                throw new OverflowException();

            m_owner.CopyTo(bytes, m_pos + m_off);
            m_pos += len;
        }

        public sbyte ReadSByte() => (sbyte)ReadByte();

        public void ReadSBytes(Span<sbyte> sbytes)
        {
            int len = sbytes.Length;

            assert(m_pos + len <= Length);
            if (m_pos + len > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (sbyte* ptrDest = sbytes)
                    m_owner.CopyTo((byte*)ptrDest, len, m_off + m_pos);
            }

            m_pos += len;
        }

        public bool ReadBool() => ReadByte() != 0;

        public void ReadBools(Span<bool> bools)
        {
            int len = bools.Length;

            assert(m_pos + len <= Length);
            if (m_pos + len > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (bool* ptrDest = bools)
                    m_owner.CopyTo((byte*)ptrDest, len, m_off + m_pos);
            }

            m_pos += len;
        }

        public short ReadShort() => (short)ReadUShort();

        public void ReadShorts(Span<short> shorts)
        {
            int size = shorts.Length << 1;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (short* ptrDest = shorts)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(short));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_off + m_pos);
                }

                m_pos += size;
            }
        }

        public ushort ReadUShort()
        {
            assert(m_pos + sizeof(ushort) <= Length);
            if (m_pos + sizeof(ushort) > Length)
                throw new OverflowException();

            int ndxFrom = m_off + m_pos;
            ushort result;

            unsafe
            {
                byte* ptrDest = (byte*)&result;

                if (m_needReorder)
                {
                    *ptrDest = m_owner.m_buffer[ndxFrom + 1];
                    *(ptrDest + 1) = m_owner.m_buffer[ndxFrom];
                }
                else
                {
                    *ptrDest = m_owner.m_buffer[ndxFrom];
                    *(ptrDest + 1) = m_owner.m_buffer[ndxFrom + 1];
                }
            }

            m_pos += sizeof(ushort);

            return result;
        }

        public void ReadUShorts(Span<ushort> ushorts)
        {
            int size = ushorts.Length << 1;

            assert(size >= 0 || m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (ushort* ptrDest = ushorts)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(ushort));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public int ReadInt() => (int)ReadUInt();

        public void ReadInts(Span<int> ints)
        {
            int size = ints.Length << 2;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (int* ptrDest = ints)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(int));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public uint ReadUInt()
        {
            assert(m_pos + sizeof(uint) <= Length);
            if (m_pos + sizeof(uint) > Length)
                throw new OverflowException();

            int ndxFrom = m_off + m_pos;
            uint result;

            unsafe
            {
                byte* ptrDest = (byte*)&result;

                if (m_needReorder)
                {
                    *ptrDest = m_owner.m_buffer[ndxFrom + 3];
                    *(ptrDest + 1) = m_owner.m_buffer[ndxFrom + 2];
                    *(ptrDest + 2) = m_owner.m_buffer[ndxFrom + 1];
                    *(ptrDest + 3) = m_owner.m_buffer[ndxFrom];
                }
                else
                {
                    *ptrDest = m_owner.m_buffer[ndxFrom];
                    *(ptrDest + 1) = m_owner.m_buffer[ndxFrom + 1];
                    *(ptrDest + 2) = m_owner.m_buffer[ndxFrom + 2];
                    *(ptrDest + 3) = m_owner.m_buffer[ndxFrom + 3];
                }
            }

            m_pos += sizeof(uint);

            return result;
        }

        public void ReadUInts(Span<uint> uints)
        {
            int size = uints.Length << 2;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (uint* ptrDest = uints)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(uint));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public long ReadLong() => (long)ReadULong();

        public void ReadLongs(Span<long> longs)
        {
            int size = longs.Length << 3;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (long* ptrDest = longs)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(long));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public ulong ReadULong()
        {
            assert(m_pos + sizeof(ulong) <= Length);
            if (m_pos + sizeof(ulong) > Length)
                throw new OverflowException();

            int ndxFrom = m_off + m_pos;
            ulong result;

            unsafe
            {
                byte* ptrDest = (byte*)&result;

                if (m_needReorder)
                {
                    ptrDest += sizeof(ulong) - 1;

                    for (int i = 0; i < sizeof(ulong); ++i)
                        *(ptrDest - i) = m_owner.m_buffer[ndxFrom + i];
                }
                else
                    for (int i = 0; i < sizeof(ulong); ++i)
                        *(ptrDest + i) = m_owner.m_buffer[ndxFrom + i];
            }

            m_pos += sizeof(ulong);

            return result;
        }

        public void ReadULongs(Span<ulong> ulongs)
        {
            int size = ulongs.Length << 3;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (ulong* ptrDest = ulongs)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(ulong));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public float ReadFloat()
        {
            assert(m_pos + sizeof(float) <= Length);
            if (m_pos + sizeof(float) > Length)
                throw new OverflowException();

            int ndxFrom = m_off + m_pos;
            float result;

            unsafe
            {
                byte* ptrDest = (byte*)&result;

                if (m_needReorder)
                {
                    ptrDest += sizeof(float) - 1;

                    for (int i = 0; i < sizeof(float); ++i)
                        *(ptrDest - i) = m_owner.m_buffer[ndxFrom + i];
                }
                else
                    for (int i = 0; i < sizeof(float); ++i)
                        *(ptrDest + i) = m_owner.m_buffer[ndxFrom + i];
            }

            m_pos += sizeof(float);

            return result;
        }

        public void ReadFloats(Span<float> floats)
        {
            int size = floats.Length << 2;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (float* ptrDest = floats)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(float));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public double ReadDouble()
        {
            assert(m_pos + sizeof(double) <= Length);
            if (m_pos + sizeof(double) > Length)
                throw new OverflowException();

            int ndxFrom = m_off + m_pos;
            double result;

            unsafe
            {
                byte* ptrDest = (byte*)&result;

                if (m_needReorder)
                {
                    ptrDest += sizeof(double) - 1;

                    for (int i = 0; i < sizeof(double); ++i)
                        *(ptrDest - i) = m_owner.m_buffer[ndxFrom + i];
                }
                else
                    for (int i = 0; i < sizeof(double); ++i)
                        *(ptrDest + i) = m_owner.m_buffer[ndxFrom + i];
            }

            m_pos += sizeof(double);

            return result;
        }

        public void ReadDoubles(Span<double> doubles)
        {
            int size = doubles.Length << 3;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (double* ptrDest = doubles)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(double));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public decimal ReadDecimal()
        {
            assert(m_pos + sizeof(decimal) <= Length);
            if (m_pos + sizeof(decimal) > Length)
                throw new OverflowException();

            int ndxFrom = m_off + m_pos;
            decimal result;

            unsafe
            {
                byte* ptrDest = (byte*)&result;

                if (m_needReorder)
                {
                    ptrDest += sizeof(decimal) - 1;

                    for (int i = 0; i < sizeof(decimal); ++i)
                        *(ptrDest - i) = m_owner.m_buffer[ndxFrom + i];
                }
                else
                    for (int i = 0; i < sizeof(decimal); ++i)
                        *(ptrDest + i) = m_owner.m_buffer[ndxFrom + i];
            }

            m_pos += sizeof(decimal);

            return result;
        }

        public void ReadDecimals(Span<decimal> decimals)
        {
            int size = decimals.Length << 4;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (decimal* ptrDest = decimals)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(decimal));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public char ReadChar() => (char)ReadUShort();

        public void ReadChars(Span<char> chars)
        {
            int size = chars.Length << 1;

            assert(size >= 0 && m_pos + size <= Length);
            if (size < 0 || m_pos + size > Length)
                throw new OverflowException();

            unsafe
            {
                fixed (char* ptrDest = chars)
                {
                    if (m_needReorder)
                        ReorderReadFromBuffer((byte*)ptrDest, size, sizeof(char));
                    else
                        m_owner.CopyTo((byte*)ptrDest, size, m_pos + m_off);
                }
            }

            m_pos += size;
        }

        public string ReadString()
        {
            int len = MultiByteIntCodec.GetInt(ReadBytes());

            assert(m_pos + len <= Length);
            if (m_pos + len > Length)
                throw new OverflowException();

            string result = m_owner.Encoding.GetString(m_owner.m_buffer, m_off + m_pos, len);
            m_pos += len;

            return result;
        }


        //private:
        readonly Buffer m_owner;
        readonly int m_off;
        int m_pos;
        readonly bool m_needReorder;

        unsafe void ReorderReadFromBuffer(byte* ptrDest, int szDest, int szChunk)
        {
            assert(m_owner.Count >= m_off + m_pos + szDest);

            fixed (byte* ptrBuffer = m_owner.m_buffer)
            {
                byte* ptrChunk = ptrBuffer + m_off + m_pos;

                while (szDest > 0)
                {
                    byte* ptrSrc = ptrChunk + szChunk;
                    int n = 0;

                    while (n++ < szChunk)
                        *ptrDest++ = *--ptrSrc;

                    ptrChunk += szChunk;
                    szDest -= szChunk;
                }
            }
        }
    }
}

