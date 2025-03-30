# class ListEx
```csharp
//ver 1
public static class ListEx
{  
  public static void Put<T>(this IList<T> lst, T item, int ndx);
  public static void Put<T>(this IList<T> lst, IEnumerabe<T> items, int ndxDest);
  public static int IndexOf<T>(this IList<T> lst,
                               T item,
                               int ndxStart,
                               Func<T, T, bool>? equals = null);
}
```
## IndexOf\<T>(this IReadOnlyList\<T>, T, Func\<T, T, bool>?)
```csharp
public static int IndexOf<T>(this IReadOnlyList<T> lst, T elt, int ndxStart, 
                             Func<T, T, bool>? eql = null)
{
  require
  {
      lst != null;
      0 <= ndxStart < lst.Count;
  }
  ensure
  {
    Result < lst.Count;
    Result == -1 || Result >= ndxStart;
    Result == -1 || lst[Result] == elt;
  }
}
```
## Put\<T>(this IList\<T>, IEnumerabe\<T>, int)
```csharp
public static int Put<T>(this IList<T> lst, IEnumerabe<T> items, int ndxDest)
{
  require
  {
      lst != null;
      items != null;
      0 <= ndxDest <= lst.Count;
  }
  ensure
  {
    Result == items.Count();
    lst.Skip(ndxDest).Take(Result).SequenceEqual(items);
    lst.Count >= Result + ndxDest; 
  }
}
```
## Put\<T>(IList\<T>, T, int)
```csharp
public static void Put<T>(this IList<T> lst, T item, int ndx)
{
  require
  {
      lst != null;
      0 <= ndx <= lst.Count;
  }
  ensure
  {
      lst[ndx] == T;
      lst.Count > ndx;
  }
}
```
