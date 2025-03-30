namespace easyLib.IO;

public static class MultiByteIntCodec
{
    public const int MaxByteCount = 9;

    public static int GetByteCount(short s) => GetByteCount((ulong)(ushort)s);
    public static int GetByteCount(ushort us) => GetByteCount((ulong)us);
    public static int GetByteCount(int n) => GetByteCount((ulong)(uint)n);
    public static int GetByteCount(uint u) => GetByteCount((ulong)u);
    public static int GetByteCount(long l) => GetByteCount((ulong)l);

    public static int GetByteCount(ulong ul)
    {
        for (int i = 0; i < MaxByteCount; ++i)
            if ((ul >>= 7) == 0)
                return i + 1;

        return MaxByteCount;
    }

    public static short GetShort(IEnumerable<byte> bytes)
    {
        require(bytes != null);

        return (short)Decode(bytes, sizeof(short));
    }

    public static int GetShort(ReadOnlySpan<byte> bytes, out short result)
    {
        require(!bytes.IsEmpty);

        (ulong ul, int count) = Decode(bytes, sizeof(short));
        result = (short)ul;

        return count;
    }

    public static ushort GetUShort(IEnumerable<byte> bytes)
    {
        require(bytes != null);

        return (ushort)Decode(bytes, sizeof(ushort));
    }

    public static int GetUShort(ReadOnlySpan<byte> bytes, out ushort result)
    {
        require(!bytes.IsEmpty);

        (ulong ul, int count) = Decode(bytes, sizeof(ushort));
        result = (ushort)ul;

        return count;
    }

    public static int GetInt(IEnumerable<byte> bytes)
    {
        require(bytes != null);

        return (int)Decode(bytes, sizeof(int));
    }

    public static int GetInt(ReadOnlySpan<byte> bytes, out int result)
    {
        require(!bytes.IsEmpty);

        (ulong ul, int count) = Decode(bytes, sizeof(int));
        result = (int)ul;

        return count;
    }

    public static uint GetUInt(IEnumerable<byte> bytes)
    {
        require(bytes != null);

        return (uint)Decode(bytes, sizeof(uint));
    }

    public static int GetUInt(ReadOnlySpan<byte> bytes, out uint result)
    {
        require(!bytes.IsEmpty);

        (ulong ul, int count) = Decode(bytes, sizeof(uint));
        result = (uint)ul;

        return count;
    }

    public static long GetLong(IEnumerable<byte> bytes)
    {
        require(bytes != null);

        return (long)Decode(bytes, sizeof(long));
    }

    public static int GetLong(ReadOnlySpan<byte> bytes, out long result)
    {
        require(!bytes.IsEmpty);

        (ulong ul, int count) = Decode(bytes, sizeof(long));
        result = (long)ul;

        return count;
    }

    public static ulong GetULong(IEnumerable<byte> bytes)
    {
        require(bytes != null);

        return Decode(bytes, sizeof(ulong));
    }

    public static int GetULong(ReadOnlySpan<byte> bytes, out ulong result)
    {
        require(!bytes.IsEmpty);

        (result, int count) = Decode(bytes, sizeof(ulong));

        return count;
    }

    public static byte[] GetBytes(short s) => GetBytes((ulong)(ushort)s);

    public static int GetBytes(short s, Span<byte> bytes)
    {
        require(GetByteCount(s) <= bytes.Length);

        return GetBytes((ulong)(ushort)s, bytes);

    }

    public static byte[] GetBytes(ushort us) => GetBytes((ulong)us);

    public static int GetBytes(ushort us, Span<byte> bytes)
    {
        require(GetByteCount(us) <= bytes.Length);

        return GetBytes((ulong)us, bytes);
    }

    public static byte[] GetBytes(int n) => GetBytes((ulong)(uint)n);

    public static int GetBytes(int n, Span<byte> bytes)
    {
        require(GetByteCount(n) <= bytes.Length);

        return GetBytes((ulong)(uint)n, bytes);
    }

    public static byte[] GetBytes(uint u) => GetBytes((ulong)u);

    public static int GetBytes(uint ui, Span<byte> bytes)
    {
        require(GetByteCount(ui) <= bytes.Length);

        return GetBytes((ulong)ui, bytes);
    }

    public static byte[] GetBytes(long l) => GetBytes((ulong)l);

    public static int GetBytes(long l, Span<byte> bytes)
    {
        require(GetByteCount(l) <= bytes.Length);

        return GetBytes((ulong)l, bytes);
    }

    public static byte[] GetBytes(ulong ul)
    {
        byte[] buffer = new byte[GetByteCount(ul)];
        GetBytes(ul, buffer);

        return buffer;
    }

    public static int GetBytes(ulong ul, Span<byte> bytes)
    {
        require(GetByteCount(ul) <= bytes.Length);

        for (int i = 0; i < MaxByteCount - 1; ++i)
        {
            byte b = (byte)ul;

            if ((ul >>= 7) == 0)
            {
                bytes[i] = b;
                return i + 1;
            }

            bytes[i] = b |= 0b10000000;
        }

        bytes[MaxByteCount - 1] = (byte)ul;

        return MaxByteCount;
    }


    //private:
    static ulong Decode(IEnumerable<byte> bytes, int szItem)
    {
        int msbMaxValue = (1 << szItem) - 1;
        int ndxByte = 0;
        ulong result = 0;

        foreach (byte b in bytes)
        {
            if (ndxByte == szItem)
                if (b > msbMaxValue)
                    break;
                else
                    return result | ((ulong)b << (szItem * 7));


            result |= (ulong)(b & 0b01111111) << (ndxByte * 7);

            if ((b & 0b10000000) == 0)
                return result;

            ++ndxByte;
        }

        throw new OverflowException();
    }

    static (ulong result, int count) Decode(ReadOnlySpan<byte> bytes, int szItem)
    {
        ulong result = 0;
        byte b;

        for (int i = 0; i < szItem; ++i)
        {
            b = bytes[i];
            result |= (ulong)(b & 0b01111111) << (i * 7);

            if ((b & 0b10000000) == 0)
                return (result, i + 1);
        }

        return (b = bytes[szItem]) > (1 << szItem) - 1 ? throw new OverflowException() :
            (result | ((ulong)b << (szItem * 7)), szItem + 1);
    }
}