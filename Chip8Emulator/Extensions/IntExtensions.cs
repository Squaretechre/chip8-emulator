namespace Chip8Emulator.Extensions;

public static class IntExtensions
{
    public static string Hex(this int @int)
    {
        return @int.ToString("X");
    } 
}