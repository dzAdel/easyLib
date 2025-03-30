# interface ISimpleTypeTraits
```csharp
///ver 1
public interface ISimpleTypeTraits
{
  bool IsNumeric { get; }
  bool IsFloatingPoint { get; }
  bool IsIntegral { get; }
  int Size { get; }
}
```
## Invariant
```csharp
Invariant
{
  !IsFloatingPoint || IsNumeric;
  !IsIntegral || IsNumeric;
  !(IsFloatingPoint && IsIntegral);
}
```
# interface INumericTypeTraits
```csharp
//ver 1
public interface INumericTypeTraits: ISimpleTypeTraits
{
  bool IsSigned { get; }
}
```
# class TypeEx
```csharp
//ver 1
public static class TypeEx
{
  public static bool Implements<T>(this Type type);
  public static bool IsSimpleType(this Type type);
  public static bool IsNumericType(this Type type);
  public static bool IsFloatingPointType(this Type type);
  public static bool IsIntegralType(this Type type);
  public static INumericTypeTraits GetNumericTypeTraits(this Type type);
  public static ISimpleTypeTraits GetSimpleTypeTraits(this Type type);
}
```
## GetNumericTypeTraits(this Type)
```csharp
public static INumericTypeTraits GetNumericTypeTraits(this Type type)
{
  require
  {
    type != null;
    type.IsNumericType();
  }
}
```
## GetSimpleTypeTraits(this Type)
```csharp
public static ISimpleTypeTraits GetSimpleTypeTraits(this Type type)
{
  require
  {
    type != null;
    type.IsSimpleType();
  }
}
```
## Implements\<T>(this Type)
```csharp
public static bool Implements<T>(this Type type)
{
  require
  {
    type != null;
    typeof(T).IsInterface;
  }
}
```
## IsFloatingPointType(this Type)
```csharp
public static bool IsFloatingPointType(this Type type)
{
  require
  {
    type != null;
  }
}
```
## IsIntegralType(this Type)
```csharp
public static bool IsIntegralType(this Type type)
{
  require
  {
    type != null;
  }
}
```
## IsNumericType(this Type)
```csharp
public static bool IsNumericType(this Type type)
{
  require
  {
    type != null;
  }
}
```
## IsSimpleType(this Type)
```csharp
public static bool IsSimpleType(this Type type)
{
  require
  {
    type != null;
  }
}
```