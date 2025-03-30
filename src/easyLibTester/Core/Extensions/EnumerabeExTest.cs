using easyLib.Extensions;
using easyLib.Test;

namespace easyLibTester.Core.Extensions;

sealed class EnumerableExTest : UnitTest
{

    public EnumerableExTest() :
        base(nameof(EnumerableExTest))
    { }


    //protected:
    protected override void Start()
    {
        TestAll();
        TestIsSorted();
        TestIsOrdered();
        TestIndexOf();
        TestMinMax();
        TestShuffle();
        TestEmit();
    }

    //private:
    void TestEmit()
    {
        Ensure(!EnumerableEx.Emit(n => n, 0, 0).Any());

        int[] ints = Enumerable.Range(0, SampleFactory.NextByte + 1).ToArray();
        Func<int, int> gen = n => (n + 1) % (byte.MaxValue + 1);
        int initVal = ints[0];
        int stopVal = ints[^1] + 1;
        int[] result = EnumerableEx.Emit(gen, initVal, stopVal).ToArray();

        Ensure(result.SequenceEqual(ints));

        initVal = SampleFactory.NextByte;
        stopVal = SampleFactory.NextByte;
        result = EnumerableEx.Emit(gen, initVal, stopVal).ToArray();

        Ensure(result.Length != 0 == (initVal != stopVal));
        Ensure(initVal == stopVal || result.First() == initVal);
        Ensure(!result.Contains(stopVal));
    }

    void TestShuffle()
    {
        int[] sample = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        int[] result = sample.Shuffle().ToArray();

        Ensure(result.Length == sample.Length);
        Ensure(sample.All(e => result.Count(x => e == x) == sample.Count(x => e == x)));
    }

    void TestMinMax()
    {
        //MinMax(IEnumerable<long>)
        long[] longs = SampleFactory.CreateLongs().Take(SampleFactory.NextByte + 1).ToArray();
        long minLong = longs.Min();
        long maxLong = longs.Max();
        Ensure(longs.MinMax() == (minLong, maxLong));

        //MinMax(IEnumerable<decimal>)
        decimal[] decs = SampleFactory.CreateDecimals().Take(SampleFactory.NextByte + 1).ToArray();
        decimal minDec = decs.Min();
        decimal maxDec = decs.Max();
        Ensure(decs.MinMax() == (minDec, maxDec));

        //MinMax(IEnumerable<string>)
        string[] strs = SampleFactory.CreateStrings().Take(SampleFactory.NextByte + 1).ToArray();
        int minLen = strs.Min(s => s.Length);
        int maxLen = strs.Max(s => s.Length);
        var (minStr, maxStr) = strs.MinMax((x, y) => x.Length - y.Length);
        Ensure((minStr.Length, maxStr.Length) == (minLen, maxLen));
    }

    void TestIndexOf()
    {
        Ensure(Enumerable.Empty<int>().IndexOf(SampleFactory.NextInt) < 1);

        Func<int, int, bool> eq = (x, y) => Math.Abs(x) == Math.Abs(y);
        int item = SampleFactory.NextInt;
        int[] sample = SampleFactory.CreateBytes().Take(SampleFactory.NextByte).Select(e => (int)e).ToArray();

        int res = sample.IndexOf(item);
        Ensure(res < 0 || Math.Abs(sample[res]) == Math.Abs(item));
        Ensure(res < 0 || !sample.Take(res).Where(e => eq(e, item)).Any());
    }

    void TestIsOrdered()
    {
        Func<int, int, bool> precedes = (x, y) => Math.Abs(x) < Math.Abs(y);
        Ensure(Enumerable.Empty<int>().IsOrdered(precedes));
        Ensure(Enumerable.Repeat(SampleFactory.NextInt, 1).IsOrdered(precedes));

        int[] sample = SampleFactory.CreateInts().Take(SampleFactory.NextByte + 1).OrderBy(Math.Abs).ToArray();
        Ensure(sample.IsOrdered(precedes));

        sample = sample.Append(sample[0] - 1).Append(sample[0] + 1).ToArray();
        Ensure(!sample.IsOrdered(precedes));
    }

    void TestIsSorted()
    {
        Ensure(Enumerable.Empty<int>().IsSorted());
        Ensure(Enumerable.Repeat(SampleFactory.NextInt, 1).IsSorted());

        int[] sample = SampleFactory.CreateInts().Take(SampleFactory.NextByte).OrderBy(n => n).ToArray();
        Ensure(sample.IsSorted());

        sample = SampleFactory.CreateInts().Take(SampleFactory.NextByte).OrderBy(n => -n).ToArray();
        Ensure(sample.IsSorted());

        sample = SampleFactory.CreateInts().Take(SampleFactory.NextByte).OrderBy(Math.Abs).ToArray();
        Ensure(sample.IsSorted((x, y) => Math.Abs(x) - Math.Abs(y)));

        int item = SampleFactory.NextInt;
        sample = Enumerable.Repeat(item, SampleFactory.NextByte + 1)
                           .Concat(SampleFactory.CreateInts(min: item).Take(SampleFactory.NextByte + 1).OrderBy(n => n))
                           .ToArray();
        Ensure(sample.IsSorted());

        sample = SampleFactory.CreateInts(min: item).Take(SampleFactory.NextByte + 1).OrderBy(n => -n).Concat(sample).ToArray();
        Ensure(!sample.IsSorted());
    }

    void TestAll()
    {
        Func<int, int, bool> pred = (e, ndx) => e < ndx;
        Ensure(Enumerable.Empty<int>().All(pred));

        int[] sample = SampleFactory.CreateInts(limit: 0).Take(SampleFactory.NextByte + 1).ToArray();
        Ensure(sample.All(pred));

        int ndx = SampleFactory.CreateInts(0, sample.Length).First();
        sample[ndx] = ndx + 1;
        Ensure(!sample.All(pred));

        sample = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        bool res = sample.All(pred);
        Ensure(sample.Length != 0 || res);
        Ensure(sample.Select((e, ndx) => (e, ndx)).All(p => pred(p.e, p.ndx)) == res);
    }
}
