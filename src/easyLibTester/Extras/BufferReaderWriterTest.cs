using easyLib.Extensions;
using easyLib.Extras.IO;
using easyLib.IO;
using easyLib.Test;
using Buffer = easyLib.Extras.IO.Buffer;

namespace easyLibTester.Extras;

sealed class BufferReaderWriterTest : UnitTest
{
    Buffer? m_buff;
    ByteOrder m_endianness;

    public BufferReaderWriterTest() :
        base(nameof(BufferReaderWriterTest))
    { }

    //protected:
    protected override void Start()
    {
        m_buff = new()
        {
            SampleFactory.CreateBytes().Take(SampleFactory.NextByte)
        };

        Cleaner.Add(m_buff);
        m_endianness = Enum.GetValues<ByteOrder>().Shuffle().First();

        TestWriteReadBytes();
        TestWriteReadSBytes();
        TestWriteReadBools();
        TestWriteReadUShorts();
        TestWriteReadShorts();
        TestWriteReadUInts();
        TestWriteReadInts();
        TestWriteReadULongs();
        TestWriteReadLongs();
        TestWriteReadFloats();
        TestWriteReadDoubles();
        TestWriteReadDecimals();
        TestWriteReadChars();
        TestWriteReadString();
    }

    //private:
    void TestWriteReadString()
    {
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        string str = SampleFactory.NextString;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        writer.WriteString(str);
        TestInvariant(writer);

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        string str1 = reader.ReadString();
        Ensure(str1 == str);
        TestInvariant(reader);
    }

