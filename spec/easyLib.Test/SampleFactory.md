# class SampleFactory
```csharp
static class SampleFactory
{
  public static byte NextByte {get;}
  public static sbyte NextSByte {get;}
  public static short NextShort {get;}
  public static ushort NextUShort {get;}
  public static int NextInt {get;}
  public static uint NextUInt {get;}
  public static long NextLong {get;}
  public static ulong NextULong {get;}
  public static float NextFloat {get;}
  public static double NextDouble {get;}
  public static decimal NextDecimal {get;}
  public static bool NextBool {get;}
  public static char NextChar {get;}
  public static string NextString {get;}
  public static IEnumerable<byte> CreateBytes(byte min = byte.MinValue, byte limit = byte.MaxValue);
  public static IEnumerable<sbyte> CreateSBytes(sbyte min = sbyte.MinValue, sbyte limit = sbyte.MaxValue);
  public static IEnumerable<short> CreateShorts(short min = short.MinValue, short limit = short.MaxValue);
  public static IEnumerable<ushort> CreateUShorts(ushort min = ushort.MinValue, ushort limit = ushort.MaxValue);
  public static IEnumerable<int> CreateInts(int min = int.MinValue, int limit = int.MaxValue);
  public static IEnumerable<uint> CreateUInts(uint min = uint.MinValue, uint limit = uint.MaxValue);
  public static IEnumerable<long> CreateLongs(long min = long.MinValue, long limit = long.MaxValue);
  public static IEnumerable<ulong> CreateULongs(ulong min = ulong.MinValue, ulong limit = ulong.MaxValue);
  public static IEnumerable<float> CreateFloats(float min = float.MinValue, float limit = float.MaxValue);
  public static IEnumerable<double> CreateDoubles(double min = double.MinValue, double limit = double.MaxValue);
  public static IEnumerable<decimal> CreateDecimals(decimal min = decimal.MinValue, decimal limit = decimal.MaxValue);
  public static IEnumerable<bool> CreateBools();
  public static IEnumerable<char> CreateChars(char min = char.MinValue, char limit = '\ud800');
  public static IEnumerable<string> CreateStrings(int minLen = 0, int lenLimit = byte.MaxValue);
}
```
## CreateBytes(byte, byte)
```csharp
public static IEnumerable<byte> CreateBytes(byte min = byte.MinValue, byte limit = byte.MaxValue)
{
  require
  {
    min < limit;
  }
}
```
## CreateChars(char, char)
```csharp
public static IEnumerable<char> CreateChars(char min = char.MinValue, char limit = '\ud800')
{
  require
  {
    min < limit;
  }
}
```
## CreateDecimals(decimal, decimal)
```csharp
public static IEnumerable<decimal> CreateDecimals(decimal min = decimal.MinValue, decimal limit = decimal.MaxValue)
{
  require
  {
    min < limit;
  }
}
```
## CreateDoubles(double, double)
```csharp
public static IEnumerable<double> CreateDoubles(double min = double.MinValue, double limit = double.MaxValue)
{
  require
  {
    double.IsFinite(min);
    double.IsFinite(limit);
    min < limit;
  }
}
```
## CreateFloats(float, float)
```csharp
public static IEnumerable<float> CreateFloats(float min = float.MinValue, float limit = float.MaxValue)
{
  require
  {
    float.IsFinite(min);
    float.IsFinite(limit);
    min < limit;
  }  
}
```
## CreateInts(int, int)
```csharp
public static IEnumerable<int> CreateInts(int min = int.MinValue, int limit = int.MaxValue)
{
  require
  {
    min < limit;
  }
}
```
## CreateLongs(long, long)
```csharp
public static IEnumerable<long> CreateLongs(long min = long.MinValue, long limit = long.MaxValue)
{
  require
  {
    min < limit;
  }
}
```
## CreateSBytes(sbyte, sbyte)
```csharp
public static IEnumerable<sbyte> CreateSBytes(sbyte min = sbyte.MinValue, sbyte limit = sbyte.MaxValue)
{
  require
  {
    min < limit;
  }
} 
```
## CreateShorts(short, short)
```csharp
public static IEnumerable<short> CreateShorts(short min = short.MinValue, short limit = short.MaxValue)
{
  require
  {
    min < limit;
  }
} 
```
## CreateStrings(int)
```csharp
public static IEnumerable<string> CreateStrings(int minLen = 0, int lenLimit = byte.MaxValue)
{
  require
  {
    0 <= minLen < lenLimit;
  }
}
```
## CreateUInts(uint, uint)
```csharp
public static IEnumerable<uint> CreateUInts(uint min = uint.MinValue, uint limit = uint.MaxValue)
{
  require
  {
    min < limit;
  }
} 
```
## CreateULongs(ulong, ulong)
```csharp
public static IEnumerable<ulong> CreateULongs(ulong min = ulong.MinValue, ulong limit = ulong.MaxValue)
{
  require
  {
    min < limit;
  }
} 
```
## CreateUShorts(ushort, ushort)
```csharp
public static IEnumerable<ushort> CreateUShorts(ushort min = ushort.MinValue, ushort limit = ushort.MaxValue)
{
  require
  {
    min < limit;
  }
} 
```
## NextByte
```csharp
public static byte NextByte
{
  get
  {
    ensure
    {
      0 <= Result < byte.MaxValue;
    }
  }
}
```
## NextChar
```csharp
public static char NextChar
{
  get
  {
    ensure
    {
      !char.IsSurrogate(Result);
    }
  }
}
```
## NextDecimal
```csharp
public static decimal NextDecimal
{
  get
  {
    ensure
    {
      decimal.MinValue <= Result < decimal.MaxValue;
    }
  }
}
```
## NextDouble
```csharp
public static double NextDouble
public static byte NextByte
{
  get
  {
    ensure
    {
      double.IsFinite(Result);
      double.MinValue <= Result < double.MaxValue;
    }
  }
}
```
## NextFloat
```csharp
public static float NextFloat
{
  get
  {
    ensure
    {
      float.IsFinite(Result);
      float.MinValue <= Result < float.MaxValue;
    }
  }
}
```
## NextInt
```csharp
public static int NextInt
{
  get
  {
    ensure
    {
      int.MinValue <= Result < int.MaxValue;
    }
  }
}
```
## NextLong
```csharp
public static long NextLong
{
  get
  {
    ensure
    {
      long.MinValue <= Result < long.MaxValue;
    }
  }
}
```
## NextSByte
```csharp
public static sbyte NextSByte
{
  get
  {
    ensure
    {
      sbyte.MinValue <= Result < sbyte.MaxValue;
    }
  }
}
```
## NextShort
```csharp
public static short NextShort
{
  get
  {
    ensure
    {
      short.MinValue <= Result < short.MaxValue;
    }
  }
}
```
## NextString
```csharp
public static string NextString
{
  get
  {
    ensure
    {
      0 <= Result.Length < byte.MaxValue;
      Result.All(c => !char.IsSurrogate(c));
    }
  }
}
```
## NextUInt
```csharp
public static uint NextUInt
{
  get
  {
    ensure
    {
      0 <= Result < uint.MaxValue;
    }
  }
}
```
## NextULong
```csharp
public static ulong NextULong
{
  get
  {
    ensure
    {
      0 <= Result < ulong.MaxValue;
    }
  }
}
```
## NextUShort
```csharp
public static ushort NextUShort
{
  get
  {
    ensure
    {
      0 <= Result < ushort.MaxValue;
    }
  }
}
```