# class LRUFileStream
```csharp
class LRUFileStream: Stream, IDestructible
{
  public const DefaultBufferSize;
  public const DefaultBufferCount;
  public LRUFileStream(string filePath, int bufferSize = DefaultBufferSize, int bufferCount = DefaultBufferCount);
  public string FilePath {get;}
  public bool IsConnected {get;}
  public bool IsDisposed {get;}
  public bool CanRead {get;}
  public bool CanWrite {get;}
  public bool CanSeek {get;}
  public long Length {get;}
  public long Position {get; set;}
  public int BufferSize {get; set;}
  public int BufferCount {get; set;}
  public double CacheHitFactor {get;}
  public void Create(bool shareReading = false);
	public void Connect(bool readOnly = false, bool shareReading = false);
	public void Disconnect();
	public int Read(Span<byte> buffer);
	public int Read(byte[] buffer, int offset, int count);
	public int ReadByte();
	public void Write(ReadOnlySpan<byte> buffer);
	public void Write(byte[] buffer, int offset, int count);
	public void WriteByte(byte value);
	public void Flush();
	public long Seek(long offset, SeekOrigin origin);
	public void SetLength(long value);
}
```
## Invariant
```csharp
Invariant
{
  !(IsConnected || IsDisposed);
  !CanWrite || IsConnected;
  CanRead == IsConnected;
  CanSeek == IsConnected;
  double.IsNaN(CacheHitFactor) || 0 <= CacheHitFactor <= 1;
}
```
## LRUFileStream(string, int, int)
```csharp
LRUFileStream(string filePath, int bufferSize = DefaultBufferSize, int bufferCount = DefaultBufferCount)
{
  require
  {
    !string.IsNullOrWhiteSpace(filePath);
    bufferCount >= 1;
    1 <= bufferSize <= Array.MaxLength;
  }
  ensure
  {
    FilePath == filePath;
    BufferSize == bufferSize;
    BufferCount == bufferCount;
  }
}
```
## BufferCount
```csharp
public int BufferCount
{
  set
  {
    require
    {
      value > 0;
    }
    ensure
    {
      BufferCount == value;
      double.IsNAN(CaheHitFactor);
    }
  }
}
```
## BufferSize
```csharp
public int BufferSize
{
  set
  {
    require
    {
      0 < value <= Array.MaxLength;
    }
    ensure
    {
      BufferSize = value;
      double.IsNAN(CaheHitFactor);
    }
  }
}
```
## CacheHitFactor
```csharp
public double CacheHitFactor
{
  get
  {
    ensure
    {
      IsConnected || double.IsNAN(CaheHitFactor);
    }
  }
}
```
## Connect(bool, bool)
```csharp
public void Connect(bool readOnly = false, bool shareReading = false)
{
  require
  {
    !IsConnected;
    File.Exists(FilePath);
  }
  ensure
  {
    IsConnected;
    CanRead;
    CanSeek;
    CanWrite == !readOnly;
    Position == 0;
  }
  throws
  {
    IOException;
    SecurityException;
    UnauthorizedAccessException;
  }
}
```
## Create(bool)
```csharp
public void Create(bool shareReading = false)
{
  require
  {
    !IsConnected;
  }
  ensure
  {
    IsConnected;
    CanRead;
    CanWrite;
    CanSeek;
    Position == 0;
    Length == 0;
    double.IsNaN(CacheHitFactor);
  }
  throws
  {
    ArgumentException;
    NotSupportedException;
    DirectoryNotFoundException;
    PathTooLongException;
    IOException;
    SecurityException;
    UnauthorizedAccessException;
  }
}
```
## Disconnect()
```csharp
public void Disconnect()
{
  require
  {
    IsConnected;
  }
  ensure
  {
    !IsConnected;
  }
  throws
  {
    IOException;
  }
}
```
## Flush()
```csharp
public void Flush()
{
  require
  {
    CanWrite;
  }
  throws
  {
    IOException;
  }
}
```
## Length
```csharp
public long Length
{
	get
	{
		require
		{
			IsConnected;
		}
	}
}
```
## Position
```csharp
long Position
{
  get
  {
    require
    {
      IsConnected;
    }
  }
  set
  {
    require
    {
      IsConnected;
      value >= 0;
    }
    ensure
    {
      Position == value;
    }
  }
}
```
## Read(byte[], int, int)
```csharp
public int Read(byte[] buffer, int offset, int count)
{
  require
  {
    IsConnected;
    buffer != null;
    offset >= 0;
    0 <= count <= buffer.Length - offset;
  }
  ensure
  {
    0 <= Result <= count;
    Result == count || Position >= Length;
    Position == old Position + Result;
  }
  throws
  {
    IOException;
  }
}
```
## Read(Span\<byte>)
```csharp
public int Read(Span<byte> buffer)
{
  require
  {
    IsConnected;
  }
  ensure
  {
    0 <= Result <= buffer.Length;
    Result == buffer.Length || Position >= Length;
    Position == old Position + Buffer.Length;
  }
  throws
  {
    IOException;
  }
}
```
## ReadByte()
```csharp
public int ReadByte()
{
  require
  {
    IsConnected;
  }
  ensure
  {
    -1 <= Result <= byte.MaxValue;
    Result == -1 || old Position < Length;
  }
  throws
  {
    IOException;
  }
}
```
## Seek(long, SeekOrigin)
```csharp
public long Seek(long offset, SeekOrigin origin)
{
  require
  {
    IsConnected;
    Enum.IsDefined(origin);
    origin != SeekOrigin.Begin || offset >= 0;
    origin == SeekOrigin.Begin || Position + offset >= 0;
  }
  ensure
  {
    Result == Position;
    origin != SeekOrigin.Begin || Position == offset;
    origin != SeekOrigin.Current || Position == old Position + offset;
    origin != SeekOrigin.End || Position == Length + offset;
  }
}
```
## SetLength(long)
```csharp
public void SetLength(long value)
{
  require
  {
    CanWrite;
    value >= 0;
  }
  ensure
  {
    Length == value;
    value > old Length || Position == old Position;
    value <= old Length || Position == Length;
  }
}
```
## Write(byte[], int, int)
  ```csharp
public void Write(byte[] buffer, int offset, int count)
{
  require
  {
    CanWrite;
    buffer != null;
    offset >= 0;
    0 <= count <= buffer.Length - offset;
  }
  ensure
  {
    Position == old Position + count;
    Length >= count;
    Count == 0 || !double.IsNaN(CacheHitFactor);
  }
  throws
  {
    IOException;
  }
}
```
## Write(ReadOnlySpan\<byte>)
```c#
public void Write(ReadOnlySpan<byte> buffer)
{
  require
  {
    CanWrite;
  }
  ensure
  {
    Position == old Position + buffer.Length;
    Length >= buffer.Length;
  }
  throws
  {
    IOException;
  }
}
```
## WriteByte(byte)
```csharp
public void WriteByte(byte value)
{
  require
  {
    CanWrite;
  }
  ensure
  {
    Position == old Position + 1;
    Length > 0;
  }
  throws
  {
    IOException;
  }
}
```