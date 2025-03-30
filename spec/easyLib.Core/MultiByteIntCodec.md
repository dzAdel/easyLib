# class MultiByteIntCodec

```csharp
//ver 1
static class MultiByteIntCodec
{
  public const int MaxByteCount;

  public static int GetByteCount(short s);
  public static int GetBytes(short s, byte[] buffer, int offset = 0);
  public static byte[] GetBytes(short s);
  public static short GetShort(IEnumerable<byte> bytes);
  public static int GetShort(Span<byte> bytes, out short result);

  public static int GetByteCount(ushort s);
  public static int GetBytes(ushort us, byte[] buffer, int offset = 0);
  public static byte[] GetBytes(ushort us);
  public static ushort GetUShort(IEnumerable<byte> bytes);
  public static int GetUShort(Span<byte> bytes, out ushort result);

  public static int GetByteCount(int n);
  public static int GetBytes(int n, byte[] buffer, int offset = 0);
  public static byte[] GetBytes(int n);
  public static int GetInt(IEnumerable<byte> bytes);
  public static int GetIntSpan<byte> bytes, out int result);

  public static int GetByteCount(uint u);
  public static int GetBytes(uint u, byte[] buffer, int offset = 0);
  public static byte[] GetBytes(uint u);
  public static uint GetUInt(IEnumerable<byte> bytes);
  public static int GetUIntSpan<byte> bytes, out uint result);

  public static int GetByteCount(long l);  
  public static int GetBytes(long l, byte[] buffer, int offset = 0);
  public static byte[] GetBytes(long l);
  public static long GetLong(IEnumerable<byte> bytes);
  public static int GetLongSpan<byte> bytes, out long result);

  public static int GetByteCount(ulong ul);  
  public static int GetBytes(ulong ul,Span<byte> dest);
  public static byte[] GetBytes(ulong ul);
  public static ulong GetULong(IEnumerable<byte> bytes);
  public static int GetULongSpan<byte> bytes, out ulong result);
}
```

## GetByteCount(int)

```csharp
public static int GetByteCount(int n)
{
   ensure
  {
    0 < Result <= MaxByteCount;
  } 
}
```

## GetByteCount(long)

```csharp
public static int GetByteCount(long l)
{
  ensure
  {
    0 < Result <= MaxByteCount;
  }
}
```

## GetByteCount(short)

```csharp
public static int GetByteCount(short s)
{
  ensure
  {
    0 < Result <= MaxByteCount;
  }
}
```

## GetByteCount(uint)

{

}

```csharp
public static int GetByteCount(uint u)
{
  ensure
  {
    0 < Result <= MaxByteCount;
  }   
}
```

## GetByteCount(ulong)

```csharp
public static int GetByteCount(ulong ul)
{
  ensure
  {
    0 < Result <= MaxByteCount;
  }
}
```

## GetByteCount(ushort)

```csharp
public static int GetByteCount(ushort s)
{
  ensure
  {
    0 < Result <= MaxByteCount;
  }  
}
```

## GetBytes(int)

```csharp
public static byte[] GetBytes(int n)
{
  ensure
  {
    Result.Length == GetByteCount(n);
    Result.Length == 1 || Result[Length - 1] != 0;
    GetLong(Result) == l;
  }
}
```

## GetBytes(int, Span<byte>)

```csharp
public static int GetBytes(int n, Span<byte> buffer)
{
  require
  {
    GetByteCount(n) <= buffer.Length;
  }
  ensure
  {
    0 < Result <= MaxByteCount; 
    buffer.SequenceEqual(GetBytes(n));
  }  
}
```

## GetBytes(long)

```csharp
public static byte[] GetBytes(long l)
{
  ensure
  {
    Result.Length == GetByteCount(l);
    Result.Length == 1 || Result[Length - 1] != 0;
    GetLong(Result) == l;
  }
}
```

## GetBytes(long, Span\<byte>)

```csharp
public static int GetBytes(long l, Span<byte> buffer)
{
  require
  {
    GetByteCount(l) <= buffer.Length;
  }
  ensure
  {
    0 < Result <= MaxByteCount; 
    buffer.SequenceEqual(GetBytes(l));
  }
}
```

## GetBytes(short)

```csharp
public static byte[] GetBytes(short s)
{
  ensure
  {
    Result.Length == GetByteCount(s);
    Result.Length == 1 || Result[Length - 1] != 0;
    GetULong(Result) == s;    
  }  
}
```

## GetBytes(short, Span\<byte>)

```csharp
public static int GetBytes(short s, Span<byte> buffer)
{
  require
  {
    GetByteCount(s) <= buffer.Length;
  }
  ensure
  {
    0 < Result <= MaxByteCount; 
    buffer.SequenceEqual(GetBytes(s));
  }  
}
```

## GetBytes(uint)

```csharp
public static byte[] GetBytes(uint u)
{
  ensure
  {
    Result.Length == GetByteCount(u);
    Result.Length == 1 || Result[Length - 1] != 0;
    GetULong(Result) == u;    
  }  
}
```

