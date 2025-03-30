using easyLib.Disposables;
using easyLib.Test;

namespace easyLibTester.Core.Disposables;

sealed class DisposableCollectionTest : UnitTest<DisposableCollection>
{
    public DisposableCollectionTest() :
        base(nameof(DisposableCollectionTest))
    { }

    //protected:
    protected override IInvariantTester DefineInvariant(DisposableCollection obj,
                                                        IInvariantTester invTester)
        => invTester[!obj.Contains(null)];

    protected override void Start()
    {
        TestAdd();
        TestClear();
        TestDispose();
    }

    //private:
    void TestDispose()
    {
        DisposableCollection dc = new();
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
        using DisposableCollection dc = new();
        List<Disposable> sample = new();
        int count = SampleFactory.NextByte;

        for (int i = 0; i < count; ++i)
        {
            Disposable d = new();
            dc.Add(d);
            sample.Add(d);
        }

        dc.Clear(false);

        TestInvariant(dc);
        Ensure(sample.All(e => !dc.Contains(e)));
        Ensure(sample.All(e => !e.IsDisposed));

        foreach (Disposable d in sample)
            dc.Add(d);

        dc.Clear(true);

        TestInvariant(dc);
        Ensure(sample.All(e => e.IsDisposed));
        Ensure(sample.All(e => !dc.Contains(e)));
    }

    void TestAdd()
    {
        List<Disposable> sample = new();
        using DisposableCollection dc = new();

        TestInvariant(dc);

        int count = SampleFactory.NextByte;

        for (int i = 0; i < count; ++i)
        {
            Disposable d = new();
            dc.Add(d);
            sample.Add(d);
        }

        TestInvariant(dc);
        Ensure(sample.All(dc.Contains));
        Ensure(sample.All(e => !e.IsDisposed));
    }
}
