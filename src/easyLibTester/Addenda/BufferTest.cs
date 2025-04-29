using easyLib.Extensions;
using easyLib.IO;
using easyLib.Test;
using Buffer = easyLib.IO.Buffer;

namespace easyLibTester.Addenda;

sealed class BufferTest : UnitTest<Buffer>
{
    public BufferTest() :
        base(nameof(BufferTest))
    { }

    //protected:
    protected override void Start()
    {
        TestConstruction();
        TestAdd();
        TestStrip();
        TestClear();
        TestReverse();
        TestReverseSlice();
        TestAsSpan();
        TestAsReadOnlySpan();
        TestIndexer();
        TestCopyTo();
        TestPut();
        TestCopyFrom();
        TestGetReader();
        TestGetWriter();
    }

    protected override IInvariantTester DefineInvariant(Buffer obj, IInvariantTester invTester) =>
        invTester[obj.Capacity >= 0]
        [obj.Capacity <= Array.MaxLength]
        [obj.Count >= 0]
        [obj.Count <= obj.Capacity]
        [obj.IsEmpty == (obj.Count == 0)]
        [obj.IsFull == (obj.Count == obj.Capacity)]
        [(obj.IsFull && obj.IsEmpty) == (obj.Capacity == 0)];


    //private:
    void TestGetWriter()
    {
        //GetWriter(ByteOrder)
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        ByteOrder endianness = Enum.GetValues<ByteOrder>().Shuffle().First();
        int offset = SampleFactory.CreateInts(0, buff.Count + 1).First();
        IBufferWriter writer = buff.GetWriter(offset, endianness);
        TestInvariant(buff);
        Ensure(writer.Capacity == buff.Capacity - offset);
        Ensure(writer.Length == buff.Count - offset);
        Ensure(writer.Position == 0);
        Ensure(writer.ByteOrder.SameAs(endianness));

        //GetWriter(int, int, ByteOrder)
        offset = SampleFactory.CreateInts(0, buff.Count + 1).First();
        int maxLen = SampleFactory.CreateInts(0, buff.Capacity - offset + 1).First();

        IBufferWriter writer2 = buff.GetWriter(offset, maxLen, endianness);
        TestInvariant(buff);
        Ensure(writer2.Capacity == maxLen);
        Ensure(writer2.Length == Math.Min(buff.Count - offset, maxLen));
        Ensure(writer2.ByteOrder.SameAs(endianness));
        Ensure(writer2.Position == 0);
    }

    void TestGetReader()
    {
        //GetReader(ByteOrder)
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        ByteOrder endianness = Enum.GetValues<ByteOrder>().Shuffle().First();
        int offset = SampleFactory.CreateInts(0, buff.Count + 1).First();

        IBufferReader reader = buff.GetReader(offset, endianness);
        TestInvariant(buff);
        Ensure(reader.Length == buff.Count - offset);
        Ensure(reader.Position == 0);
        Ensure(reader.ByteOrder.SameAs(endianness));

        //GetReader(int, int, ByteOrder)
        int len = SampleFactory.CreateInts(0, buff.Count + 1).First();
        offset = SampleFactory.CreateInts(0, buff.Count - len + 1).First();

        IBufferReader reader2 = buff.GetReader(offset, len, endianness);
        TestInvariant(buff);
        Ensure(reader2.Length == len);
        Ensure(reader2.ByteOrder.SameAs(endianness));
        Ensure(reader2.Position == 0);
    }

    void TestCopyFrom()
    {
        //CopyFrom(ReadOnlySpan<byte>, int)
        int count = SampleFactory.NextByte;
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        byte[] bytes = SampleFactory.CreateBytes().Take(count).ToArray();
        int ndxTo = SampleFactory.CreateInts(0, buff.Count + 1).First();
        int buffCount = buff.Count;
        buff.CopyFrom(bytes, ndxTo);
        TestInvariant(buff);
        Ensure(buff.Count == Math.Max(count + ndxTo, buffCount));
        Ensure(buff.Skip(ndxTo).Take(count).SequenceEqual(bytes));

        //CopyFrom(IReadOnlyBuffer, int, int, int)
        int ndxFrom = SampleFactory.NextByte;
        count = SampleFactory.CreateInts(0, buff.Count + 1).First();
        using Buffer buff2 = new()
        {
            SampleFactory.CreateBytes().Take(ndxFrom + count)
        };

        ndxTo = SampleFactory.CreateInts(0, buff.Count + 1).First();
        buffCount = buff.Count;
        buff.CopyFrom(buff2, count, ndxTo, ndxFrom);
        TestInvariant(buff);
        TestInvariant(buff2);
        Ensure(buff.Count == Math.Max(count + ndxTo, buffCount));
        Ensure(buff.AsSpan(count, ndxTo).SequenceEqual(buff2.AsReadOnlySpan(count, ndxFrom)));

        //CopyFrom(Stream, int, int)
        count = SampleFactory.NextByte;
        MemoryStream ms = new();
        ms.Write(SampleFactory.CreateBytes().Take(count).ToArray(), 0, count);
        ndxTo = SampleFactory.CreateInts(0, buff.Count + 1).First();
        ms.Position = 0;
        buffCount = buff.Count;
        int n = buff.CopyFrom(ms, count, ndxTo);
        Ensure(n == count);
        Ensure(buff.Count == Math.Max(n + ndxTo, buffCount));
        Ensure(buff.AsSpan(n, ndxTo).SequenceEqual(ms.ToArray()));
    }