## GetBytes(uint, Span\<byte>)

```csharp
public static int GetBytes(uint u, Span<byte> buffer)
{
  require
  {
    GetByteCount(u) <= buffer.Length;
  }
  ensure
  {
    0 < Result <= MaxByteCount; 
    buffer.SequenceEqual(GetBytes(u));
  }   
}
```

## GetBytes(ulong)

```csharp
public static byte[] GetBytes(ulong ul)
{
  ensure
  {
    Result.Length == GetByteCount(ul);
    Result.Length == 1 || Result[Length - 1] != 0;
    GetULong(Result) == ul;    
  }
}
```

## GetBytes(ulong, Span\<byte>)

```csharp
public static int GetBytes(ulong ul, Span<byte> dest)
{
  require
  {
    GetByteCount(ul) <= dest.Length; 
  }
  ensure
  {
    Result GetByteCount(ul); 
    bytes.SequenceEqual(GetBytes(ul));
  }  
}
```

## GetBytes(ushort)

```csharp
public static byte[] GetBytes(ushort us)
{
  ensure
  {
    Result.Length == GetByteCount(us);
    Result.Length == 1 || Result[Length - 1] != 0;
    GetULong(Result) == us;    
  }  
}
```

## GetBytes(ushort, Span\<byte>)

```csharp
public static int GetBytes(ushort us, Span<byte> buffer)
{
  require
  {
    GetByteCount(us) <= buffer.Length; 
  }
  ensure
  {
    0 < Result <= MaxByteCount; 
    buffer.SequenceEqual(GetBytes(us));
  }  
}
```

## GetInt(IEnumerable<byte>)

```csharp
public static int GetInt(IEnumerable<byte> bytes)
{
  require
  {
    bytes != null;
  }
  throws
  {
    OverflowException;
  }
}
```

## GetInt(Span<byte>, out int)

```csharp
public static int GetInt(Span<byte> bytes, out int n)
{
  require
  {
    !bytes.IsEmpty;
  }
  ensure
  {
    Result == GetByteCount(n);
  }
  throws
  {
    IndexOutOfRangeException;
    OverflowException;
  }
}
```

## GetLong(IEnumerable\<byte>)

```csharp
public static long GetLong(IEnumerable<byte> bytes)
{
  require
  {
    bytes != null;
  }
}
```

## GetLong(Span\<byte>, out long)

```csharp
public static int GetLong(Span<byte> bytes, out long val)
{
  require
  {
    !bytes.IsEmpty;
  }
  ensure
  {
    Result == GetByteCount(val);
  }
  throws
  {
    IndexOutOfRangeException;
  }
}
```

## GetShort(IEnumerable\<byte>)

```csharp
public static short GetShort(IEnumerable<byte> bytes)
{
  require
  {
    bytes != null;
  }
  throws
  {
    OverflowException;
  }
}
```

## GetShort(Span\<byte>, out short)

```csharp
public static int GetShort(Span<byte> bytes, out short val)
{
  require
  {
    !bytes.IsEmpty;
  }
  ensure
  {
    Result == GetByteCount(val);
  }
  throws
  {
    IndexOutOfRangeException;
    OverflowException;
  }
}
```

## GetUInt(IEnumerable\<byte>)

```csharp
public static uint GetUInt(IEnumerable<byte> bytes)
{
  require
  {
    bytes != null;
  }
  throws
  {
    OverflowException;
  }
}
```

## GetUInt(Span\<byte>, out uint)

```csharp
public static int GetUInt(Span<byte> bytes, out uint val)
{
  require
  {
    !bytes.IsEmpty;
  }
  ensure
  {
    Result == GetByteCount(val);
  }
  throws
  {
    IndexOutOfRangeException;
    OverflowException;
  }
}
```

## GetULong(IEnumerable\<byte>)

```csharp
public static ulong GetULong(IEnumerable<byte> bytes)
{
  require
  {
    bytes != null;
  }
}
```

## GetULong(Span\<byte>, out ulong)

```csharp
public static int GetULong(Span<byte> bytes, out ulong val)
{
  require
  {
    !bytes.IsEmpty;
  }
  ensure
  {
    Result == GetByteCount(val);
  }
  throws
  {
    IndexOutOfRangeException;
  }
}
```

## GetUShort(IEnumerable\<byte>)

```csharp
public static short GetUShort(IEnumerable<byte> bytes)
{
  require
  {
    bytes != null;
  }
  throws
  {
    OverflowException;
  }
}
```

## GetUShort(Span\<byte>, out ushort)

```csharp
public static int GetUShort(Span<byte> bytes, out ushort val)
{
  require
  {
    !bytes.IsEmpty;
  }
  ensure
  {
    Result == GetByteCount(val);
  }
  throws
  {
    IndexOutOfRangeException;
    OverflowException;
  }
}
```