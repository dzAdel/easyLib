namespace easyLib.Extensions;

partial class TypeEx
{
    sealed class BooleanTypeTraits : ISimpleTypeTraits
    {
        public bool IsNumeric => false;
        public bool IsFloatingPoint => false;
        public bool IsIntegral => false;
        public int Size => sizeof(bool);
    }
    //-------------------------------------------------------------------

    sealed class CharTypeTraits : ISimpleTypeTraits
    {
        public bool IsNumeric => false;
        public bool IsFloatingPoint => false;
        public bool IsIntegral => false;
        public int Size => sizeof(char);
    }
    //---------------------------------------------------------------------

    sealed class FloatingPointTypeTraits : INumericTypeTraits
    {
        public FloatingPointTypeTraits(int sz) => Size = sz;

        public bool IsSigned => true;
        public bool IsNumeric => true;
        public bool IsFloatingPoint => true;
        public bool IsIntegral => false;
        public int Size { get; }
    }
    //------------------------------------------------------------------------

    sealed class IntegralTypeTraits : INumericTypeTraits
    {
        public IntegralTypeTraits(int sz, bool isSigned)
        {
            Size = sz;
            IsSigned = isSigned;
        }

        public bool IsSigned { get; }
        public bool IsNumeric => true;
        public bool IsFloatingPoint => false;
        public bool IsIntegral => true;
        public int Size { get; }
    }
}
