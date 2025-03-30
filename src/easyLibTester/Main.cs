
using easyLib.Test;
using easyLibTester.Core.ADT;
using easyLibTester.Core.Disposables;
using easyLibTester.Core.Extensions;
using easyLibTester.Core.IO;
using easyLibTester.Test;

TestManager testManager = new();

//easyLib.Test
testManager.AddTest(new SampleFactoryTest());

//easyLib
testManager.AddTest(new DisposableCollectionTest());
testManager.AddTest(new ConcurrentDisposableCollectionTest());

//easyLib.Extensions
testManager.AddTest(new ArrayExTest());
testManager.AddTest(new TypeExTest());
testManager.AddTest(new EnumerableExTest());
testManager.AddTest(new SpanExTest());
testManager.AddTest(new ListExTest());

//easyLib.IO
testManager.AddTest(new MultiByteIntCodecTest());
testManager.AddTest(new BinaryStreamReaderWriterTest());
testManager.AddTest(new RandomAccessStreamReaderWriterTest());
testManager.AddTest(new BufferTest());
testManager.AddTest(new BufferReaderWriterTest());
testManager.AddTest(new LRUFileStreamTest());

//easyLib.ADT
testManager.AddTest(new ForwardListTest());

testManager.BreakOnFailure = true;
testManager.Execute(SampleFactory.NextByte + 1);
DisposablesTracker.AssertEmpty();