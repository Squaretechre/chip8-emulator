using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace Chip8Emulator;

public class Chip8
{
    private const int F = 15;

    private readonly Func<int> _randomNumber;
    private readonly ITestOutputHelper _testOutputHelper;
    
    public byte[] Memory { get; } = new byte[4096];

    public int[] V { get; }

    public byte DelayTimer;
    public byte SoundTimer;

    public Chip8(int[] v, int pc, Func<int> randomNumber, ITestOutputHelper testOutputHelper)
    {
        V = v;
        PC = pc;
        Stack = new Stack<int>();
        _randomNumber = randomNumber;
        _testOutputHelper = testOutputHelper;
        
        new DigitSprites().CopyTo(Memory);
    }

    // This register is generally used to store memory addresses, so only the lowest (rightmost) 12 bits are usually used.
    public int I { get; private set; }

    public int PC { get; private set; }

    public Stack<int> Stack { get; }

    public void ProcessInstruction(short instruction)
    {
        var instructionHex = HexStringFor(instruction);

        if (Regex.IsMatch(instructionHex, "00EE"))
        {
            PC = Stack.Pop();
        }

        if (Regex.IsMatch(instructionHex, "1..."))
        {
            PC = Lower12BitsOf(instruction);
        }

        if (Regex.IsMatch(instructionHex, "2..."))
        {
            Stack.Push(PC);
            PC = Lower12BitsOf(instruction);
        }

        if (Regex.IsMatch(instructionHex, "3..."))
        {
            var (upperByte, valueToCompare) = UpperAndLowerBytesOf(instruction);

            var x = LowerNibbleOf(upperByte);

            if (V[x] != valueToCompare) return;

            PC += 2;
        }

        if (Regex.IsMatch(instructionHex, "4..."))
        {
            var (upperByte, valueToCompare) = UpperAndLowerBytesOf(instruction);

            var x = LowerNibbleOf(upperByte);

            if (V[x] == valueToCompare) return;

            PC += 2;
        }

        if (Regex.IsMatch(instructionHex, "5..0"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            if (V[x] != V[y]) return;

            PC += 2;
        }

        if (Regex.IsMatch(instructionHex, "6..."))
        {
            var (upperByte, value) = UpperAndLowerBytesOf(instruction);

            var x = LowerNibbleOf(upperByte);

            V[x] = value;
        }

        if (Regex.IsMatch(instructionHex, "7..."))
        {
            var (upperByte, value) = UpperAndLowerBytesOf(instruction);

            var x = LowerNibbleOf(upperByte);

            V[x] += value;
        }

        if (Regex.IsMatch(instructionHex, "8..0"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            V[x] = V[y];
        }

        if (Regex.IsMatch(instructionHex, "8..1"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            V[x] |= V[y];
        }

        if (Regex.IsMatch(instructionHex, "8..2"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            V[x] &= V[y];
        }

        if (Regex.IsMatch(instructionHex, "8..3"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            V[x] ^= V[y];
        }

        if (Regex.IsMatch(instructionHex, "8..4"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            var addResult = V[x] + V[y];

            V[F] = addResult > 255 ? 1 : 0;

            V[x] = addResult & 0xFF;
        }

        if (Regex.IsMatch(instructionHex, "8..5"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            V[F] = V[x] > V[y] ? 1 : 0;

            V[x] -= V[y];
        }
        
        if (Regex.IsMatch(instructionHex, "8..6"))
        {
            var (x, _) = MiddleTwoNibblesOf(instruction);

            V[F] = (V[x] & 0x01) == 1 ? 1 : 0;

            V[x] /= 2;
        }
        
        if (Regex.IsMatch(instructionHex, "8..7"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            V[F] = V[y] > V[x] ? 1 : 0;

            V[x] = V[y] - V[x];
        }
        
        if (Regex.IsMatch(instructionHex, "8..E"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            V[F] = (V[x] >> 7) & 1;

            V[x] <<= 1;
        }
        
        if (Regex.IsMatch(instructionHex, "9..0"))
        {
            var (x, y) = MiddleTwoNibblesOf(instruction);

            if (V[x] == V[y]) return;

            PC += 2;
        }

        if (Regex.IsMatch(instructionHex, "A..."))
        {
            I = Lower12BitsOf(instruction);
        }

        if (Regex.IsMatch(instructionHex, "B..."))
        {
            PC = Lower12BitsOf(instruction) + V[0];
        }
        
        if (Regex.IsMatch(instructionHex, "C..."))
        {
            var (upperByte, kk) = UpperAndLowerBytesOf(instruction);

            var x = LowerNibbleOf(upperByte);

            V[x] = _randomNumber() & kk;
        }
    }

    private static string HexStringFor(short instruction)
    {
        var instructionBytesFromShort = BitConverter
            .GetBytes(instruction)
            .Reverse()
            .ToArray();

        return BitConverter
            .ToString(instructionBytesFromShort, 0, 2)
            .Replace("-", "");
    }

    private static int Lower12BitsOf(short instruction) => instruction & 0xFFF;
    private static int UpperNibbleOf(byte @byte) => @byte >> 4;
    private static int LowerNibbleOf(byte @byte) => @byte & 0x0F;

    private static Tuple<byte, byte> UpperAndLowerBytesOf(short instruction)
    {
        var instructionBytes = BitConverter.GetBytes(instruction).Reverse().ToArray();
        var upperByte = instructionBytes[0];
        var lowerByte = instructionBytes[1];
        return new Tuple<byte, byte>(upperByte, lowerByte);
    }

    private static Tuple<int, int> MiddleTwoNibblesOf(short instruction)
    {
        var (upperByte, lowerByte) = UpperAndLowerBytesOf(instruction);
        return new Tuple<int, int>(LowerNibbleOf(upperByte), UpperNibbleOf(lowerByte));
    }
}