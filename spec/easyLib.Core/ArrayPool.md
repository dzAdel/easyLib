# class ArrayPool
```csharp
public static class ArrayPool
{
  public static T[] Alloc<T>(int minLen);
  public static void Free<T>(T[]? array);
}
```
## Alloc< T >(int)
```csharp
public static T[] Alloc<T>(int minLen)
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