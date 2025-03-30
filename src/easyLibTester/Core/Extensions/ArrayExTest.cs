using easyLib.Extensions;
using easyLib.Test;

namespace easyLibTester.Core.Extensions;

sealed class ArrayExTest : UnitTest
{
    public ArrayExTest() :
        base(nameof(ArrayExTest))
    { }

    //protected:
    protected override void Start() => TestReverseSlice();

    //private:
    void TestReverseSlice()
    {
        int szSlice = SampleFactory.NextByte + 1;
        int sliceCount = SampleFactory.NextByte;
        int offset = SampleFactory.NextByte;
        byte[] sample = SampleFactory.CreateBytes().Take(szSlice * sliceCount + offset).ToArray();
        byte[] reversed = new byte[sample.Length];
        Array.Copy(sample, reversed, sample.Length);
        reversed.ReverseSlice(szSlice, sliceCount, offset);

        bool qry = reversed.Skip(offset).
            Take(sliceCount * szSlice).
            Chunk(szSlice).
            Zip(sample.Skip(offset).Take(sliceCount * szSlice).Chunk(szSlice)).
            All(p => p.First.Reverse().SequenceEqual(p.Second));

        Ensure(qry);
    }
}
