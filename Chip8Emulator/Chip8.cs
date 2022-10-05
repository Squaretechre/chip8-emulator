using Chip8Emulator.Extensions;

namespace Chip8Emulator;

public class Chip8
{
    private const int F = 15;

    private readonly Func<int> _randomNumber;
    private readonly IDebugger _debugger;
    
    public byte[] Memory { get; } = new byte[4096];

    public int[] V { get; }

    public byte DelayTimer;
    public byte SoundTimer;

    public Chip8(int[] v, int pc, Func<int> randomNumber, IDebugger debugger)
    {
        V = v;
        PC = pc;
        Stack = new Stack<int>();
        _randomNumber = randomNumber;
        _debugger = debugger;
        
        new DigitSprites().CopyTo(Memory);
    }

    public int I { get; private set; }

    public int PC { get; private set; }

    public Stack<int> Stack { get; }

    public void Process(short instruction)
    {
        if (instruction.Matches("00EE"))
        {
            PC = Stack.Pop();
        }

        if (instruction.Matches("1..."))
        {
            PC = instruction.Lower12Bits();
        }

        if (instruction.Matches("2..."))
        {
            Stack.Push(PC);
            PC = instruction.Lower12Bits();
        }

        if (instruction.Matches("3..."))
        {
            var (upperByte, valueToCompare) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            if (V[x] != valueToCompare) return;

            PC += 2;
        }

        if (instruction.Matches("4..."))
        {
            var (upperByte, valueToCompare) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            if (V[x] == valueToCompare) return;

            PC += 2;
        }

        if (instruction.Matches("5..0"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            if (V[x] != V[y]) return;

            PC += 2;
        }

        if (instruction.Matches("6..."))
        {
            var (upperByte, value) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            V[x] = value;
        }

        if (instruction.Matches("7..."))
        {
            var (upperByte, value) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            V[x] += value;
        }

        if (instruction.Matches("8..0"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            V[x] = V[y];
        }

        if (instruction.Matches("8..1"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            V[x] |= V[y];
        }

        if (instruction.Matches("8..2"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            V[x] &= V[y];
        }

        if (instruction.Matches("8..3"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            V[x] ^= V[y];
        }

        if (instruction.Matches("8..4"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            var addResult = V[x] + V[y];

            V[F] = addResult > 255 ? 1 : 0;

            V[x] = addResult & 0xFF;
        }

        if (instruction.Matches("8..5"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            V[F] = V[x] > V[y] ? 1 : 0;

            V[x] -= V[y];
        }
        
        if (instruction.Matches("8..6"))
        {
            var (x, _) = instruction.MiddleTwoNibbles();

            V[F] = (V[x] & 0x01) == 1 ? 1 : 0;

            V[x] /= 2;
        }
        
        if (instruction.Matches("8..7"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            V[F] = V[y] > V[x] ? 1 : 0;

            V[x] = V[y] - V[x];
        }
        
        if (instruction.Matches("8..E"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            V[F] = (V[x] >> 7) & 1;

            V[x] <<= 1;
        }
        
        if (instruction.Matches("9..0"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();

            if (V[x] == V[y]) return;

            PC += 2;
        }

        if (instruction.Matches("A..."))
        {
            I = instruction.Lower12Bits();
        }

        if (instruction.Matches("B..."))
        {
            PC = instruction.Lower12Bits() + V[0];
        }
        
        if (instruction.Matches("C..."))
        {
            var (upperByte, kk) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            V[x] = _randomNumber() & kk;
        }
    }
}