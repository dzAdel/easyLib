using easyLib;
using easyLib.Extensions;
using easyLib.IO;
using easyLib.Test;

namespace easyLibTester.Core.IO;

class BinaryStreamReaderWriterTest : UnitTest
{
    MemoryStream? m_stm;

    public BinaryStreamReaderWriterTest() :
        base(nameof(BinaryStreamReaderWriterTest))
    { }

    protected BinaryStreamReaderWriterTest(string caption) :
        base(caption)
    { }

    //protected:
    protected override void Start()
    {
        TestConstruction();
        TestWriteReadBytes();
        TestWriteReadSBytes();
        TestWriteReadBools();
        TestWriteReadChars();
        TestWriteReadUShorts();
        TestWriteReadShorts();
        TestWriteReadUInts();
        TestWriteReadInts();
        TestWriteReadULongs();
        TestWriteReadLongs();
        TestWriteReadFloats();
        TestWriteReadDoubles();
        TestWriteReadDecimals();
        TestWriteReadString();
    }

    protected virtual void TestConstruction()
    {
        ByteOrder endianness = NextByteOrder();

        using BinaryStreamReader reader = new(GetStream(), endianness);
        Ensure(reader.ByteOrder.SameAs(endianness));

        using BinaryStreamWriter writer = new(GetStream(), endianness);
        Ensure(writer.ByteOrder.SameAs(endianness));
    }

    protected virtual BinaryStreamReader CreateReader()
    {
        ByteOrder endianness = NextByteOrder();

        return CreateReader(endianness);
    }

    protected virtual BinaryStreamReader CreateReader(ByteOrder endianness)
    {
        BinaryStreamReader reader = new(GetStream(), endianness);
        Cleaner.Add(reader);

        return reader;
    }

    protected virtual BinaryStreamWriter CreateWriter()
    {
        ByteOrder endianness = NextByteOrder();
        return CreateWriter(endianness);
    }

    protected virtual BinaryStreamWriter CreateWriter(ByteOrder endianness)
    {
        BinaryStreamWriter writer = new(GetStream(), endianness);
        Cleaner.Add(writer);

        return writer;
    }

    protected static ByteOrder NextByteOrder()
    {
        ByteOrder[] byteOrders = { ByteOrder.BigEndian, ByteOrder.LittleEndian, ByteOrder.Network, ByteOrder.System };
        int ndx = SampleFactory.CreateInts(0, byteOrders.Length).First();

        return byteOrders[ndx];
    }

    protected Stream GetStream() => m_stm ??= new MemoryStream();
    protected void RewindStream() => GetStream().Position = 0;

    protected void ResetStream()
    {
        Stream stm = GetStream();
        stm.Position = 0;
        stm.SetLength(0);
    }

