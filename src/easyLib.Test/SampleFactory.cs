using System.Buffers;

namespace easyLib.Test;

public static class SampleFactory
{
    public static byte NextByte => (byte)Random.Shared.Next(byte.MinValue, byte.MaxValue);
    public static sbyte NextSByte => (sbyte)Random.Shared.Next(sbyte.MinValue, sbyte.MaxValue);
    public static short NextShort => (short)Random.Shared.Next(short.MinValue, short.MaxValue);
    public static ushort NextUShort => (ushort)Random.Shared.Next(ushort.MinValue, ushort.MaxValue);
    public static int NextInt => Random.Shared.Next(int.MinValue, int.MaxValue);
    public static uint NextUInt => (uint)Random.Shared.NextInt64(0, uint.MaxValue);
    public static long NextLong => Random.Shared.NextInt64(long.MinValue, long.MaxValue);
    public static ulong NextULong => (ulong)(Random.Shared.NextDouble() * ulong.MaxValue);
    public static bool NextBool => Random.Shared.Next(0, 2) == 1;
    public static char NextChar => (char)Random.Shared.Next(0, '\uD800');
    public static string NextString => CreateStrings().First();

    public static float NextFloat
    {
        get
        {
            float f = Random.Shared.NextSingle();
            return float.MinValue - (f * float.MinValue) + (f * float.MaxValue);
        }
    }

    public static double NextDouble
    {
        get
        {
            double d = Random.Shared.NextDouble();
            return double.MinValue - (d * double.MinValue) + (d * double.MaxValue);
        }
    }

    public static decimal NextDecimal
    {
        get
        {
            decimal d = (decimal)Random.Shared.NextDouble();
            return decimal.MinValue - (d * decimal.MinValue) + (d * decimal.MaxValue);
        }
    }

    public static IEnumerable<byte> CreateBytes(byte min = byte.MinValue, byte limit = byte.MaxValue)
    {
        require(min < limit);

        while (true)
            yield return (byte)Random.Shared.Next(min, limit);
    }

    public static IEnumerable<sbyte> CreateSBytes(sbyte min = sbyte.MinValue, sbyte limit = sbyte.MaxValue)
    {
        require(min < limit);

        while (true)
            yield return (sbyte)Random.Shared.Next(min, limit);
    }

    public static IEnumerable<short> CreateShorts(short min = short.MinValue, short limit = short.MaxValue)
    {
        require(min < limit);

        while (true)
            yield return (short)Random.Shared.Next(min, limit);
    }

    public static IEnumerable<ushort> CreateUShorts(ushort min = ushort.MinValue, ushort limit = ushort.MaxValue)
    {
        require(min < limit);

        while (true)
            yield return (ushort)Random.Shared.Next(min, limit);
    }

    public static IEnumerable<int> CreateInts(int min = int.MinValue, int limit = int.MaxValue)
    {
        require(min < limit);

        while (true)
            yield return Random.Shared.Next(min, limit);
    }

    public static IEnumerable<uint> CreateUInts(uint min = uint.MinValue, uint limit = uint.MaxValue)
    {
        require(min < limit);

        while (true)
            yield return (uint)Random.Shared.NextInt64(min, limit);
    }

    public static IEnumerable<long> CreateLongs(long min = long.MinValue, long limit = long.MaxValue)
    {
        require(min < limit);

        while (true)
            yield return Random.Shared.NextInt64(min, limit);
    }

    public static IEnumerable<ulong> CreateULongs(ulong min = ulong.MinValue, ulong limit = ulong.MaxValue)
    {
        require(min < limit);

        double d;

        while (true)
        {
            //do not factor out d in case of overflow (MaxValue - MinValue)
            d = Random.Shared.NextDouble();
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
            f = Random.Shared.NextSingle();

            f = min - (f * min) + (f * limit); //do not factor out f in case of overflow (MaxValue - MinValue)
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
            d = Random.Shared.NextDouble();

            d = min - (d * min) + (d * limit); //do not factor out d in case of overflow (MaxValue - MinValue)
            yield return d >= limit || d < min ? min : d;
        }
    }

    public static IEnumerable<decimal> CreateDecimals(decimal min = decimal.MinValue, decimal limit = decimal.MaxValue)
    {
        require(min < limit);

        decimal d;

        while (true)
        {
            d = (decimal)Random.Shared.NextDouble();

            d = min - (d * min) + (d * limit); //do not factor out d in case of overflow (MaxValue - MinValue)
            yield return d >= limit || d < min ? min : d;
        }
    }

    public static IEnumerable<bool> CreateBools()
    {
        while (true)
            yield return Random.Shared.Next(0, 2) == 1;
    }

    public static IEnumerable<char> CreateChars(char min = char.MinValue, char limit = '\uD800')
    {
        require(min < limit);

        while (true)
            yield return (char)Random.Shared.Next(min, limit);
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

            len = Random.Shared.Next(minLen, lenLimit);
            chars = ArrayPool<char>.Shared.Rent(len);

            for (int i = 0; i < len; ++i)
                chars[i] = (char)Random.Shared.Next(0, charLimit);

            string result = new(chars, 0, len);
            ArrayPool<char>.Shared.Return(chars);

            yield return result;
        }
    }
}