    void TestPut()
    {
        int ndxTo = SampleFactory.NextByte;
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte + ndxTo)
        };

        int count = SampleFactory.NextByte;
        //brache 1
        byte[] bytes = SampleFactory.CreateBytes().Take(count).ToArray();
        int n = buff.Put(bytes, ndxTo);

        TestInvariant(buff);
        Ensure(n == count);
        Ensure(buff.Count >= ndxTo + count);
        Ensure(buff.AsReadOnlySpan(count, ndxTo).SequenceEqual(bytes));

        //branche 2
        List<byte> list = new(SampleFactory.CreateBytes().Take(SampleFactory.NextByte));
        ndxTo = SampleFactory.CreateInts(0, buff.Count + 1).First();
        n = buff.Put(list, ndxTo);
        count = list.Count;
        TestInvariant(buff);
        Ensure(n == count);
        Ensure(buff.Count >= ndxTo + count);
        Ensure(buff.AsReadOnlySpan(count, ndxTo).SequenceEqual(list.ToArray()));

        //branche 3
        count = SampleFactory.NextByte;
        ndxTo = SampleFactory.CreateInts(0, buff.Count + 1).First();
        bytes = SampleFactory.CreateBytes().Take(count).ToArray();
        n = buff.Put(toEnumerable(bytes), ndxTo);
        TestInvariant(buff);
        Ensure(n == count);
        Ensure(buff.Count >= ndxTo + count);
        Ensure(buff.Skip(ndxTo).Take(count).SequenceEqual(toEnumerable(bytes)));

        //local:
        static IEnumerable<byte> toEnumerable(byte[] bytes)
        {
            foreach (byte b in bytes)
                yield return b;
        }
    }

    void TestCopyTo()
    {
        //CopyTo(Span<byte>, int)
        int ndxFrom = SampleFactory.NextByte;
        int count = SampleFactory.NextByte;
        byte[] bytes = new byte[count];

        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte + ndxFrom + count)
        };

        Span<byte> span = bytes;
        buff.CopyTo(span, ndxFrom);

        TestInvariant(buff);
        Ensure(buff.Skip(ndxFrom).Take(count).SequenceEqual(bytes));

        //CopyTo(Buffer, int, int, int)
        using Buffer buff1 = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        int ndxTo = buff1.IsEmpty ? 0 : SampleFactory.CreateInts(0, buff1.Count + 1).First();
        buff.CopyTo(buff1, count, ndxTo, ndxFrom);
        TestInvariant(buff);
        TestInvariant(buff1);
        Ensure(buff.AsReadOnlySpan(count, ndxFrom).SequenceEqual(buff1.AsReadOnlySpan(count, ndxTo)));

        //CopyTo(Stream, int, int)
        MemoryStream ms = new();
        buff.CopyTo(ms, count, ndxFrom);
        TestInvariant(buff);
        Ensure(buff.AsReadOnlySpan(count, ndxFrom).SequenceEqual(ms.ToArray()));
    }

    void TestIndexer()
    {
        using Buffer buff = new();
        byte b = SampleFactory.NextByte;
        buff[0] = b;
        TestInvariant(buff);
        Ensure(!buff.IsEmpty);
        Ensure(buff.Count == 1);
        Ensure(buff[0] == b);

        buff.Add(SampleFactory.CreateBytes().Take(SampleFactory.NextByte));
        int ndx = SampleFactory.CreateInts(0, buff.Count + 1).First();
        b = SampleFactory.NextByte;
        buff[ndx] = b;
        TestInvariant(buff);
        Ensure(buff[ndx] == b);
    }

    void TestAsReadOnlySpan()
    {
        //AsReadOnlySpan()
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        ReadOnlySpan<byte> span = buff.AsReadOnlySpan();
        Ensure(span.ToArray().SequenceEqual(buff));

        //AsReadOnlySpan(int, int)
        int count = SampleFactory.NextByte;
        int off = buff.Count;
        byte[] bytes = SampleFactory.CreateBytes().Take(count).ToArray();
        buff.Add(bytes);
        buff.Add(SampleFactory.CreateBytes().Take(SampleFactory.NextByte));
        span = buff.AsReadOnlySpan(count, off);
        Ensure(span.ToArray().SequenceEqual(bytes));
    }

    void TestAsSpan()
    {
        //AsSpan()
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        Span<byte> span = buff.AsSpan();
        Ensure(span.ToArray().SequenceEqual(buff));

        //AsSpan(int, int)
        int count = SampleFactory.NextByte;
        int off = buff.Count;
        byte[] bytes = SampleFactory.CreateBytes().Take(count).ToArray();
        buff.Add(bytes);
        buff.Add(SampleFactory.CreateBytes().Take(SampleFactory.NextByte));
        span = buff.AsSpan(count, off);
        Ensure(span.ToArray().SequenceEqual(bytes));
    }

    void TestReverseSlice()
    {
        int count = SampleFactory.NextByte;
        int[] ints = SampleFactory.CreateInts().Take(count).ToArray();
        List<byte> revBytes = new();
        int off = SampleFactory.NextByte;
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(off)
        };

        foreach (int n in ints)
        {
            byte[] bits = BitConverter.GetBytes(n);
            buff.Add(bits);
            revBytes.AddRange(bits.Reverse());
        }

        buff.Add(SampleFactory.CreateBytes().Take(SampleFactory.NextByte));
        buff.ReverseSlice(count, sizeof(int), off);
        TestInvariant(buff);
        Ensure(buff.Skip(off).Take(count << 2).SequenceEqual(revBytes));
    }

    void TestReverse()
    {
        int count = SampleFactory.NextByte;
        byte[] bytes = SampleFactory.CreateBytes().Take(count).ToArray();
        int ndx = SampleFactory.NextByte;
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(ndx),
            bytes,
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        Array.Reverse(bytes);
        buff.Reverse(count, ndx);
        TestInvariant(buff);
        Ensure(buff.Skip(ndx).Take(count).SequenceEqual(bytes));
    }

    void TestClear()
    {
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        buff.Clear();
        TestInvariant(buff);
        Ensure(buff.IsEmpty);
    }

    void TestStrip()
    {
        using Buffer buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte + 1)
        };

        int count = buff.Count;
        int n = SampleFactory.CreateInts(0, count + 1).First();
        buff.Strip(n);

        TestInvariant(buff);
        Ensure(buff.Count == count - n);
    }

    void TestAdd()
    {
        //Add(byte)
        using Buffer buff = new(1);
        byte b = SampleFactory.NextByte;
        buff.Add(b);
        TestInvariant(buff);
        Ensure(!buff.IsEmpty);
        Ensure(buff.IsFull);
        Ensure(buff.Count == 1);
        Ensure(buff[0] == b);

        //Add(IEnumerable<byte>)
        using Buffer buff2 = new();
        byte[] bytes = SampleFactory.CreateBytes().Take(SampleFactory.NextByte).ToArray();
        int n = buff2.Add(bytes);
        TestInvariant(buff2);
        Ensure(n == bytes.Length);
        Ensure(buff2.Count == n);
        Ensure(bytes.SequenceEqual(buff2));

        byte[] bytes2 = SampleFactory.CreateBytes().Take(SampleFactory.NextByte).ToArray();
        n = buff2.Add(bytes2);
        TestInvariant(buff2);
        Ensure(n == bytes2.Length);
        Ensure(buff2.Count == bytes.Length + bytes2.Length);
        Ensure(bytes.SequenceEqual(buff2.Take(bytes.Length)));
        Ensure(bytes2.SequenceEqual(buff2.Skip(bytes.Length)));

        int count = SampleFactory.NextByte;
        using Buffer buff3 = new(count);
        EnsureThrow<OverflowException>(() => buff3.Add(SampleFactory.CreateBytes().Take(count + 1)));
    }

    void TestConstruction()
    {
        int maxLen = SampleFactory.NextByte;
        using Buffer buff1 = new(maxLen, SampleFactory.NextBool);

        Ensure(buff1.Capacity == maxLen);
        Ensure(buff1.IsEmpty);
        TestInvariant(buff1);

        using Buffer buff2 = new();
        Ensure(buff2.Capacity == Array.MaxLength);
        Ensure(buff2.IsEmpty);
        TestInvariant(buff2);

        using Buffer buff3 = new(buff1);
        Ensure(buff3.Capacity == buff1.Capacity);
        Ensure(buff3.Count == buff1.Count);
        TestInvariant(buff3);

        using Buffer buff4 = new(0, SampleFactory.NextBool);
        Ensure(buff4.Capacity == 0);
        Ensure(buff4.IsEmpty == true);
        Ensure(buff4.IsFull == true);
        TestInvariant(buff4);

        using Buffer buff5 = new(Array.MaxLength, SampleFactory.NextBool);
        Ensure(buff5.Capacity == Array.MaxLength);
        Ensure(buff5.IsEmpty);
        TestInvariant(buff5);
    }
}

