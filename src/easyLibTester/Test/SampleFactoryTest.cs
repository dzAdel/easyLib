using easyLib.Test;

namespace easyLibTester.Test;

sealed class SampleFactoryTest : UnitTest
{
    public SampleFactoryTest() :
        base(nameof(SampleFactoryTest))
    { }


    //protected:
    protected override void Start()
    {
        TestNextByte();
        TestNextSByte();
        TestNextShort();
        TestNextInt();
        TestNextUInt();
        TestNextLong();
        TestNextULong();
        TestNextFloat();
        TestNextDouble();
        TestNextDecimal();
        TestNextChar();
        TestNextString();
        TestCreateBytes();
        TestCreateBools();
        TestCreateSBytes();
        TestCreateShorts();
        TestCreateUShorts();
        TestCreateInts();
        TestCreateUInts();
        TestCreateLongs();
        TestCreateULongs();
        TestCreateFloats();
        TestCreateDoubles();
        TestCreateDecimals();
        TestCreateChars();
        TestCreateStrings();
    }

    //private:
    void TestCreateStrings()
    {
        int minLen = SampleFactory.NextByte;
        int lenLimit = SampleFactory.NextByte;

        while (minLen == lenLimit)
        {
            minLen = SampleFactory.NextByte;
            lenLimit = SampleFactory.NextByte;
        }

        if (lenLimit < minLen)
            (minLen, lenLimit) = (lenLimit, minLen);

        string[] res = SampleFactory.CreateStrings(minLen, lenLimit).Take(SampleFactory.NextByte).ToArray();
        Ensure(res.All(s => s.Length >= minLen));
        Ensure(res.All(s => s.Length < lenLimit));
        Ensure(res.All(s => s.All(c => !char.IsSurrogate(c))));
    }