    void TestWriteReadChars()
    {
        //Write(char) + ReadChar()
        List<char> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextChar);
        writer.WriteChar(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(char));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(char));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(char));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadChar() == list[^1]);
        Ensure(reader.Position == sizeof(char));
        TestInvariant(reader);

        //Write(ReadOnlySpan<decimal>) + ReadDescimal(Span<decimal>)
        char[] items = SampleFactory.CreateChars().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteChars(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(char));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(char));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(char));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new char[list.Count];
        reader.ReadChars(items);
        Ensure(reader.Position == list.Count * sizeof(char));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadDecimals()
    {
        //Write(decimal) + ReadDecimal()
        List<decimal> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextDecimal);
        writer.WriteDecimal(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(decimal));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(decimal));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(decimal));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadDecimal() == list[^1]);
        Ensure(reader.Position == sizeof(decimal));
        TestInvariant(reader);

        //Write(ReadOnlySpan<decimal>) + ReadDescimal(Span<decimal>)
        decimal[] items = SampleFactory.CreateDecimals().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteDecimals(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(decimal));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(decimal));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(decimal));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new decimal[list.Count];
        reader.ReadDecimals(items);
        Ensure(reader.Position == list.Count * sizeof(decimal));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadDoubles()
    {
        //Write(double) + ReadDouble()
        List<double> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextDouble);
        writer.WriteDouble(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(double));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(double));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(double));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadDouble() == list[^1]);
        Ensure(reader.Position == sizeof(double));
        TestInvariant(reader);

        //Write(ReadOnlySpan<double>) + ReadDoubles(Span<double>)
        double[] items = SampleFactory.CreateDoubles().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteDoubles(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(double));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(double));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(double));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new double[list.Count];
        reader.ReadDoubles(items);
        Ensure(reader.Position == list.Count * sizeof(double));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadFloats()
    {
        //Write(float) + ReadFloat()
        List<float> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextFloat);
        writer.WriteFloat(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(float));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(float));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(float));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadFloat() == list[^1]);
        Ensure(reader.Position == sizeof(float));
        TestInvariant(reader);

        //Write(ReadOnlySpan<float>) + ReadFloats(Span<float>)
        float[] items = SampleFactory.CreateFloats().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteFloats(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(float));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(float));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(float));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new float[list.Count];
        reader.ReadFloats(items);
        Ensure(reader.Position == list.Count * sizeof(float));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadLongs()
    {
        //Write(long) + ReadLong()
        List<long> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextLong);
        writer.WriteLong(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(long));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(long));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(long));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadLong() == list[^1]);
        Ensure(reader.Position == sizeof(long));
        TestInvariant(reader);

        //Write(ReadOnlySpan<long>) + ReadLongs(Span<long>)
        long[] items = SampleFactory.CreateLongs().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteLongs(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(long));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(long));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(long));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new long[list.Count];
        reader.ReadLongs(items);
        Ensure(reader.Position == list.Count * sizeof(long));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadULongs()
    {
        //Write(ulong) + ReadULong()
        List<ulong> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextULong);
        writer.WriteULong(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(ulong));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(ulong));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(ulong));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadULong() == list[^1]);
        Ensure(reader.Position == sizeof(ulong));
        TestInvariant(reader);

        //Write(ReadOnlySpan<ulong>) + ReadULongs(Span<ulong>)
        ulong[] items = SampleFactory.CreateULongs().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteULongs(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(ulong));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(ulong));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(ulong));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new ulong[list.Count];
        reader.ReadULongs(items);
        Ensure(reader.Position == list.Count * sizeof(ulong));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadInts()
    {
        //Write(int) + ReadIInt()
        List<int> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextInt);
        writer.WriteInt(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(int));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(int));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(int));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadInt() == list[^1]);
        Ensure(reader.Position == sizeof(int));
        TestInvariant(reader);

        //Write(ReadOnlySpan<int>) + ReadBools(Span<int>)
        int[] items = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteInts(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(int));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(int));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(int));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new int[list.Count];
        reader.ReadInts(items);
        Ensure(reader.Position == list.Count * sizeof(int));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadUInts()
    {
        //Write(uint) + ReadUInt()
        List<uint> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextUInt);
        writer.WriteUInt(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(uint));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(uint));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(uint));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadUInt() == list[^1]);
        Ensure(reader.Position == sizeof(uint));
        TestInvariant(reader);

        //Write(ReadOnlySpan<uint>) + ReadBools(Span<uint>)
        uint[] items = SampleFactory.CreateUInts().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteUInts(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(uint));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(uint));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(uint));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new uint[list.Count];
        reader.ReadUInts(items);
        Ensure(reader.Position == list.Count * sizeof(uint));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadShorts()
    {
        //Write(short) + ReadShort()
        List<short> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextShort);
        writer.WriteShort(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(short));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(short));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(short));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadShort() == list[^1]);
        Ensure(reader.Position == sizeof(short));
        TestInvariant(reader);

        //Write(ReadOnlySpan<short>) + ReadBools(Span<short>)
        short[] items = SampleFactory.CreateShorts().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteShorts(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(short));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(short));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(short));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new short[list.Count];
        reader.ReadShorts(items);
        Ensure(reader.Position == list.Count * sizeof(short));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadUShorts()
    {
        //Write(ushort) + ReadUShort()
        List<ushort> list = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset, m_endianness);
        long oldLen = writer.Length;
        list.Add(SampleFactory.NextUShort);
        writer.WriteUShort(list[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == sizeof(ushort));
        Ensure(writer.Length >= oldLen || writer.Length <= oldLen + sizeof(ushort));
        Ensure(Buffer.Count >= oldCount || Buffer.Count <= oldCount + sizeof(ushort));

        IBufferReader reader = Buffer.GetReader(offset, m_endianness);
        Ensure(reader.ReadUShort() == list[^1]);
        Ensure(reader.Position == sizeof(ushort));
        TestInvariant(reader);

        //Write(ReadOnlySpan<ushort>) + ReadBools(Span<ushort>)
        ushort[] items = SampleFactory.CreateUShorts().Take(SampleFactory.NextByte).ToArray();
        list.AddRange(items);
        writer.WriteUShorts(items);
        TestInvariant(writer);
        Ensure(writer.Position == list.Count * sizeof(ushort));
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + list.Count * sizeof(ushort));
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + list.Count * sizeof(ushort));

        reader = Buffer.GetReader(offset, m_endianness);
        items = new ushort[list.Count];
        reader.ReadUShorts(items);
        Ensure(reader.Position == list.Count * sizeof(ushort));
        Ensure(items.SequenceEqual(list));
        TestInvariant(reader);
    }

    void TestWriteReadBools()
    {
        //Write(bool) + ReadBool()
        List<bool> bools = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset);
        long oldLen = writer.Length;
        bools.Add(SampleFactory.NextBool);
        writer.WriteBool(bools[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == 1);
        Ensure(writer.Length == oldLen || writer.Length == oldLen + 1);
        Ensure(Buffer.Count == oldCount || Buffer.Count == oldCount + 1);

        IBufferReader reader = Buffer.GetReader(offset);
        {
            Ensure(reader.ReadBool() == bools[^1]);
            Ensure(reader.Position == 1);
            TestInvariant(reader);
        }

        //Write(ReadOnlySpan<bool>) + ReadBools(Span<bool>)
        bool[] items = SampleFactory.CreateBools().Take(SampleFactory.NextByte).ToArray();
        bools.AddRange(items);
        writer.WriteBools(items);
        TestInvariant(writer);
        Ensure(writer.Position == bools.Count);
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + bools.Count);
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + bools.Count);

        reader = Buffer.GetReader(offset);
        items = new bool[bools.Count];
        reader.ReadBools(items);
        Ensure(reader.Position == bools.Count);
        Ensure(items.SequenceEqual(bools));
        TestInvariant(reader);
    }

    void TestWriteReadSBytes()
    {
        //Write(sbyte) + ReadSbyte()
        List<sbyte> sbytes = new();
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        int oldCount = Buffer.Count;
        IBufferWriter writer = Buffer.GetWriter(offset);
        long oldLen = writer.Length;
        sbytes.Add(SampleFactory.NextSByte);
        writer.WriteSByte(sbytes[^1]);
        TestInvariant(writer);
        Ensure(writer.Position == 1);
        Ensure(writer.Length == oldLen || writer.Length == oldLen + 1);
        Ensure(Buffer.Count == oldCount || Buffer.Count == oldCount + 1);

        IBufferReader reader = Buffer.GetReader(offset);
        Ensure(reader.ReadSByte() == sbytes[^1]);
        Ensure(reader.Position == 1);
        TestInvariant(reader);

        //Write(ReadOnlySan<sbyte>) + ReadSBytes(Span<sbyte>)
        sbyte[] items = SampleFactory.CreateSBytes().Take(SampleFactory.NextByte).ToArray();
        sbytes.AddRange(items);
        writer.WriteSBytes(items);
        TestInvariant(writer);
        Ensure(writer.Position == sbytes.Count);
        Ensure(writer.Length >= oldLen);
        Ensure(writer.Length <= oldLen + sbytes.Count);
        Ensure(Buffer.Count >= oldCount);
        Ensure(Buffer.Count <= oldCount + sbytes.Count);

        reader = Buffer.GetReader(offset);
        items = new sbyte[sbytes.Count];
        reader.ReadSBytes(items);
        Ensure(reader.Position == sbytes.Count);
        Ensure(items.SequenceEqual(sbytes));
        TestInvariant(reader);
    }

    void TestWriteReadBytes()
    {
        //Write(byte) + ReadByte()
        List<byte> bytes = new();
        bytes.AddRange(SampleFactory.CreateBytes().Take(SampleFactory.NextByte + 1));
        int offset = SampleFactory.CreateInts(0, Buffer.Count + 1).First();
        IBufferWriter writer = Buffer.GetWriter(offset);
        int oldCount = Buffer.Count;

        foreach (byte b in bytes)
            writer.WriteByte(b);
        TestInvariant(writer);
        Ensure(writer.Position == bytes.Count);
        Ensure(writer.Length == Buffer.Count - offset);
        Ensure(Buffer.Count == Math.Max(oldCount, offset + bytes.Count));

        IBufferReader reader = Buffer.GetReader(offset);
        Ensure(bytes.All(b => reader.ReadByte() == b));
        Ensure(reader.Position == bytes.Count);
        TestInvariant(reader);

        //Write(ReadOnlySpan<byte>) + ReadBytes(Span<byte>) + ReadBytes()
        oldCount = Buffer.Count;
        writer.WriteBytes(Buffer.AsReadOnlySpan(0));
        TestInvariant(writer);
        Ensure(writer.Position == bytes.Count);
        Ensure(Buffer.Count == oldCount);

        byte[] items = SampleFactory.CreateBytes().Take(SampleFactory.NextByte).ToArray();
        bytes.AddRange(items);
        writer.WriteBytes(items);
        TestInvariant(writer);
        Ensure(writer.Position == bytes.Count);
        Ensure(Buffer.Count == Math.Max(offset + bytes.Count, oldCount));

        byte[] items2 = new byte[bytes.Count];
        IBufferReader reader1 = Buffer.GetReader(offset);
        reader1.ReadBytes(items2);
        TestInvariant(reader1);
        Ensure(reader1.Position == bytes.Count);
        Ensure(bytes.SequenceEqual(items2));

        reader.Position = 0;
        Ensure(Buffer.Skip(offset).Take(bytes.Count).SequenceEqual(bytes));
        TestInvariant(reader);
    }
    void TestInvariant(IBufferReader reader)
    {
        Ensure(reader.Position <= reader.Length);
        Ensure(reader.Length <= Buffer.Count);
    }

    void TestInvariant(IBufferWriter writer)
    {
        Ensure(writer.Length <= writer.Capacity);
        Ensure(writer.Position <= writer.Length);
        Ensure(writer.Length <= Buffer.Count);
    }

    Buffer Buffer => m_buff ?? new();

}