    //private:
    void TestWriteReadString()
    {
        ResetStream();

        //Write(string) + ReadString()
        string str = SampleFactory.NextString;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteString(str);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadString() == str);
    }

    void TestWriteReadDecimals()
    {
        ResetStream();

        //Write(decimal) + ReadDecimal()
        decimal d = SampleFactory.NextDecimal;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteDecimal(d);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadDecimal() == d);

        //Write(ReadOnlySpan<>) + ReadDecimals(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        decimal[] src = SampleFactory.CreateDecimals().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteDecimals(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        decimal[] dest = ArrayPool.Rent<decimal>(count + destOff);
        reader.ReadDecimals(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));
        ArrayPool.Return(dest);
    }

    void TestWriteReadDoubles()
    {
        ResetStream();

        //Write(double) + ReadDouble()
        double d = SampleFactory.NextDouble;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteDouble(d);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadDouble() == d);

        //Write(ReadOnlySpan<>) + ReadDoubles(Span<>)
        RewindStream();
        int srcOff = SampleFactory.NextByte;
        int count = SampleFactory.NextByte;
        double[] src = SampleFactory.CreateDoubles().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteDoubles(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        double[] dest = ArrayPool.Rent<double>(count + destOff);
        reader.ReadDoubles(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));
        ArrayPool.Return(dest);
    }

    void TestWriteReadFloats()
    {
        ResetStream();

        //Write(float) + ReadFloat()
        float f = SampleFactory.NextFloat;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteFloat(f);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadFloat() == f);

        //Write(ReadOnlySpan<>) + ReadFloats(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        float[] src = SampleFactory.CreateFloats().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteFloats(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        float[] dest = ArrayPool.Rent<float>(count + destOff);
        reader.ReadFloats(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));
        ArrayPool.Return(dest);
    }

    void TestWriteReadLongs()
    {
        ResetStream();

        //Write(long) + ReadLong()
        long l = SampleFactory.NextLong;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteLong(l);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadLong() == l);

        //Write(ReadOnlySpan<>) + ReadLongs(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        long[] src = SampleFactory.CreateLongs().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteLongs(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        long[] dest = ArrayPool.Rent<long>(count + destOff);
        reader.ReadLongs(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));
        ArrayPool.Return(dest);
    }

    void TestWriteReadULongs()
    {
        ResetStream();

        //Write(ulong) + ReadULong()
        ulong ul = SampleFactory.NextULong;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteULong(ul);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadULong() == ul);

        //Write(ReadOnlySpan<>) + ReadULongs(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        ulong[] src = SampleFactory.CreateULongs().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteULongs(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        ulong[] dest = ArrayPool.Rent<ulong>(count + destOff);
        reader.ReadULongs(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));

    }

    void TestWriteReadInts()
    {
        ResetStream();

        //Write(int) + ReadInt()
        int i = SampleFactory.NextInt;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteInt(i);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadInt() == i);

        //Write(ReadOnlySpan<>) + ReadInts(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        int[] src = SampleFactory.CreateInts().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteInts(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        int[] dest = ArrayPool.Rent<int>(destOff + count);
        reader.ReadInts(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));

        ArrayPool.Return(dest);
    }

    void TestWriteReadUInts()
    {
        ResetStream();

        //Write(uint) + ReadUInt()
        uint ui = SampleFactory.NextUInt;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteUInt(ui);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadUInt() == ui);

        //Write(ReadOnlySpan<>) + ReadUInts(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        uint[] src = SampleFactory.CreateUInts().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteUInts(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        uint[] dest = ArrayPool.Rent<uint>(count + destOff);
        reader.ReadUInts(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));

        ArrayPool.Return(dest);
    }

    void TestWriteReadShorts()
    {
        ResetStream();

        //Write(short) + ReadShort()
        short s = SampleFactory.NextShort;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteShort(s);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadShort() == s);

        //Write(ReadOnlySpan<>) + ReadShorts(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        short[] src = SampleFactory.CreateShorts().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteShorts(src.AsSpan(srcOff, count));

        int destOff = SampleFactory.NextByte;
        short[] dest = ArrayPool.Rent<short>(count + destOff);
        RewindStream();
        reader.ReadShorts(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));

        ArrayPool.Return(dest);
    }


    void TestWriteReadUShorts()
    {
        ResetStream();

        //Write(ushort) + ReadUShort()
        ushort us = SampleFactory.NextUShort;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteUShort(us);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadUShort() == us);

        //Write(ReadOnlySpan<>) + ReadUShorts(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        ushort[] src = SampleFactory.CreateUShorts().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteUShorts(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        ushort[] dest = ArrayPool.Rent<ushort>(count + destOff);
        reader.ReadUShorts(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));

        ArrayPool.Return(dest);
    }

    void TestWriteReadChars()
    {
        ResetStream();

        //Write(char) + ReadChar()
        char c = SampleFactory.NextChar;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteChar(c);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadChar() == c);

        //Write(ReadOnlySapn<>) + ReadChars(Span<>)
        RewindStream();
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        char[] src = SampleFactory.CreateChars().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        writer.WriteChars(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        char[] dest = ArrayPool.Rent<char>(count + destOff);
        reader.ReadChars(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));

        ArrayPool.Return(dest);
    }

    void TestWriteReadBools()
    {
        ResetStream();

        //Write(bool) + ReadBool()
        bool b = SampleFactory.NextBool;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteBool(b);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadBool() == b);

        //Write(ReadOnlySapn<>) + ReadBools(Span<>)
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        bool[] src = SampleFactory.CreateBools().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        RewindStream();
        writer.WriteBools(src.AsSpan(srcOff, count));

        RewindStream();
        int destOff = SampleFactory.NextByte;
        bool[] dest = ArrayPool.Rent<bool>(count + destOff);
        reader.ReadBools(dest.AsSpan(destOff, count));
        Ensure(dest.Skip(destOff).Take(count).SequenceEqual(src.Skip(srcOff).Take(count)));
        ArrayPool.Return(dest);
    }

    void TestWriteReadSBytes()
    {
        ResetStream();

        //Write(sbyte) + ReadSByte()
        sbyte sb = SampleFactory.NextSByte;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteSByte(sb);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadSByte() == sb);

        //Write(ReadOnlySapn<>) + ReadSBytes(Span<>)
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        sbyte[] src = SampleFactory.CreateSBytes().Take(count + srcOff + SampleFactory.NextByte).ToArray();
        ReadOnlySpan<sbyte> rosp = new(src, srcOff, count);

        RewindStream();
        writer.WriteSBytes(rosp);

        int dstOff = SampleFactory.NextByte;
        sbyte[] dest = ArrayPool.Rent<sbyte>(count + dstOff);

        RewindStream();
        reader.ReadSBytes(dest.AsSpan(dstOff, count));
        Ensure(src.Skip(srcOff).Take(count).SequenceEqual(dest.Skip(dstOff).Take(count)));

        ArrayPool.Return(dest);
    }

    void TestWriteReadBytes()
    {
        ResetStream();

        //Write(byte) + ReadByte()
        byte b = SampleFactory.NextByte;
        BinaryStreamWriter writer = CreateWriter();
        writer.WriteByte(b);

        BinaryStreamReader reader = CreateReader(writer.ByteOrder);
        RewindStream();
        Ensure(reader.ReadByte() == b);

        //Write(ReadOnlySapn<>) + ReadBytes(Span<>)
        int count = SampleFactory.NextByte;
        int srcOff = SampleFactory.NextByte;
        byte[] sample = SampleFactory.CreateBytes().Take(count + srcOff + SampleFactory.NextByte).ToArray();

        ResetStream();
        ReadOnlySpan<byte> src = new(sample, srcOff, count);
        writer.WriteBytes(src);

        RewindStream();
        int destOff = SampleFactory.NextByte;
        byte[] bytes = ArrayPool.Rent<byte>(count + destOff);
        Span<byte> dest = new(bytes, destOff, count);
        reader.ReadBytes(dest);

        Ensure(sample.Skip(srcOff).Take(count).SequenceEqual(bytes.Skip(destOff).Take(count)));

        ////ReadBytes()
        RewindStream();
        Ensure(reader.ReadBytes().SequenceEqual(sample.Skip(srcOff).Take(count)));

        ArrayPool.Return(bytes);
    }

    //void TestWriteRead()
    //{
    //    ResetStream();

    //    int count = SampleFactory.NextByte;
    //    int offset = SampleFactory.NextByte;
    //    int szSimple = SampleFactory.NextByte + count + offset;
    //    byte[] sample = SampleFactory.CreateBytes().Take(szSimple).ToArray();
    //    BinaryStreamWriter writer = CreateWriter();
    //    writer.Write(sample, count, offset);

    //    RewindStream();

    //    int offBuffer = SampleFactory.NextByte;
    //    int szBuffer = SampleFactory.NextByte + count + offBuffer;
    //    byte[] buffer = new byte[szBuffer];
    //    BinaryStreamReader reader = CreateReader(writer.ByteOrder);
    //    Ensure(reader.Read(buffer, count, offBuffer) == count);
    //    Ensure(sample.Skip(offset).Take(count).SequenceEqual(buffer.Skip(offBuffer).Take(count)));
    //}
}