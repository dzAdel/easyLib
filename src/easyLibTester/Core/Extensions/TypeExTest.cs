using easyLib.Extensions;
using easyLib.Test;

namespace easyLibTester.Core.Extensions;

sealed class TypeExTest : UnitTest<ISimpleTypeTraits>
{
    public TypeExTest() :
        base(nameof(TypeExTest))
    { }

    //protected:
    protected override IInvariantTester DefineInvariant(ISimpleTypeTraits obj,
                                                        IInvariantTester invTester)
        => invTester[!obj.IsFloatingPoint || obj.IsNumeric]
                    [!obj.IsIntegral || obj.IsNumeric]
                    [!(obj.IsFloatingPoint && obj.IsIntegral)];

    protected override void Start()
    {
        TestImplements();
        TestIsSimpleType();
        TestIsNumericType();
        TestIsFloatingPointType();
        TestIsIntegralType();
        TestGetNumericTypeTraits();
        TestGetSimpleTypeTraits();
    }

    //private:
    void TestGetSimpleTypeTraits()
    {
        ISimpleTypeTraits simpTraits = typeof(byte).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == 1);

        simpTraits = typeof(sbyte).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == 1);

        simpTraits = typeof(short).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(short));

        simpTraits = typeof(ushort).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(ushort));

        simpTraits = typeof(int).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(int));

        simpTraits = typeof(uint).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(int));

        simpTraits = typeof(long).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(long));

        simpTraits = typeof(ulong).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(ulong));

        simpTraits = typeof(float).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(!simpTraits.IsIntegral);
        Ensure(simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(float));

        simpTraits = typeof(double).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(!simpTraits.IsIntegral);
        Ensure(simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(double));

        simpTraits = typeof(decimal).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(!simpTraits.IsIntegral);
        Ensure(simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(decimal));

        simpTraits = typeof(nint).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == nint.Size);

        simpTraits = typeof(nuint).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(simpTraits.IsNumeric);
        Ensure(simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == nuint.Size);

        simpTraits = typeof(bool).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(!simpTraits.IsNumeric);
        Ensure(!simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == 1);

        simpTraits = typeof(char).GetSimpleTypeTraits();
        TestInvariant(simpTraits);
        Ensure(!simpTraits.IsNumeric);
        Ensure(!simpTraits.IsIntegral);
        Ensure(!simpTraits.IsFloatingPoint);
        Ensure(simpTraits.Size == sizeof(char));
    }

    void TestGetNumericTypeTraits()
    {
        INumericTypeTraits numTraits = typeof(byte).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(!numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == 1);

        numTraits = typeof(sbyte).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == 1);

        numTraits = typeof(short).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(short));

        numTraits = typeof(ushort).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(!numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(ushort));

        numTraits = typeof(int).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(int));

        numTraits = typeof(uint).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(!numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(int));

        numTraits = typeof(long).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(long));

        numTraits = typeof(ulong).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(!numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(ulong));

        numTraits = typeof(float).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(!numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(float));

        numTraits = typeof(double).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(!numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(double));

        numTraits = typeof(decimal).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(!numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == sizeof(decimal));

        numTraits = typeof(nint).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == nint.Size);

        numTraits = typeof(nuint).GetNumericTypeTraits();
        Ensure(numTraits.IsNumeric);
        Ensure(numTraits.IsIntegral);
        Ensure(!numTraits.IsSigned);
        Ensure(!numTraits.IsFloatingPoint);
        Ensure(numTraits.Size == nuint.Size);
    }

    void TestIsIntegralType()
    {
        Ensure(typeof(byte).IsIntegralType());
        Ensure(typeof(sbyte).IsIntegralType());
        Ensure(typeof(short).IsIntegralType());
        Ensure(typeof(ushort).IsIntegralType());
        Ensure(typeof(int).IsIntegralType());
        Ensure(typeof(uint).IsIntegralType());
        Ensure(typeof(long).IsIntegralType());
        Ensure(typeof(ulong).IsIntegralType());
        Ensure(!typeof(float).IsIntegralType());
        Ensure(!typeof(double).IsIntegralType());
        Ensure(!typeof(decimal).IsIntegralType());
        Ensure(typeof(nint).IsIntegralType());
        Ensure(typeof(nuint).IsIntegralType());
        Ensure(!typeof(bool).IsIntegralType());
        Ensure(!typeof(char).IsIntegralType());
        Ensure(!typeof(DateTime).IsIntegralType());
        Ensure(!typeof(string).IsIntegralType());
    }

    void TestIsFloatingPointType()
    {
        Ensure(!typeof(byte).IsFloatingPointType());
        Ensure(!typeof(sbyte).IsFloatingPointType());
        Ensure(!typeof(short).IsFloatingPointType());
        Ensure(!typeof(ushort).IsFloatingPointType());
        Ensure(!typeof(int).IsFloatingPointType());
        Ensure(!typeof(uint).IsFloatingPointType());
        Ensure(!typeof(long).IsFloatingPointType());
        Ensure(!typeof(ulong).IsFloatingPointType());
        Ensure(typeof(float).IsFloatingPointType());
        Ensure(typeof(double).IsFloatingPointType());
        Ensure(typeof(decimal).IsFloatingPointType());
        Ensure(!typeof(nint).IsFloatingPointType());
        Ensure(!typeof(nuint).IsFloatingPointType());
        Ensure(!typeof(bool).IsFloatingPointType());
        Ensure(!typeof(char).IsFloatingPointType());
        Ensure(!typeof(DateTime).IsFloatingPointType());
        Ensure(!typeof(string).IsFloatingPointType());
    }

    void TestIsNumericType()
    {
        Ensure(typeof(byte).IsNumericType());
        Ensure(typeof(sbyte).IsNumericType());
        Ensure(typeof(short).IsNumericType());
        Ensure(typeof(ushort).IsNumericType());
        Ensure(typeof(int).IsNumericType());
        Ensure(typeof(uint).IsNumericType());
        Ensure(typeof(long).IsNumericType());
        Ensure(typeof(ulong).IsNumericType());
        Ensure(typeof(float).IsNumericType());
        Ensure(typeof(double).IsNumericType());
        Ensure(typeof(decimal).IsNumericType());
        Ensure(typeof(nint).IsNumericType());
        Ensure(typeof(nuint).IsNumericType());
        Ensure(!typeof(bool).IsNumericType());
        Ensure(!typeof(char).IsNumericType());
        Ensure(!typeof(DateTime).IsNumericType());
        Ensure(!typeof(string).IsNumericType());
    }

    void TestIsSimpleType()
    {
        Ensure(typeof(byte).IsSimpleType());
        Ensure(typeof(sbyte).IsSimpleType());
        Ensure(typeof(short).IsSimpleType());
        Ensure(typeof(ushort).IsSimpleType());
        Ensure(typeof(int).IsSimpleType());
        Ensure(typeof(uint).IsSimpleType());
        Ensure(typeof(long).IsSimpleType());
        Ensure(typeof(ulong).IsSimpleType());
        Ensure(typeof(float).IsSimpleType());
        Ensure(typeof(double).IsSimpleType());
        Ensure(typeof(decimal).IsSimpleType());
        Ensure(typeof(nint).IsSimpleType());
        Ensure(typeof(nuint).IsSimpleType());
        Ensure(typeof(bool).IsSimpleType());
        Ensure(typeof(char).IsSimpleType());
        Ensure(!typeof(DateTime).IsSimpleType());
        Ensure(!typeof(string).IsSimpleType());
    }

    void TestImplements()
    {
        Ensure(typeof(int).Implements<IComparable<int>>());
        Ensure(typeof(int).Implements<IConvertible>());
        Ensure(!typeof(int).Implements<IEnumerable<int>>());

        Ensure(typeof(string).Implements<IComparable<string>>());
        Ensure(typeof(string).Implements<IConvertible>());
        Ensure(typeof(string).Implements<IEnumerable<char>>());
        Ensure(!typeof(string).Implements<IEnumerable<int>>());
    }
}

