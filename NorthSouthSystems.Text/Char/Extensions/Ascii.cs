namespace NorthSouthSystems.Text;

public static partial class CharExtensions
{
    public static bool IsAscii(this char value) => value >= 0x00 && value <= 0x7F;
    public static bool IsAsciiPrintable(this char value) => value >= 0x20 && value <= 0x7E;
}
