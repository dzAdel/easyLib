using easyLib.IO;

namespace easyLib.IO;

partial class Buffer
{
    sealed class Writer : IBufferWriter
    {
        public Writer(Buffer owner, int maxLen, int offset, ByteOrder endianness)
        {
            require(owner != null);
            require(maxLen >= 0);
            require(offset >= 0);
            require(offset <= owner.Count);
            require(maxLen <= owner.Capacity - offset);

            m_owner = owner;
            m_maxLen = maxLen;
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

        public long Length => Math.Min(m_maxLen, m_owner.Count - m_off);
        public ByteOrder ByteOrder { get; }
        public int Capacity => m_maxLen;

        public void WriteByte(byte b)
        {
            assert(m_pos < m_maxLen);
            if (m_pos >= m_maxLen)
                throw new OverflowException();

            m_owner[m_off + m_pos++] = b;
        }

        public void WriteBytes(ReadOnlySpan<byte> bytes)
        {
            int len = bytes.Length;

            assert(len + m_pos <= m_maxLen);
            if (len + m_pos > m_maxLen)
                throw new OverflowException();

            m_owner.CopyFrom(bytes, m_off + m_pos);
            m_pos += len;
        }

        public void WriteSByte(sbyte sb) => WriteByte((byte)sb);

        public void WriteSBytes(ReadOnlySpan<sbyte> sbytes)
        {
            int len = sbytes.Length;

            assert(m_pos + len <= m_maxLen);
            if (m_pos + len > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + len)
                m_owner.ResizeBuffer(ndxTo + len);

            unsafe
            {
                fixed (sbyte* ptrSrc = sbytes)
                    m_owner.CopyFrom((byte*)ptrSrc, len, ndxTo);
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + len);
            m_pos += len;
        }

        public void WriteBool(bool b) => WriteByte(b ? (byte)1 : (byte)0);

        public void WriteBools(ReadOnlySpan<bool> bools)
        {
            int len = bools.Length;

            assert(m_pos + len <= m_maxLen);
            if (m_pos + len > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + len)
                m_owner.ResizeBuffer(ndxTo + len);

            unsafe
            {
                fixed (bool* ptrSrc = bools)
                    m_owner.CopyFrom((byte*)ptrSrc, len, ndxTo);
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + len);
            m_pos += len;
        }

        public void WriteShort(short s) => WriteUShort((ushort)s);

        public void WriteShorts(ReadOnlySpan<short> shorts)
        {
            int size = shorts.Length << 1;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (short* ptrSrc = shorts)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(short));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteUShort(ushort us)
        {
            assert(m_pos + sizeof(ushort) <= m_maxLen);
            if (m_pos + sizeof(ushort) > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + sizeof(ushort))
                m_owner.ResizeBuffer(ndxTo + sizeof(ushort));

            unsafe
            {
                byte* ptrSrc = (byte*)&us;

                if (m_needReorder)
                {
                    m_owner.m_buffer[ndxTo + 1] = *ptrSrc;
                    m_owner.m_buffer[ndxTo] = *(ptrSrc + 1);
                }
                else
                {
                    m_owner.m_buffer[ndxTo] = *ptrSrc;
                    m_owner.m_buffer[ndxTo + 1] = *(ptrSrc + 1);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + sizeof(ushort));
            m_pos += sizeof(ushort);
        }

        public void WriteUShorts(ReadOnlySpan<ushort> ushorts)
        {
            int size = ushorts.Length << 1;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (ushort* ptrSrc = ushorts)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(ushort));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteInt(int i) => WriteUInt((uint)i);

        public void WriteInts(ReadOnlySpan<int> ints)
        {
            int size = ints.Length << 2;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (int* ptrSrc = ints)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(int));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteUInt(uint ui)
        {
            assert(m_pos + sizeof(uint) <= m_maxLen);
            if (m_pos + sizeof(uint) > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + sizeof(uint))
                m_owner.ResizeBuffer(ndxTo + sizeof(uint));

            unsafe
            {
                byte* ptrSrc = (byte*)&ui;

                if (m_needReorder)
                {
                    m_owner.m_buffer[ndxTo + 3] = *ptrSrc;
                    m_owner.m_buffer[ndxTo + 2] = *(ptrSrc + 1);
                    m_owner.m_buffer[ndxTo + 1] = *(ptrSrc + 2);
                    m_owner.m_buffer[ndxTo] = *(ptrSrc + 3);
                }
                else
                {
                    m_owner.m_buffer[ndxTo] = *ptrSrc;
                    m_owner.m_buffer[ndxTo + 1] = *(ptrSrc + 1);
                    m_owner.m_buffer[ndxTo + 2] = *(ptrSrc + 2);
                    m_owner.m_buffer[ndxTo + 3] = *(ptrSrc + 3);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + sizeof(uint));
            m_pos += sizeof(uint);
        }

        public void WriteUInts(ReadOnlySpan<uint> uints)
        {
            int size = uints.Length << 2;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (uint* ptrSrc = uints)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(uint));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteLong(long l) => WriteULong((ulong)l);

        public void WriteLongs(ReadOnlySpan<long> longs)
        {
            int size = longs.Length << 3;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (long* ptrSrc = longs)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(long));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteULong(ulong ul)
        {
            assert(m_pos + sizeof(ulong) <= m_maxLen);
            if (m_pos + sizeof(ulong) > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + sizeof(ulong))
                m_owner.ResizeBuffer(ndxTo + sizeof(ulong));

            unsafe
            {
                byte* ptrSrc = (byte*)&ul;

                if (m_needReorder)
                    for (int i = sizeof(ulong) - 1; i >= 0; --i)
                        m_owner.m_buffer[ndxTo + i] = *ptrSrc++;
                else
                    for (int i = 0; i < sizeof(ulong); ++i)
                        m_owner.m_buffer[ndxTo + i] = *(ptrSrc + i);
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + sizeof(ulong));
            m_pos += sizeof(ulong);
        }

        public void WriteULongs(ReadOnlySpan<ulong> ulongs)
        {
            int size = ulongs.Length << 3;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (ulong* ptrSrc = ulongs)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(ulong));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteFloat(float f)
        {
            assert(m_pos + sizeof(float) <= m_maxLen);
            if (m_pos + sizeof(float) > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + sizeof(float))
                m_owner.ResizeBuffer(ndxTo + sizeof(float));

            unsafe
            {
                byte* ptrSrc = (byte*)&f;

                if (m_needReorder)
                    for (int i = sizeof(float) - 1; i >= 0; --i)
                        m_owner.m_buffer[ndxTo + i] = *ptrSrc++;
                else
                    for (int i = 0; i < sizeof(float); ++i)
                        m_owner.m_buffer[ndxTo + i] = *(ptrSrc + i);
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + sizeof(float));
            m_pos += sizeof(float);
        }

        public void WriteFloats(ReadOnlySpan<float> floats)
        {
            int size = floats.Length << 2;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (float* ptrSrc = floats)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(float));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteDouble(double d)
        {
            assert(m_pos + sizeof(double) <= m_maxLen);
            if (m_pos + sizeof(double) > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + sizeof(double))
                m_owner.ResizeBuffer(ndxTo + sizeof(double));

            unsafe
            {
                byte* ptrSrc = (byte*)&d;

                if (m_needReorder)
                    for (int i = sizeof(double) - 1; i >= 0; --i)
                        m_owner.m_buffer[ndxTo + i] = *ptrSrc++;
                else
                    for (int i = 0; i < sizeof(double); ++i)
                        m_owner.m_buffer[ndxTo + i] = *(ptrSrc + i);
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + sizeof(double));
            m_pos += sizeof(double);
        }


        public void WriteDoubles(ReadOnlySpan<double> doubles)
        {
            int size = doubles.Length << 3;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (double* ptrSrc = doubles)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(double));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteDecimal(decimal d)
        {
            assert(m_pos + sizeof(decimal) <= m_maxLen);
            if (m_pos + sizeof(decimal) > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + sizeof(decimal))
                m_owner.ResizeBuffer(ndxTo + sizeof(decimal));

            unsafe
            {
                byte* ptrSrc = (byte*)&d;

                if (m_needReorder)
                    for (int i = sizeof(decimal) - 1; i >= 0; --i)
                        m_owner.m_buffer[ndxTo + i] = *ptrSrc++;
                else
                    for (int i = 0; i < sizeof(decimal); ++i)
                        m_owner.m_buffer[ndxTo + i] = *(ptrSrc + i);
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + sizeof(decimal));
            m_pos += sizeof(decimal);
        }

        public void WriteDecimals(ReadOnlySpan<decimal> decimals)
        {
            int size = decimals.Length << 4;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (decimal* ptrSrc = decimals)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(decimal));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteChar(char c) => WriteUShort(c);

        public void WriteChars(ReadOnlySpan<char> chars)
        {
            int size = chars.Length << 1;
            int ndxTo = m_off + m_pos;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            unsafe
            {
                fixed (char* ptrSrc = chars)
                {
                    if (m_needReorder)
                        ReorderCopyToBuffer((byte*)ptrSrc, size, sizeof(char));
                    else
                        m_owner.CopyFrom((byte*)ptrSrc, size, ndxTo);
                }
            }

            m_owner.Count = Math.Max(m_owner.Count, ndxTo + size);
            m_pos += size;
        }

        public void WriteString(string str)
        {
            require(str != null);

            Span<byte> span = stackalloc byte[MultiByteIntCodec.MaxByteCount];
            byte[] bytes = m_owner.Encoding.GetBytes(str);
            int szSpan = MultiByteIntCodec.GetBytes(bytes.Length, span);
            int size = bytes.Length + szSpan;

            assert(m_pos + size <= m_maxLen);
            if (m_pos + size > m_maxLen)
                throw new OverflowException();

            int ndxTo = m_off + m_pos;

            if (m_owner.m_buffer.Length < ndxTo + size)
                m_owner.ResizeBuffer(ndxTo + size);

            m_owner.CopyFrom(span[..szSpan], ndxTo);
            m_owner.CopyFrom(bytes, ndxTo + szSpan);
        }

        //private:
        readonly Buffer m_owner;
        readonly int m_maxLen;
        readonly int m_off;
        int m_pos;
        readonly bool m_needReorder;

        unsafe void ReorderCopyToBuffer(byte* ptrSrc, int szSrc, int szChunk)
        {
            assert(m_owner.m_buffer.Length >= m_off + m_pos + szSrc);

            fixed (byte* ptrBuffer = m_owner.m_buffer)
            {
                byte* ptrChunk = ptrBuffer + m_off + m_pos;

                while (szSrc > 0)
                {
                    byte* ptrDest = ptrChunk + szChunk;
                    int n = 0;

                    while (n++ < szChunk)
                        *--ptrDest = *ptrSrc++;

                    ptrChunk += szChunk;
                    szSrc -= szChunk;
                }
            }
        }
    }
}

