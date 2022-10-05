namespace Chip8Emulator;

public static class ByteExtensions
{
   public static string ToBinaryString(this byte @byte) {
     return Convert.ToString(@byte, 2).PadLeft(8, '0'); 
   }
}