# interface IReadOnlyBuffer
```csharp
//ver 1
interface IReadOnlyBuffer: IReadOnlyList<byte>
{
  bool IsEmpty {get;}
  IBufferReader GetReader(int offset = 0, ByteOrder endianness = ByteOrder.System);
  IBufferReader GetReader(int offset, int len, ByteOrder endianness);
  ReadOnlySpan<byte> AsReadOnlySpan(int count, int ndxStart = 0);
  ReadOnlySpan<byte> AsReadOnlySpan();
  void CopyTo(Stream dest, int count, int ndxFrom = 0);
  void CopyTo(Span<byte> dest, int ndxFrom = 0);
}
```
## Invariant
```csharp
Invariant
{
  0 <= Count <= Array.MaxLength;
  this.Count() == Count;
  IsEmpty == (Count == 0);
}
```
## AsReadOnlySpan()
```csharp
ReadOnlySpan<byte> AsReadOnlySpan()
{
  ensure
  {
    Result.Length == Count;
    Result.ToImmutableArray().SequenceEqual(this);
  }
}
```
## AsReadOnlySpan(int, int)
```csharp
ReadOnlySpan<byte> AsReadOnlySpan(int count, int ndxStart = 0)
{
  require
  {
    count >= 0;
    ndxStart >= 0;
    count <= Count - ndxStart;
  }
  ensure
  {
    Result.Length == count;
    Result.ToImmutableArray().SequenceEqual(this.Skip(ndxStart).Take(count));
  }
}
```
## CopyTo(Span\<byte>, int)
```csharp
void CopyTo(Span<byte> dest, int ndxFrom = 0)
{
  require
  {
    ndxFrom >= 0;
    dest.Length <= Count - ndxFrom;
  }
  ensure
  {
    dest.ToImmutableArray().SequenceEqual(this.Skip(ndxFrom).Take(count));
  }
}
```
## CopyTo(Stream, int, int)
```csharp
void CopyTo(Stream dest, int count, int ndxFrom = 0)
{
  require
  {
    dest != null;
    dest.CanWrite;
    count >= 0;
    ndxFrom >= 0;
    count <= Length - ndxFrom;
  }
}
```
## GetReader(int, ByteOrder)
```csharp
IBufferReader GetReader(int offset = 0, ByteOrder endianness = ByteOrder.System)
{
  require
  {
    0 <= offset <= Count;
    Enum.IsDefined(endianness);
  }
  ensure
  {
    Result != null;
    Result.Length == Count - offset;
    Result.Position == 0;
    Result.ByteOrder.SameAs(endianness);
  }
}
```
## GetReader(int, int, ByteOrder)
```csharp
IBufferReader GetReader(int offset, int len, ByteOrder endianness = ByteOrder.System)
{
  require
  {
    len >= 0;
    offset >= 0;
    Enum.IsDefined(endianness);
    len <= Count - offset;
  }
  ensure
  {
    Result != null;
    Result.Length == len;
    Result.Position == 0;
    Result.ByteOrder.SameAs(endianness);
  }
}
```
# class Buffer
```csharp
//ver 1
class Buffer: IReadOnlyBuffer, IDestrictible
{
  public Buffer();
  public Buffer(int maxLength, bool reserve = false);
  public Buffer(Buffer other);
  public bool IsDisposed {get;}
  public int Capacity {get;}
  public int Count {get;}
  public bool IsEmpty {get;}
  public bool IsFull {get;}
  public byte this[int ndx] {get; set;}
  public ReadOnlySpan<byte> AsReadOnlySpan(int count, int ndxStart = 0);
  public ReadOnlySpan<byte> AsReadOnlySpan();
  public Span<byte> AsSpan(int count, int ndxStart = 0);
  public Span<byte> AsSpan();
  public IEnumerator<byte> GetEnumerator();
  public void Add(byte b);
  public void Add(IEnumerable<byte> src);
  public int Put(IEnumerable<byte> src, int ndxTo = 0);
  public void Strip(int count);
  public void Clear();
  public void Fill(byte b, int count, int ndxStart = 0);
  public void Reverse(int count, int ndxStart = 0);
  public void ReverseSlice(int chunkCount, int szChunk, int ndxStart = 0);
  public void CopyTo(Span<byte> dest, int ndxFrom = 0);
  public void CopyTo(Buffer dest, int count, int ndxTo = 0, int ndxFrom = 0);
  public void CopyTo(Stream dest, int count, int ndxFrom = 0);
  public void CopyFrom(ReadOnlySpan<byte> src, int ndxTo = 0);
  public void CopyFrom(IReadOnlyBuffer src, int count, int ndxTo = 0, int ndxFrom = 0);
  public int CopyFrom(Stream src, int count, int ndxTo = 0);
  public IBufferReader GetReader(int offset, int len, ByteOrder endianness);
  public IBufferReader GetReader(int offset = 0, ByteOrder endianness = ByteOrder.System);
  public IBufferWriter GetWriter(int offset, int maxLen, ByteOrder endianness);
  public IBufferWriter GetWriter(int offset = 0, ByteOrder endianness = ByteOrder.System);
  public void Dispose();
}
```
## Invariant
```csharp
Invariant
{
  0 <= Capacity <= Array.MaxLength;
  0 <= Count <= Capacity;
  IsEmpty == (Count == 0);
  IsFull == (Count == Capacity);
  (IsFull && IsEmpty) == (Capacity == 0);
}
```
## Buffer()
```csharp
public Buffer()
{
  ensure
  {
    Capacity == Array.MaxLength;
    IsEmpty;
  }
}
```
## Buffer(Buffer)
```csharp
public Buffer(Buffer other)
{
  require
  {
    other != null;
  }
  ensure
  {
    Capacity == other.Capacity;
    Count == other.Count;
    this.SequenceEqual(other);
  }
}
```
## Buffer(int, bool)
```csharp
public Buffer(int maxLength, bool resereve = false)
{
  require
  {
    0 <= maxLength <= Array.MaxLength;
  }
  ensure
  {
    Capacity == maxLength;
    IsEmpty;
  }
}
```
## Add(byte)
```csharp
public void Add(byte b)
{
  require
  {
    !IsFull;
  }
  ensure
  {
    !IsEmpty;
    Count == old Count + 1;
    this[Count - 1] == b;
  }
}
```
## Add(IEnumerable\<byte>)
```csharp
public int Add(IEnumerable<byte> src)
{
  require
  {
    src != null;
  }
  ensure
  {
    Result == src.Count();
    src.SequenceEqual(this.Skip(old Count).Take(Result));
  }
  throws
  {
    OverflowException;
  }
}
```
## AsSpan()
```csharp
public Span<byte> AsSpan()
{
  ensure
  {
    Result.Length == count;
    Result.ToImmutableArray().SequenceEqual(this);
  }
}
```
## AsSpan(int, int)
```csharp
public Span<byte> AsSpan(int count, int ndxStart = 0)
{
  require
  {
    count >= 0;
    ndxStart >= 0;
    count <= Count - ndxStart;
  }
  ensure
  {
    Result.Length == count;
    Result.ToImmutableArray().SequenceEqual(this.Skip(ndxStart).Take(count));
  }
}
```
## Clear()
```csharp
public void Clear()
{
  ensure
  {
    IsEmpty;
  }
}
```
## CopyFrom(IReadOnlyBuffer, int, int, int)
```csharp
public void CopyFrom(IReadOnlyBuffer dest, int count, int ndxTo = 0, int ndxFrom = 0)
{
  require
  {
    dest != null;
    count >= 0;
    ndxFrom >= 0;
    0 <= ndxTo <= Count;
    count <= dest.Count - ndxFrom;
    count <= Capacity - ndxTo;
  }
  ensure
  {
    Count == Math.Max(ndxTo + count, old Count);
    dest.Skip(ndxFrom).Take(count).SequenceEqual(this.Skip(ndxTo).Take(count));
  }
}
```
## CopyFrom(ReadOnlySpan<\byte>, int)
```csharp
public void CopyFrom(ReadOnlySpan<byte> src, int ndxTo = 0)
{
  require
  {
    0 <= ndxTo <= Count;
    src.Length <= Capacity - ndxTo;
  }
  ensure
  {
    Count == Math.Max(ndxTo + src.Length, old Count);
    src.ToImmutableArray().SequenceEqual(this.Skip(ndxTo).Take(src.Length));
  }
}
```
## CopyFrom(Stream, int, int)
```csharp
public int CopyFrom(Stream src, int count, int ndxTo = 0)
{
  require
  {
    src != null;
    src.CanRead;
    count >= 0;
    0 <= ndxTo <= Count;
    count <= Capacity - ndxTo;
  }
  ensure
  {
    0 <= Result <= count;
    Count == Math.Max(Result + ndxTo, old count);
  }
  throws
  {
    IOException;
  }
}
```
## CopyTo(Buffer, int, int, int)
```csharp
public void CopyTo(Buffer dest, int count, int ndxTo = 0, int ndxFrom = 0)
{
  require
  {
    dest != null;
    count >= 0;
    ndxFrom >= 0;
    0 <= ndxTo <= dest.Count;
    count <= dest.Capacity - ndxTo;
    count <= Count - ndxFrom;
  }
  ensure
  {
    dest.Count >= ndxTo + count;
    dest.Skip(ndxTo).Take(count).SequenceEqual(this.Skip(ndxFrom).Take(count));
  }
}
```
## CopyTo(Span\<byte>, int)
```csharp
public void CopyTo(Span<byte> dest, int ndxFrom = 0)
{
  require
  {
    ndxStart >= 0;
    dest.Length <= Count - ndxFrom;
  }
  ensure
  {
    dest.ToImmutableArray().SequenceEqual(this.Skip(ndxStart).Take(dest.Length));
  }
}
```
## CopyTo(Stream, int, int)
```csharp
public void CopyTo(Stream dest, int count, int ndxFrom = 0)
{
  require
  {
    dest != null;
    dest.CanWrite;
    count >= 0;
    ndxFrom >= 0;
    count <= Count - ndxFrom;
  }
  throws
  {
    IOException;
  }
}
```
## Dispose()
```csharp
public void Dispose()
{
  ensure
  {
    IsDisposed;
  }
}
```
## Fill(byte, int, int)
```csharp
public void Fill(byte b, int count, int ndxStart = 0)
{
  require
  {
    count >= 0;
    0 <= ndxStart <= Count;
    count <= Capacity - ndxStart;
  }
  ensure
  {
    this.Skip(ndxStart).Take(count).All(e => e == b);
  }
}
```
## GetReader(int, ByteOrder)
```csharp
IBufferReader GetReader(int offset = 0, ByteOrder endianness == ByteOrder.System)
{
  require
  {
    0 <= offset <= Count;
    Enum.IsDefined(endianness);
  }
  ensure
  {
    Result != null;
    Result.Length == Count - offset;
    Result.Position == 0;
    Result.ByteOrder.SameAs(endianness);
  }
}
```
## GetReader(int, int, ByteOrder)
```csharp
public IBufferReader GetReader(int offset, int len, ByteOrder endianness)
{
  require
  {
    len >= 0;
    offset >= 0;
    Enum.IsDefined(endianness);
    len <= Count - offset;
  }
  ensure
  {
    Result != null;
    Result.Length == len;
    Result.ByteOrder.SameAs(endianness);
    Result.Position == 0;
  }
}
```
## GetWriter(int, ByteOrder)
```csharp
public IBufferWriter GetWriter(int offset = 0. ByteOrder endianness = ByteOrder.System)
{
  require
  {
    0 <= offset <= Count;
    Enum.IsDefined(endianness);
  }
  ensure
  {
    Result != null;
    Result.Length == Count - offset;
    Result.Capacity = Capacity - offset;
    Result.ByteOrder.SameAs(endianness);
    Result.Position == 0;
  }
}
```
## GetWriter(int, int, ByteOrder)
```csharp
public IBufferWriter GetWriter(int offset, int maxLen, ByteOrder endianness)
{
  require
  {
    maxLen >= 0;
    0 <= offset <= Count;
    Enum.IsDefined(endianness);
    maxLen <= Capacity - offset;
  }
  ensure
  {
    Result != null;
    Result.Capacity = maxLen;
    Result.Length == Math.Min(Count - offset, maxLen);
    Result.ByteOrder.SameAs(endianness);
    Position == 0;
  }
}
```
## Put(IEnumerable\<byte>, int)
```csharp
public int Put(IEnumerable<byte> src, int ndxTo = 0)
{
  require
  {
    src != null;
    0 <= ndx <= Count;
    src.Count() <= Capcity - ndxTo;
  }
  ensure
  {
    Result == src.Count();
    Count >= Result + ndxTo;
    src.SequenceEqual(this.Skip(ndxTo).Take(Result));
  }
  throws
  {
    OverflowException;
  }
}
```
## Reverse(int, int)
```csharp
public void Reverse(int count, int ndxStart = 0)
{
  require
  {
    count >= 0;
    ndxStart >= 0;
    count <= Count - ndxStart;
  }
}
```
## ReverseSlice(int, int, int)
```csharp
public void Reverse(int chunkCount, int szChunck, int ndxStart = 0)
{
  require
  {
    szChunk > 0;
    chunkCount >= 0;
    ndxStart >= 0;
    chunkCount * szChunck <= Count - ndxStart;
  }
}
```
## Strip(int)
```csharp
public void Strip(int count)
{
  require
  {
    0 <= count <= Count;
  }
  ensure
  {
    Count = old Count - count;
  }
}
```
## this[int]
```csharp
public byte this[int ndx]
{
  get
  {
    require
    {
      0 <= ndx < Count;
    }
  }
  set
  {
    require
    {
      0 <= ndx <= Count;
      ndx != Count || !IsFull;
    }
    ensure
    {
      !IsEmpty;
      this[ndx] == value;
    }
  }
}
```