namespace easyLib.IO;

public interface IReader
{
    byte ReadByte();
    sbyte ReadSByte();
    bool ReadBool();
    char ReadChar();
    short ReadShort();
    ushort ReadUShort();
    int ReadInt();
    uint ReadUInt();
    long ReadLong();
    ulong ReadULong();
    float ReadFloat();
    double ReadDouble();
    decimal ReadDecimal();
    string ReadString();
}
