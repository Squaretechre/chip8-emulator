namespace Chip8Emulator;

public static class ShortExtensions
{
    public static string ToBinaryString(this short @short) {
        return Convert.ToString(@short, 2).PadLeft(16, '0'); 
    }
}