using System.Text.RegularExpressions;

namespace Chip8Emulator.Extensions;

public static class ShortExtensions
{
    public static Tuple<byte, byte> UpperAndLowerBytes(this short @short)
    {
        var instructionBytes = BitConverter.GetBytes(@short).Reverse().ToArray();
        var upperByte = instructionBytes[0];
        var lowerByte = instructionBytes[1];
        return new Tuple<byte, byte>(upperByte, lowerByte);
    }

    public static Tuple<int, int> MiddleTwoNibbles(this short @short)
    {
        var (upperByte, lowerByte) = @short.UpperAndLowerBytes();
        return new Tuple<int, int>(upperByte.LowerNibble(), lowerByte.UpperNibble());
    }

    public static int Lower12Bits(this short @short) => @short & 0x0FFF;
    // public static int Lower12Bits(this short @short) => @short & 0xFF;
    
    public static bool Matches(this short @short, string pattern) 
        => Regex.IsMatch(@short.ToHexString(), pattern);

    public static string ToHexString(this short instruction)
    {
        var instructionBytesFromShort = BitConverter
            .GetBytes(instruction)
            .Reverse()
            .ToArray();

        return instructionBytesFromShort.ToHex();
    }
}