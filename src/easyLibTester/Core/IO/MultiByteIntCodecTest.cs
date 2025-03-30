using easyLib.IO;
using easyLib.Test;

namespace easyLibTester.Core.IO;

sealed class MultiByteIntCodecTest : UnitTest
{
    public MultiByteIntCodecTest() :
        base(nameof(MultiByteIntCodecTest))
    { }

    //protected:
    protected override void Start()
    {
        TestULong();
        TestLong();
        TestUInt();
        TestInt();
        TestUShort();
        TestShort();
    }

    //private:
    void TestShort()
    {
        runTest(short.MaxValue);
        runTest(short.MinValue);

        runTest(SampleFactory.NextShort);

        //local:
        void runTest(short item)
        {
            int len = MultiByteIntCodec.GetByteCount(item);
            Ensure(len > 0);
            Ensure(len <= MultiByteIntCodec.MaxByteCount);

            byte[] bytes0 = new byte[MultiByteIntCodec.MaxByteCount];
            int ret = MultiByteIntCodec.GetBytes(item, bytes0);
            Ensure(ret == len);

            byte[] bytes1 = MultiByteIntCodec.GetBytes(item);
            Ensure(bytes1.Length == len);
            Ensure(bytes1.SequenceEqual(bytes0.Take(len)));

            Ensure(MultiByteIntCodec.GetShort(bytes1) == item);

            ret = MultiByteIntCodec.GetShort(bytes0, out short item1);
            Ensure(ret == len);
            Ensure(item == item1);
        }
    }

    void TestUShort()
    {
        runTest(ushort.MaxValue);
        runTest(ushort.MinValue);
        runTest(SampleFactory.NextUShort);

        //local:
        void runTest(ushort item)
        {
            int len = MultiByteIntCodec.GetByteCount(item);
            Ensure(len > 0);
            Ensure(len <= MultiByteIntCodec.MaxByteCount);

            byte[] bytes0 = new byte[MultiByteIntCodec.MaxByteCount];
            int ret = MultiByteIntCodec.GetBytes(item, bytes0);
            Ensure(ret == len);

            byte[] bytes1 = MultiByteIntCodec.GetBytes(item);
            Ensure(bytes1.Length == len);
            Ensure(bytes1.SequenceEqual(bytes0.Take(len)));

            Ensure(MultiByteIntCodec.GetUShort(bytes1) == item);

            ret = MultiByteIntCodec.GetUShort(bytes0, out ushort item1);
            Ensure(ret == len);
            Ensure(item == item1);
        }
    }

    void TestInt()
    {
        runTest(int.MaxValue);
        runTest(int.MinValue);

        runTest(SampleFactory.NextInt);

        //local:
        void runTest(int item)
        {
            int len = MultiByteIntCodec.GetByteCount(item);
            Ensure(len > 0);
            Ensure(len <= MultiByteIntCodec.MaxByteCount);

            byte[] bytes0 = new byte[MultiByteIntCodec.MaxByteCount];
            int ret = MultiByteIntCodec.GetBytes(item, bytes0);
            Ensure(ret == len);

            byte[] bytes1 = MultiByteIntCodec.GetBytes(item);
            Ensure(bytes1.Length == len);
            Ensure(bytes1.SequenceEqual(bytes0.Take(len)));

            Ensure(MultiByteIntCodec.GetInt(bytes1) == item);

            ret = MultiByteIntCodec.GetInt(bytes0, out int item1);
            Ensure(ret == len);
            Ensure(item == item1);
        }
    }

    void TestUInt()
    {
        runTest(uint.MaxValue);
        runTest(uint.MinValue);

        runTest(SampleFactory.NextUInt);

        //local:
        void runTest(uint item)
        {
            int len = MultiByteIntCodec.GetByteCount(item);
            Ensure(len > 0);
            Ensure(len <= MultiByteIntCodec.MaxByteCount);

            byte[] bytes0 = new byte[MultiByteIntCodec.MaxByteCount];
            int ret = MultiByteIntCodec.GetBytes(item, bytes0);
            Ensure(ret == len);

            byte[] bytes1 = MultiByteIntCodec.GetBytes(item);
            Ensure(bytes1.Length == len);
            Ensure(bytes1.SequenceEqual(bytes0.Take(len)));

            Ensure(MultiByteIntCodec.GetUInt(bytes1) == item);

            ret = MultiByteIntCodec.GetUInt(bytes0, out uint item1);
            Ensure(ret == len);
            Ensure(item == item1);
        }
    }

    void TestLong()
    {
        runTest(long.MaxValue);
        runTest(long.MinValue);

        runTest(SampleFactory.NextLong);

        //local:
        void runTest(long item)
        {
            int len = MultiByteIntCodec.GetByteCount(item);
            Ensure(len > 0);
            Ensure(len <= MultiByteIntCodec.MaxByteCount);

            byte[] bytes0 = new byte[MultiByteIntCodec.MaxByteCount];
            int ret = MultiByteIntCodec.GetBytes(item, bytes0);
            Ensure(ret == len);

            byte[] bytes1 = MultiByteIntCodec.GetBytes(item);
            Ensure(bytes1.Length == len);
            Ensure(bytes1.SequenceEqual(bytes0.Take(len)));

            Ensure(MultiByteIntCodec.GetLong(bytes1) == item);

            ret = MultiByteIntCodec.GetLong(bytes0, out long item1);
            Ensure(ret == len);
            Ensure(item == item1);
        }
    }

    void TestULong()
    {
        runTest(ulong.MaxValue);
        runTest(ulong.MinValue);

        runTest(SampleFactory.NextULong);

        //local:
        void runTest(ulong item)
        {
            int len = MultiByteIntCodec.GetByteCount(item);
            Ensure(len > 0);
            Ensure(len <= MultiByteIntCodec.MaxByteCount);

            byte[] bytes0 = new byte[MultiByteIntCodec.MaxByteCount];
            int ret = MultiByteIntCodec.GetBytes(item, bytes0);
            Ensure(ret == len);

            byte[] bytes1 = MultiByteIntCodec.GetBytes(item);
            Ensure(bytes1.Length == len);
            Ensure(bytes1.SequenceEqual(bytes0.Take(len)));

            Ensure(MultiByteIntCodec.GetULong(bytes1) == item);

            ret = MultiByteIntCodec.GetULong(bytes0, out ulong item1);
            Ensure(ret == len);
            Ensure(item == item1);
        }
    }
}

