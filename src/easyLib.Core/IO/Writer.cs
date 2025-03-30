namespace easyLib.IO;

public interface IWriter
{
    void WriteByte(byte b);
    void WriteSByte(sbyte sb);
    void WriteBool(bool b);
    void WriteChar(char c);
    void WriteShort(short s);
    void WriteUShort(ushort us);
    void WriteInt(int i);
    void WriteUInt(uint ui);
    void WriteLong(long l);
    void WriteULong(ulong ul);
    void WriteFloat(float f);
    void WriteDouble(double d);
    void WriteDecimal(decimal d);
    void WriteString(string str);
}
