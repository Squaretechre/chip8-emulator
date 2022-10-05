using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace Chip8Emulator;

public class Chip8
{
    private readonly ITestOutputHelper _testOutputHelper;
    public byte[] Memory { get; } = new byte[4096];

    public int[] V { get; }

    public byte DelayTimer;
    public byte SoundTimer;

    public Chip8(int[] v, int pc, ITestOutputHelper testOutputHelper)
    {
        V = v;
        PC = pc;
        Stack = new Stack<int>();
        _testOutputHelper = testOutputHelper;
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

            var register = LowerNibbleOf(upperByte);

            if (V[register] != valueToCompare) return;

            PC += 2;
        }

        if (Regex.IsMatch(instructionHex, "4..."))
        {
            var (upperByte, valueToCompare) = UpperAndLowerBytesOf(instruction);

            var register = LowerNibbleOf(upperByte);

            if (V[register] == valueToCompare) return;

            PC += 2;
        }

        if (Regex.IsMatch(instructionHex, "5..0"))
        {
            var (register1, register2) = MiddleTwoNibblesOf(instruction);

            if (V[register1] != V[register2]) return;

            PC += 2;
        }

        if (Regex.IsMatch(instructionHex, "6..."))
        {
            var (upperByte, value) = UpperAndLowerBytesOf(instruction);

            var register = LowerNibbleOf(upperByte);

            V[register] = value;
        }

        if (Regex.IsMatch(instructionHex, "7..."))
        {
            var (upperByte, value) = UpperAndLowerBytesOf(instruction);

            var register = LowerNibbleOf(upperByte);

            V[register] += value;
        }

        if (Regex.IsMatch(instructionHex, "8..0"))
        {
            var (registerToAssign, registerWithValue) = MiddleTwoNibblesOf(instruction);

            V[registerToAssign] = V[registerWithValue];
        }

        if (Regex.IsMatch(instructionHex, "8..1"))
        {
            var (registerToAssign, registerToOrWith) = MiddleTwoNibblesOf(instruction);

            V[registerToAssign] |= V[registerToOrWith];
        }

        if (Regex.IsMatch(instructionHex, "8..2"))
        {
            var (registerToAssign, registerToAndWith) = MiddleTwoNibblesOf(instruction);

            V[registerToAssign] &= V[registerToAndWith];
        }

        if (Regex.IsMatch(instructionHex, "8..3"))
        {
            var (registerToAssign, registerToXorWith) = MiddleTwoNibblesOf(instruction);

            V[registerToAssign] ^= V[registerToXorWith];
        }

        if (Regex.IsMatch(instructionHex, "8..4"))
        {
            var (register1, register2) = MiddleTwoNibblesOf(instruction);

            var addResult = V[register1] + V[register2];

            V[15] = addResult > 255 ? 1 : 0;

            V[register1] = addResult & 0xFF;
        }

        if (Regex.IsMatch(instructionHex, "8..5"))
        {
            var (registerToAssign, registerToSubtract) = MiddleTwoNibblesOf(instruction);

            V[15] = V[registerToAssign] > V[registerToSubtract] ? 1 : 0;

            V[registerToAssign] -= V[registerToSubtract];
        }
        
        if (Regex.IsMatch(instructionHex, "8..6"))
        {
            var (register, _) = MiddleTwoNibblesOf(instruction);

            V[15] = (V[register] & 0x01) == 1 ? 1 : 0;

            V[register] /= 2;
        }
        
        if (Regex.IsMatch(instructionHex, "8..7"))
        {
            var (registerX, registerY) = MiddleTwoNibblesOf(instruction);

            V[15] = V[registerY] > V[registerX] ? 1 : 0;

            V[registerX] = V[registerY] - V[registerX];
        }

        if (Regex.IsMatch(instructionHex, "A..."))
        {
            I = Lower12BitsOf(instruction);
        }

        if (Regex.IsMatch(instructionHex, "B..."))
        {
            PC = Lower12BitsOf(instruction) + V[0];
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