    void TestCreateChars()
    {
        char min = SampleFactory.NextChar;
        char limit = SampleFactory.NextChar;

        while (min == limit)
        {
            min = SampleFactory.NextChar;
            limit = SampleFactory.NextChar;
        }

        if (limit < min)
            (min, limit) = (limit, min);

        IEnumerable<char> res = SampleFactory.CreateChars(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));
    }

    void TestCreateDecimals()
    {
        decimal min = SampleFactory.NextDecimal;
        decimal limit = SampleFactory.NextDecimal;

        while (min == limit)
        {
            min = SampleFactory.NextDecimal;
            limit = SampleFactory.NextDecimal;
        }

        if (limit < min)
            (min, limit) = (limit, min);

        IEnumerable<decimal> res = SampleFactory.CreateDecimals(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));
    }

    void TestCreateDoubles()
    {
        double min = SampleFactory.NextDouble;
        double limit = SampleFactory.NextDouble;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            limit = Math.BitIncrement(limit);

        IEnumerable<double> res = SampleFactory.CreateDoubles(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = Math.BitDecrement(double.MaxValue);
        limit = double.MaxValue;
        res = SampleFactory.CreateDoubles(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = double.MinValue;
        limit = Math.BitIncrement(double.MinValue);
        res = SampleFactory.CreateDoubles(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));
    }

    void TestCreateFloats()
    {
        float min = SampleFactory.NextFloat;
        float limit = SampleFactory.NextFloat;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            limit = MathF.BitIncrement(limit);

        IEnumerable<float> res = SampleFactory.CreateFloats(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = MathF.BitDecrement(float.MaxValue);
        limit = float.MaxValue;
        res = SampleFactory.CreateFloats(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = float.MinValue;
        limit = MathF.BitIncrement(float.MinValue);
        res = SampleFactory.CreateFloats(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));
    }

    void TestCreateULongs()
    {
        ulong min = SampleFactory.NextULong;
        ulong limit = SampleFactory.NextULong;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<ulong> res = SampleFactory.CreateULongs(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = ulong.MaxValue - 1;
        limit = ulong.MaxValue;
        res = SampleFactory.CreateULongs(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        min = ulong.MinValue;
        limit = ulong.MinValue + 1;
        res = SampleFactory.CreateULongs(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));
    }

    void TestCreateLongs()
    {
        long min = SampleFactory.NextLong;
        long limit = SampleFactory.NextLong;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<long> res = SampleFactory.CreateLongs(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = long.MaxValue - 1;
        limit = long.MaxValue;
        res = SampleFactory.CreateLongs(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        min = long.MinValue;
        limit = long.MinValue + 1;
        res = SampleFactory.CreateLongs(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));
    }

    void TestCreateUInts()
    {
        uint min = SampleFactory.NextUInt;
        uint limit = SampleFactory.NextUInt;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<uint> res = SampleFactory.CreateUInts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = uint.MaxValue - 1;
        limit = uint.MaxValue;
        res = SampleFactory.CreateUInts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        min = uint.MinValue;
        limit = uint.MinValue + 1;
        res = SampleFactory.CreateUInts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));
    }

    void TestCreateInts()
    {
        int min = SampleFactory.NextInt;
        int limit = SampleFactory.NextInt;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<int> res = SampleFactory.CreateInts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = int.MaxValue - 1;
        limit = int.MaxValue;
        res = SampleFactory.CreateInts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        min = int.MinValue;
        limit = int.MinValue + 1;
        res = SampleFactory.CreateInts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));
    }

    void TestCreateUShorts()
    {
        ushort min = SampleFactory.NextUShort;
        ushort limit = SampleFactory.NextUShort;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<ushort> res = SampleFactory.CreateUShorts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = ushort.MaxValue - 1;
        limit = ushort.MaxValue;
        res = SampleFactory.CreateUShorts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        min = ushort.MinValue;
        limit = ushort.MinValue + 1;
        res = SampleFactory.CreateUShorts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));
    }

    void TestCreateShorts()
    {
        short min = SampleFactory.NextShort;
        short limit = SampleFactory.NextShort;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<short> res = SampleFactory.CreateShorts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = short.MaxValue - 1;
        limit = short.MaxValue;
        res = SampleFactory.CreateShorts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        min = short.MinValue;
        limit = short.MinValue + 1;
        res = SampleFactory.CreateShorts(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));
    }

    void TestCreateBools()
    {
        Ensure(SampleFactory.CreateBools().Take(int.MaxValue).Any(e => !e));
        Ensure(SampleFactory.CreateBools().Take(int.MaxValue).Any(e => e));
    }

    void TestCreateSBytes()
    {
        sbyte min = SampleFactory.NextSByte;
        sbyte limit = SampleFactory.NextSByte;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<sbyte> res = SampleFactory.CreateSBytes(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = sbyte.MaxValue - 1;
        limit = sbyte.MaxValue;
        res = SampleFactory.CreateSBytes(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        min = sbyte.MinValue;
        limit = sbyte.MinValue + 1;
        res = SampleFactory.CreateSBytes(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));
    }

    void TestCreateBytes()
    {
        byte min = SampleFactory.NextByte;
        byte limit = SampleFactory.NextByte;

        if (limit < min)
            (min, limit) = (limit, min);
        else
            ++limit;

        IEnumerable<byte> res = SampleFactory.CreateBytes(min, limit).Take(byte.MaxValue);
        Ensure(res.All(e => e >= min));
        Ensure(res.All(e => e < limit));

        min = byte.MaxValue - 1;
        limit = byte.MaxValue;
        res = SampleFactory.CreateBytes(min, limit).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == min));

        res = SampleFactory.CreateBytes(0, 1).Take(SampleFactory.NextByte);
        Ensure(res.All(e => e == 0));
    }

    void TestNextULong()
    {
        ulong res = SampleFactory.NextULong;
        Ensure(res < ulong.MaxValue);
    }

    void TestNextUInt()
    {
        uint res = SampleFactory.NextUInt;
        Ensure(res < uint.MaxValue);
    }
    void TestNextString()
    {
        string res = SampleFactory.NextString;
        Ensure(res.Length < byte.MaxValue);
        Ensure(res.All(c => !char.IsSurrogate(c)));
    }

    void TestNextShort()
    {
        short res = SampleFactory.NextShort;
        Ensure(res < short.MaxValue);
    }

    void TestNextLong()
    {
        long res = SampleFactory.NextLong;
        Ensure(res < long.MaxValue);
    }

    void TestNextInt()
    {
        int res = SampleFactory.NextInt;
        Ensure(res < int.MaxValue);
    }

    void TestNextFloat()
    {
        float result = SampleFactory.NextFloat;

        Ensure(float.IsFinite(result));
        Ensure(result < float.MaxValue);
    }

    void TestNextDouble()
    {
        double result = SampleFactory.NextDouble;

        Ensure(double.IsFinite(result));
        Ensure(result < double.MaxValue);
    }

    void TestNextDecimal()
    {
        decimal result = SampleFactory.NextDecimal;
        Ensure(result < decimal.MaxValue);
    }

    void TestNextChar()
    {
        char result = SampleFactory.NextChar;
        Ensure(!char.IsSurrogate(result));
    }

    void TestNextSByte()
    {
        sbyte result = SampleFactory.NextSByte;
        Ensure(result < sbyte.MaxValue);
    }
    void TestNextByte()
    {
        byte result = SampleFactory.NextByte;
        Ensure(result < byte.MaxValue);
    }
}
