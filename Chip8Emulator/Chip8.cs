using Chip8Emulator.Extensions;

namespace Chip8Emulator;

public class Chip8
{
    private const int F = 15;

    private readonly IDebugger _debugger;
    private readonly Func<int> _randomNumber;
    private readonly DigitSprites _digitSprites;
    
    public int I { get; private set; }
    public int[] V { get; }
    public int PC { get; private set; }
    
    public Stack<int> Stack { get; }
    public byte[] Memory { get; }

    public byte DelayTimer;
    public byte SoundTimer;
    private readonly Display _display;

    public Chip8(int[] v, int pc, Func<int> randomNumber, IDebugger debugger)
    {
        V = v;
        PC = pc;
        Stack = new Stack<int>();
        Memory = new byte[4096];
        
        _randomNumber = randomNumber;
        _debugger = debugger;
        _display = new Display();
        _digitSprites = new DigitSprites();
        _digitSprites.CopyTo(Memory);
    }

    public string Display()
    {
        return _display.ToString();
    }

    public void Process(short instruction)
    {
        if (instruction.Matches("00E0"))
        {
            _display.Clear();
        }
        
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
        
        if (instruction.Matches("D..."))
        {
            var (upperByte, lowerByte) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();
            var y = lowerByte.UpperNibble();
            var n = lowerByte.LowerNibble();

            var sprite = new byte[n];

            for (var i = 0; i < n; i++)
            {
                sprite[i] = Memory[I + i];
            } 
            
            _display.DrawSpriteAt(x, y, sprite);
        }
        
        if (instruction.Matches("F.1E"))
        {
            var (upperByte, _) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            I += V[x];
        }
        
        if (instruction.Matches("F.29"))
        {
            var (upperByte, _) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            I = _digitSprites.MemoryLocationFor(x);
        }
        
        if (instruction.Matches("F.33"))
        {
            var (upperByte, _) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            var registerParts = V[x]
                .ToString("D3")
                .Select(char.GetNumericValue)
                .Select(Convert.ToByte)
                .ToList();

            var hundreds = registerParts[0];
            var tens = registerParts[1];
            var ones = registerParts[2];
            
            Memory[I] = hundreds;
            Memory[I + 1] = tens;
            Memory[I + 2] = ones;
        }
        
        if (instruction.Matches("F.55"))
        {
            var (upperByte, _) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            for (var register = 0; register <= x; register++)
            {
                Memory[I + register] = Convert.ToByte(V[register]);
            }
        }
        
        if (instruction.Matches("F.65"))
        {
            var (upperByte, _) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble();

            for (var register = 0; register <= x; register++)
            {
                V[register] = Memory[I + register];
            }
        }
    }
}