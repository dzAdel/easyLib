# class ArrayPool
```csharp
public static class ArrayPool
{
  public static T[] Rent<T>(int minLen);
  public static void Return<T>(T[] array);
}
```
## Rent\<T\>(int)
```csharp
public static T[] Rent<T>(int minLen)
{
  require
  {
    0 <= minLen <= Array.MaxLength;
  }
  ensure
  {
    Result.Length >= minLen;
  }
}
```