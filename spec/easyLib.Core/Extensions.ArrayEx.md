# class ArrayEx
```csharp
static class ArrayEx
{
	public static void ReverseSlice(this byte[] bytes,
		int szSlice, 
		int sliceCount, 
		int offset = 0); 
	public static ReadOnlySpan AsReadOnlySpan(this T[] array);
	public static ReadOnlySpan 	AsReadOnlySpan(this T[] array, int count, int ndxStart = 0);
}
```
## AsReadOnlySpan< T >(this T[])
```csharp
public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array)
{
  require
  {
    array != null;
  }
  ensure
  {
    Result.SequenceEqual(array);
  }
}
```
## AsReadOnlySpan< T >(this T[], int, int)
```csharp
public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array, int count, int ndxStart = 0)
{
  require
  {
    array != null;
    0 <= count <= array.Length;
    0 <= ndxStart < array.Length;
    count <= array.Length - ndxStart;
  }
  ensure
  {
    Result.Length == count;
    Result.AsEnumerable().SequenceEqual(array.Skip(ndxStart).Take(count));
  }
}
```
## ReverseSlice(this byte[], int, int, int)
```csharp
public static void ReverseSlice(this byte[] bytes, int szSlice, int sliceCount, int offset = 0)
{
  require
  {
    bytes != null;
    szSlice > 0;
    sliceCount >= 0;
    offset >= 0;
    szSlice * sliceCount <= bytes.Length - offset;
  }
  ensure
  {
    bytes.Skip(offset).Take(sliceCount * szSlice).
      Chunk(szSlice).
      Zip(old bytes.Skip(offset).Take(sliceCount * szSlice).Chunk(szSlice)).
      All(p => p.First.Reverse().SequenceEqual(p.Second));
  }
}
```