using easyLib.Extensions;
using easyLib.Test;

namespace easyLibTester.Core.Extensions;

sealed class ListExTest : UnitTest
{
    public ListExTest() :
        base(nameof(ListExTest))
    { }

    //protected:
    protected override void Start()
    {
        TestPut();
        TestIndexOf();
    }

    //private:
    void TestIndexOf()
    {
        List<int> list = new();
        list.AddRange(SampleFactory.CreateInts(min: 0).Take(SampleFactory.NextByte + 1));
        int ndxStart = SampleFactory.CreateInts(0, list.Count).First();
        int item = list.Skip(ndxStart).Shuffle().First();
        Func<int, int, bool> eqls = (x, y) => Math.Abs(x) == Math.Abs(y);
        int ndxResult = list.IndexOf(item, ndxStart, eqls);
        Ensure(ndxResult < list.Count);
        Ensure(ndxResult >= ndxStart);
        Ensure(list[ndxResult] == item);

        item = list.Skip(ndxStart).Shuffle().First();
        ndxResult = list.IndexOf(-item, ndxStart, eqls);
        Ensure(ndxResult < list.Count);
        Ensure(ndxResult >= ndxStart);
        Ensure(list[ndxResult] == item);

        item = SampleFactory.CreateInts(limit: 0).First();
        Ensure(((IReadOnlyList<int>)list).IndexOf(item, ndxStart) == -1);

        list = new(SampleFactory.CreateInts().Take(SampleFactory.NextByte + 1));
        item = SampleFactory.NextInt;
        ndxStart = SampleFactory.CreateInts(0, list.Count).First();
        ndxResult = list.IndexOf(item, ndxStart, eqls);
        Ensure(ndxResult < list.Count);
        Ensure(ndxResult == -1 || ndxResult >= ndxStart);
        Ensure(ndxResult == -1 || eqls(list[ndxResult], item));
    }


    void TestPut()
    {
        //Put(this IList<T>, T, int)
        List<int> list = new();
        list.AddRange(SampleFactory.CreateInts().Take(SampleFactory.NextByte));

        int item = SampleFactory.NextInt;
        list.Put(item, list.Count);
        Ensure(list[^1] == item);

        item = SampleFactory.NextInt;
        int ndx = SampleFactory.CreateInts(0, list.Count + 1).First();
        list.Put(item, ndx);
        Ensure(list[ndx] == item);

        //Put<T>(this IList<T>, IEnumerabe<T>, int)
        ndx = SampleFactory.CreateInts(0, list.Count + 1).First();
        int[] items = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        int count = list.Put(items, ndx);
        Ensure(count == items.Length);
        Ensure(list.Skip(ndx).Take(count).SequenceEqual(items));
        Ensure(list.Count >= count + ndx);
    }
}

