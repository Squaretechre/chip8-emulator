namespace Chip8Emulator.Extensions;

public static class ByteExtensions
{
    public static int UpperNibble(this byte @byte) => @byte >> 4;
    public static int LowerNibble(this byte @byte) => @byte & 0x0F;
}