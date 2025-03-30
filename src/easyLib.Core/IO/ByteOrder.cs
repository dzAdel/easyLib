namespace easyLib.IO;

public enum ByteOrder : byte
{
    System,
    LittleEndian,
    BigEndian,
    Network = BigEndian
}
//------------------------------------

public static class ByteOrderEx
{
    public static ByteOrder Normalize(this ByteOrder endianness)
    {
        require(Enum.IsDefined(endianness));

        return endianness == ByteOrder.System ?
            (BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian) :
            endianness;
    }

    public static bool SameAs(this ByteOrder bo1, ByteOrder bo2)
    {
        require(Enum.IsDefined(bo1));
        require(Enum.IsDefined(bo2));

        return bo1.Normalize() == bo2.Normalize();
    }
}

