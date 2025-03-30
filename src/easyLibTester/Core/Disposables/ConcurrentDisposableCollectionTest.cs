using easyLib.Disposables;
using easyLib.Test;
using System.Collections.Concurrent;

namespace easyLibTester.Core.Disposables;

sealed class ConcurrentDisposableCollectionTest : UnitTest<ConcurrentDisposableCollection>
{
    public ConcurrentDisposableCollectionTest() :
        base(nameof(ConcurrentDisposableCollectionTest))
    { }

    //protected:
    protected override IInvariantTester DefineInvariant(ConcurrentDisposableCollection obj,
                                                        IInvariantTester invTester)
        => invTester[!obj.Contains(null)];

    protected override void Start()
    {
        TestAdd();
        TestClear();
        TestDispose();
    }

    //private;
    void TestDispose()
    {
        ConcurrentDisposableCollection dc = new();
        List<Disposable> sample = new();
        int count = SampleFactory.NextByte;

        for (int i = 0; i < count; ++i)
        {
            Disposable d = new();
            dc.Add(d);
            sample.Add(d);
        }

        dc.Dispose();

        Ensure(sample.All(e => e.IsDisposed));
        Ensure(dc.IsDisposed);
    }

    void TestClear()
    {
        ConcurrentBag<Disposable> sample = new();
        using ConcurrentDisposableCollection dc = new();


        Action<int> act = n =>
        {
            if (n % 3 == 0)
                dc.Clear(true);
            else
            {
                Disposable d = new();
                dc.Add(d);
                sample.Add(d);
            }
        };

        int count = SampleFactory.NextByte + 1;

        Parallel.For(0, count, act);

        TestInvariant(dc);
        Ensure(!dc.IsDisposed);
        Ensure(sample.Where(e => e.IsDisposed).All(e => !dc.Contains(e)));
        Ensure(sample.Where(e => !e.IsDisposed).All(dc.Contains));
    }

    void TestAdd()
    {
        ConcurrentBag<Disposable> sample = new();
        using ConcurrentDisposableCollection dc = new();

        TestInvariant(dc);

        int count = SampleFactory.NextByte + 1;

        for (int i = 0; i < count; ++i)
        {
            Disposable d = new();
            dc.Add(d);
            sample.Add(d);
        }

        TestInvariant(dc);
        Ensure(sample.All(dc.Contains));
        Ensure(sample.All(e => !e.IsDisposed));

        count = SampleFactory.NextByte;

        Action<int> add = _ =>
        {
            Disposable d = new();
            dc.Add(d);
            sample.Add(d);
        };

        Parallel.For(0, count, add);

        TestInvariant(dc);
        Ensure(sample.All(dc.Contains));
        Ensure(sample.All(e => !e.IsDisposed));
    }
}
