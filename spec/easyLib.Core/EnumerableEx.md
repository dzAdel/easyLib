# class EnumerableEx
```csharp
//ver 1
static class EnumerableEx
{
  public static IEnumerable<T> Emit<T>(Func<T, T> generate, T initValue, T stopValue);
  public static bool All<T>(this IEnumerable<T> src, Func<T, int, bool> predicate);
  public static bool IsSorted<T>(this IEnumerable<T> src, Comparison<T>? compare = null);
  public static bool IsOrdered<T>(this IEnumerable<T> src, Func<T, T, bool> precedes);
  public static int IndexOf<T>(this IEnumerable<T> src, T item, Func<T, T, bool>? equals = null);
  public static (T min, T max) MinMax<T>(this IEnumerable<T> src, Comparison<T>? compare = null);
  public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> src);  
}
```
## All<T>(this IEnumerable<T>, Func<T, int, bool>)
```csharp
public static bool All<T>(this IEnumerable<T> src, Func<T, int, bool> predicate)
{
  require
  {
    src != null;
    predicate != null;
  }
  ensure
  {
    src.Any() || Result;
  }
}
```
## Emit<T>(Func<T, T>, T, T)
```csharp
public static IEnumerable<T> Emit<T>(Func<T, T> generate, T initValue, T stopValue)
{
  require
  {
    generate != null;
  }
  ensure
  {
    Result.Any() == (initValue != stopValue);
    initValue == stopValue || Result.First() == initValue;
    !Result.Contains(stopValue);
  }
}
```
## IndexOf<T>(this IEnumerable<T>, T, Func\<T, T, bool>?)
```csharp
public static int IndexOf<T>(this IEnumerable<T> src, T item, Func<T, T, bool>? equals = null)
{
  require
  {
    src != null;
  }
  ensure
  {
    src.Any() || Result < 0;
    Result < 0 || src.ElementAt(Result) == item;
    Result < 0 || !src.Take(Result).Where(e => e == item).Any();
  }
}
```
## IsOrdered<T>(this IEnumerable<T>, Func<T, T, bool>)
```csharp
public static bool IsOrdered<T>(this IEnumerable<T> src, Func<T, T, bool> precedes)
{
  require
  {
    src != null;
    precedes != null;
  }
  ensure
  {
    src.Count() > 1 || Result;
    !Result || src.Select((e, ndx) => (e, ndx)).
      Skip(1).
      All(p => !precedes(src.ElementAt(p.ndx - 1), p.e));
  }
}
```
## IsSorted<T>(this IEnumerable<T>, Comparison<T>?)
```csharp
public static bool IsSorted<T>(this IEnumerable<T> src, Comparison<T>? compare = null)
{
  require
  {
    src != null;
    compare != null || typeof(T).Implements<IComparable<T>>();
    compare != null || typeof(T).Implements<IComparable>();
  }
  ensure
  {
    src.Count() > 1 || Result; 
  }
}
```
## MinMax<T>(this IEnumerable<T>, Comparison<T>?)
```csharp
public static (T min, T max) MinMax<T>(this IEnumerable<T> src, Comparison<T>? compare = null)
{
  require
  {
    src != null;
    src.Any();
    compare != null || typeof(T).Implements<IComparable<T>>();
    compare != null || typeof(T).Implements<IComparable>();    
  }
  ensure
  {
    Result.min != Result.Max || src.All(e => e == Result.min);
    Result.min == src.Min(compare);
    Result.Max == src.Max(compare);
  }
}
```
## Shuffle<T>(this IEnumerable<T>)
```csharp
public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> src)
{
  require
  {
    src != null;
  }
  ensure
  {
    Result.Count() == src.Count();
  }
}
```