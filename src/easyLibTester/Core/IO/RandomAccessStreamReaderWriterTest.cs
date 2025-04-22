using easyLib.Extensions;
using easyLib.IO;
using easyLib.Test;

namespace easyLibTester.Core.IO;

sealed class RandomAccessStreamReaderWriterTest : BinaryStreamReaderWriterTest
{
    public RandomAccessStreamReaderWriterTest() :
        base(nameof(RandomAccessStreamReaderWriterTest))
    { }

    //protected:
    protected override void Start()
    {
        base.Start();
        TestPosition();
    }

    protected override void TestConstruction()
    {
        base.TestConstruction();

        Stream stm = GetStream();
        ByteOrder endianness = NextByteOrder();
        using SeekableStreamReader reader = new(stm, endianness);
        Ensure(reader.Position == stm.Position);
        Ensure(reader.Length == stm.Length);
        Ensure(reader.ByteOrder.SameAs(endianness));

        using SeekableStreamWriter writer = new(stm, endianness);
        Ensure(writer.Position == stm.Position);
        Ensure(writer.Length == stm.Length);
        Ensure(writer.ByteOrder.SameAs(endianness));
    }

    protected override SeekableStreamWriter CreateWriter(ByteOrder endianness)
    {
        SeekableStreamWriter writer = new(GetStream(), endianness);
        Cleaner.Add(writer);
        return writer;
    }

    protected override SeekableStreamWriter CreateWriter() => CreateWriter(NextByteOrder());

    protected override SeekableStreamReader CreateReader(ByteOrder endianness)
    {
        SeekableStreamReader reader = new(GetStream(), endianness);
        Cleaner.Add(reader);
        return reader;
    }

    protected override SeekableStreamReader CreateReader() => CreateReader(NextByteOrder());

    //private:
    void TestPosition()
    {
        ResetStream();

        using SeekableStreamWriter writer = CreateWriter();
        byte[] bytes = SampleFactory.CreateBytes().Take(SampleFactory.NextByte + 1).ToArray();
        writer.WriteBytes(bytes);

        long pos = SampleFactory.CreateLongs(0, bytes.Length + 1).First();
        writer.Position = pos;
        Ensure(writer.Position == pos);

        using SeekableStreamReader reader = CreateReader(writer.ByteOrder);
        reader.Position = pos;
        Ensure(reader.Position == pos);

        pos = SampleFactory.CreateLongs(0, bytes.Length).First();
        reader.Position = pos;
        Ensure(reader.Position == pos);
        byte b = reader.ReadByte();
        Ensure(b == bytes[pos]);
    }
}

