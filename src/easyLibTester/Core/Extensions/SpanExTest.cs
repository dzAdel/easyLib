using easyLib.Extensions;
using easyLib.Test;

namespace easyLibTester.Core.Extensions;

sealed class SpanExTest : UnitTest
{
    public SpanExTest() :
        base(nameof(SpanExTest))
    { }

    //protected:
    protected override void Start()
    {
        TestReverseSlice();
        TestToEnumerable();
    }

    //private:
    void TestToEnumerable()
    {
        int[] ints = [.. SampleFactory.CreateInts().Take(SampleFactory.NextByte)];
        ReadOnlySpan<int> span = new(ints);
        Ensure(span.ToEnumerable().SequenceEqual(ints));

        span = new Span<int>(ints);
        Ensure(span.ToEnumerable().SequenceEqual(ints));
    }

    void TestReverseSlice()
    {
        byte[] bits = BitConverter.GetBytes(SampleFactory.NextInt);
        byte[] bitsReversed = [.. bits.Reverse()];
        bitsReversed.AsSpan().ReverseSlice(sizeof(int));
        Ensure(bits.SequenceEqual(bitsReversed));

        int count = SampleFactory.NextByte;
        int[] ints = SampleFactory.CreateInts().Take(count).ToArray();
        List<byte> bytes = [];
        List<byte> revBytes = [];

        for (int i = 0; i < count; ++i)
        {
            bits = BitConverter.GetBytes(ints[i]);
            bytes.AddRange(bits);
            revBytes.AddRange(bits.Reverse());
        }

        int off = SampleFactory.NextByte;

        byte[] src = SampleFactory.CreateBytes().
            Take(off).
            Concat(bytes).
            Concat(SampleFactory.CreateBytes().Take(SampleFactory.NextByte)).
            ToArray();
        src.AsSpan(off, count * sizeof(int)).ReverseSlice(sizeof(int));

        Ensure(src.Skip(off).Take(count * sizeof(int)).SequenceEqual(revBytes));
    }
}

