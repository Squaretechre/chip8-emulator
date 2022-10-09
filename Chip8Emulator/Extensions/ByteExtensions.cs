namespace Chip8Emulator.Extensions;

public static class ByteExtensions
{
    public static int UpperNibble(this byte @byte) => @byte >> 4;
    public static int LowerNibble(this byte @byte) => @byte & 0x0F;

    public static string ToHex(this byte[] bytes) 
        => BitConverter.ToString(bytes, 0, 2).Replace("-", "");
    
    public static string[] ToHexArray(this byte[] bytes) 
        => bytes.Select(x => x.ToString("X2")).ToArray();
}