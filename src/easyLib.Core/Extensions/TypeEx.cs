namespace easyLib.Extensions;

public static partial class TypeEx
{
    public static bool Implements<T>(this Type type)
    {
        require(type != null);
        require(typeof(T).IsInterface);

        return type.IsAssignableTo(typeof(T));
    }

    public static bool IsSimpleType(this Type type)
    {
        require(type != null);

        return type == typeof(decimal) || type.IsPrimitive;
    }

    public static bool IsNumericType(this Type type)
    {
        require(type != null);

        return type == typeof(decimal) || (type.IsPrimitive && type != typeof(bool) && type != typeof(char));
    }

    public static bool IsFloatingPointType(this Type type)
    {
        require(type != null);

        return type == typeof(double) || type == typeof(float) || type == typeof(decimal);
    }

    public static bool IsIntegralType(this Type type)
    {
        require(type != null);

        return IsNumericType(type) && !IsFloatingPointType(type);
    }

    public static INumericTypeTraits GetNumericTypeTraits(this Type type)
    {
        require(type != null);
        require(type.IsNumericType());

        if (type == typeof(byte))
            return new IntegralTypeTraits(sizeof(byte), false);

        if (type == typeof(sbyte))
            return new IntegralTypeTraits(sizeof(sbyte), true);

        if (type == typeof(short))
            return new IntegralTypeTraits(sizeof(short), true);

        if (type == typeof(ushort))
            return new IntegralTypeTraits(sizeof(ushort), false);

        if (type == typeof(int))
            return new IntegralTypeTraits(sizeof(int), true);

        if (type == typeof(uint))
            return new IntegralTypeTraits(sizeof(uint), false);

        if (type == typeof(long))
            return new IntegralTypeTraits(sizeof(long), true);

        if (type == typeof(ulong))
            return new IntegralTypeTraits(sizeof(ulong), false);

        if (type == typeof(float))
            return new FloatingPointTypeTraits(sizeof(float));

        if (type == typeof(double))
            return new FloatingPointTypeTraits(sizeof(double));

        if (type == typeof(decimal))
            return new FloatingPointTypeTraits(sizeof(decimal));

        if (type == typeof(nint))
            return new IntegralTypeTraits(IntPtr.Size, true);

        assert(type == typeof(nuint));

        return new IntegralTypeTraits(UIntPtr.Size, false);
    }

    public static ISimpleTypeTraits GetSimpleTypeTraits(this Type type)
    {
        require(type != null);
        require(type.IsSimpleType());

        return type == typeof(bool) ? new BooleanTypeTraits() :
            type == typeof(char) ? new CharTypeTraits() : GetNumericTypeTraits(type);
    }
}