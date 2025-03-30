# Class SpanEx
```csharp
//ver 1
static class SpanEx
{
  public static void ReverseSlice(this Span<byte> bytes, int szSlice);
  public static IEnumerable<T> ToEnumerable<T>(this Span<T> span);
}
```
## ReverseSlice(this Span\<byte>, int)
```csharp
public static void ReverseSlice(this Span<byte> bytes, int szSlice)
{
  require
  {
    szSlice > 0;
    bytes.Length % szSlice == 0;
  }
}
```
## ToEnumerable\<T>(this Span\<T>)
```csharp
public static IEnumerable<T> ToEnumerable<T>(this Span<T> span)
{
  ensure
  {
    span.ToArray().SequnceEqual(Result);
  }
}
```