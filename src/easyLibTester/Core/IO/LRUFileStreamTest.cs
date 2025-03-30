using easyLib.Disposables;
using easyLib.IO;
using easyLib.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyLibTester.Core.IO;

sealed class LRUFileStreamTest : UnitTest<LRUFileStream>
{
    public LRUFileStreamTest() :
        base(nameof(LRUFileStreamTest))
    { }

    //protected:
    protected override IInvariantTester DefineInvariant(LRUFileStream fs, IInvariantTester invTester) =>
        invTester[!(fs.IsConnected && fs.IsDisposed)]
        [!fs.CanWrite || fs.IsConnected]
        [fs.CanRead == fs.IsConnected]
        [double.IsNaN(fs.CacheHitFactor) || 0 <= fs.CacheHitFactor && fs.CacheHitFactor <= 1];

    protected override void Start()
    {
        string filePath = Path.GetTempFileName();
        Cleaner.Add(new AutoReleaser(() => File.Delete(filePath)));

        TestConstruction(filePath);

        int buffCount = SampleFactory.CreateInts(1, byte.MaxValue).First();
        int buffSize = SampleFactory.CreateInts(1, ushort.MaxValue).First();
        using LRUFileStream fs = new(filePath, buffSize, buffCount);
        //m_ms.SetLength(0);

        TestCreate(fs);
        TestBufferSize(fs);
        TestWriteRead(fs);
    }


    //private:
    //MemoryStream m_ms = new();

    void TestWriteRead(LRUFileStream fs)
    {
        //Write(ReadOnlySpan<byte>)

        //Write(byte[], int, int)
        int size = SampleFactory.CreateShorts(0).First();
        int offset = size == 0 ? 0 : SampleFactory.CreateInts(0, size).First();
        int count = SampleFactory.CreateInts(0, size - offset + 1).First();
        byte[] buffer = SampleFactory.CreateBytes().Take(size).ToArray();

        long oldPos = fs.Position;
        fs.Write(buffer, offset, count);

        TestInvariant(fs);
        Ensure(fs.Position == oldPos + count);
        Ensure(fs.Length >= count);
        Ensure(count == 0 || !double.IsNaN(fs.CacheHitFactor));
    }

    void TestCreate(LRUFileStream fs)
    {
        bool share = SampleFactory.NextBool;
        fs.Create(share);

        TestInvariant(fs);
        Ensure(fs.IsConnected);
        Ensure(fs.CanRead);
        Ensure(fs.CanWrite);
        Ensure(fs.CanSeek);
        Ensure(fs.Position == 0);
        Ensure(fs.Length == 0);
        Ensure(double.IsNaN(fs.CacheHitFactor));
    }

    void TestBufferSize(LRUFileStream fs)
    {
        int buffSize = SampleFactory.CreateInts(1, ushort.MaxValue).First();
        fs.BufferSize = buffSize;

        TestInvariant(fs);
        Ensure(fs.BufferSize == buffSize);
        Ensure(double.IsNaN(fs.CacheHitFactor));
    }

    void TestConstruction(string filePath)
    {
        int buffCount = SampleFactory.CreateInts(1, byte.MaxValue).First();
        int buffSize = SampleFactory.CreateInts(1, ushort.MaxValue).First();
        using LRUFileStream fs = new(filePath, buffSize, buffCount);

        TestInvariant(fs);
        Ensure(fs.FilePath == filePath);
        Ensure(fs.BufferSize == buffSize);
        Ensure(fs.BufferCount == buffCount);
    }
}