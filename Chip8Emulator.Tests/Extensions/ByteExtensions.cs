namespace Chip8Emulator.Tests.Extensions;

public static class ByteExtensions
{
   public static string ToBinaryString(this byte @byte) {
     return Convert.ToString(@byte, 2).PadLeft(8, '0'); 
   }
}