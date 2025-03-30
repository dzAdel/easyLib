using System.Buffers;

namespace easyLib.Test;

public static class SampleFactory
{
    public static byte NextByte
    {
        get
        {
            lock (m_rand)
                return (byte)m_rand.Next(byte.MinValue, byte.MaxValue);
        }
    }
    public static sbyte NextSByte
    {
        get
        {
            lock (m_rand)
                return (sbyte)m_rand.Next(sbyte.MinValue, sbyte.MaxValue);
        }
    }

    public static short NextShort
    {
        get
        {
            lock (m_rand)
                return (short)m_rand.Next(short.MinValue, short.MaxValue);
        }
    }

    public static ushort NextUShort
    {
        get
        {
            lock (m_rand)
                return (ushort)m_rand.Next(ushort.MinValue, ushort.MaxValue);
        }
    }

    public static int NextInt
    {
        get
        {
            lock (m_rand)
                return m_rand.Next(int.MinValue, int.MaxValue);
        }
    }

    public static uint NextUInt
    {
        get
        {
            lock (m_rand)
                return (uint)m_rand.NextInt64(0, uint.MaxValue);
        }
    }

    public static long NextLong
    {
        get
        {
            lock (m_rand)
                return m_rand.NextInt64(long.MinValue, long.MaxValue);
        }
    }

    public static ulong NextULong
    {
        get
        {
            double d;

            lock (m_rand)
                d = m_rand.NextDouble();

            return (ulong)(d * ulong.MaxValue);
        }
    }

    public static float NextFloat
    {
        get
        {
            float f;

            lock (m_rand)
                f = m_rand.NextSingle();

            return float.MinValue - (f * float.MinValue) + (f * float.MaxValue);
        }
    }

    public static double NextDouble
    {
        get
        {
            double d;

            lock (m_rand)
                d = m_rand.NextDouble();

            return double.MinValue - (d * double.MinValue) + (d * double.MaxValue);
        }
    }

    public static decimal NextDecimal
    {
        get
        {
            decimal d;

            lock (m_rand)
                d = (decimal)m_rand.NextDouble();

            return decimal.MinValue - (d * decimal.MinValue) + (d * decimal.MaxValue);
        }
    }

    public static bool NextBool
    {
        get
        {
            lock (m_rand)
                return m_rand.Next(0, 2) == 1;
        }
    }

    public static char NextChar
    {
        get
        {
            lock (m_rand)
                return (char)m_rand.Next(0, '\uD800');
        }
    }

    public static string NextString => CreateStrings().First();

    public static IEnumerable<byte> CreateBytes(byte min = byte.MinValue, byte limit = byte.MaxValue)
    {
        require(min < limit);

        int n;

        while (true)
        {
            lock (m_rand)
                n = m_rand.Next(min, limit);

            yield return (byte)n;
        }
    }

    public static IEnumerable<sbyte> CreateSBytes(sbyte min = sbyte.MinValue, sbyte limit = sbyte.MaxValue)
    {
        require(min < limit);

        int n;

        while (true)
        {
            lock (m_rand)
                n = m_rand.Next(min, limit);

            yield return (sbyte)n;
        }
    }

    public static IEnumerable<short> CreateShorts(short min = short.MinValue, short limit = short.MaxValue)
    {
        require(min < limit);

        int n;

        while (true)
        {
            lock (m_rand)
                n = m_rand.Next(min, limit);

            yield return (short)n;
        }
    }

    public static IEnumerable<ushort> CreateUShorts(ushort min = ushort.MinValue, ushort limit = ushort.MaxValue)
    {
        require(min < limit);

        int n;

        while (true)
        {
            lock (m_rand)
                n = m_rand.Next(min, limit);

            yield return (ushort)n;
        }
    }

    public static IEnumerable<int> CreateInts(int min = int.MinValue, int limit = int.MaxValue)
    {
        require(min < limit);

        int n;

        while (true)
        {
            lock (m_rand)
                n = m_rand.Next(min, limit);

            yield return n;
        }
    }

    public static IEnumerable<uint> CreateUInts(uint min = uint.MinValue, uint limit = uint.MaxValue)
    {
        require(min < limit);

        long l;

        while (true)
        {
            lock (m_rand)
                l = m_rand.NextInt64(min, limit);

            yield return (uint)l;
        }
    }

    public static IEnumerable<long> CreateLongs(long min = long.MinValue, long limit = long.MaxValue)
    {
        require(min < limit);

        long l;

        while (true)
        {
            lock (m_rand)
                l = m_rand.NextInt64(min, limit);

            yield return l;
        }
    }

    public static IEnumerable<ulong> CreateULongs(ulong min = ulong.MinValue, ulong limit = ulong.MaxValue)
    {
        require(min < limit);

        double d;

        while (true)
        {
            lock (m_rand)
                d = m_rand.NextDouble();

            yield return min - (ulong)(d * min) + (ulong)(d * limit);
        }
    }

    public static IEnumerable<float> CreateFloats(float min = float.MinValue, float limit = float.MaxValue)
    {
        require(float.IsFinite(min));
        require(float.IsFinite(limit));
        require(min < limit);

        float f;

        while (true)
        {
            lock (m_rand)
                f = m_rand.NextSingle();

            f = min - (f * min) + (f * limit);
            yield return f >= limit || f < min ? min : f;
        }
    }

    public static IEnumerable<double> CreateDoubles(double min = double.MinValue, double limit = double.MaxValue)
    {
        require(double.IsFinite(min));
        require(double.IsFinite(limit));
        require(min < limit);

        double d;

        while (true)
        {
            lock (m_rand)
                d = m_rand.NextDouble();

            d = min - (d * min) + (d * limit);
            yield return d >= limit || d < min ? min : d;
        }
    }

    public static IEnumerable<decimal> CreateDecimals(decimal min = decimal.MinValue, decimal limit = decimal.MaxValue)
    {
        require(min < limit);

        decimal d;

        while (true)
        {
            lock (m_rand)
                d = (decimal)m_rand.NextDouble();

            d = min - (d * min) + (d * limit);
            yield return d >= limit || d < min ? min : d;
        }
    }

    public static IEnumerable<bool> CreateBools()
    {
        int n;

        while (true)
        {
            lock (m_rand)
                n = m_rand.Next(0, 2);

            yield return n == 1;
        }
    }

    public static IEnumerable<char> CreateChars(char min = char.MinValue, char limit = '\ud800')
    {
        require(min < limit);

        int n;

        while (true)
        {
            lock (m_rand)
                n = m_rand.Next(min, limit);

            yield return (char)n;
        }
    }

    public static IEnumerable<string> CreateStrings(int minLen = 0, int lenLimit = byte.MaxValue)
    {
        require(minLen >= 0);
        require(minLen < lenLimit);

        while (true)
        {
            int len;
            const int charLimit = '\uD800';
            char[] chars;

            lock (m_rand)
            {
                len = m_rand.Next(minLen, lenLimit);
                chars = ArrayPool<char>.Shared.Rent(len);

                for (int i = 0; i < len; ++i)
                    chars[i] = (char)m_rand.Next(0, charLimit);
            }

            string result = new(chars, 0, len);
            ArrayPool<char>.Shared.Return(chars);

            yield return result;
        }
    }

    //private:
    readonly static Random m_rand = new(); //lock
}
