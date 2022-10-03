using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace Chip8Emulator;

public class Chip8Should
{
    public class Chip8
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public byte[] Memory { get; } = new byte[4096];

        // 16 general purpose 8-bit registers, usually referred to as Vx, where x is a hexadecimal digit (0 through F).
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
        
        // The program counter (PC) should be 16-bit, and is used to store the currently executing address.
        public int PC { get; private set; }

        // The stack pointer (SP) can be 8-bit, it is used to point to the topmost level of the stack.
        public byte SP { get;  }

        public Stack<int> Stack { get; }

        /*
            nnn or addr - A 12-bit value, the lowest 12 bits of the instruction
            n or nibble - A 4-bit value, the lowest 4 bits of the instruction
            x - A 4-bit value, the lower 4 bits of the high byte of the instruction
            y - A 4-bit value, the upper 4 bits of the low byte of the instruction
            kk or byte - An 8-bit value, the lowest 8 bits of the instruction
        */
        public void ReadInstruction(short instruction)
        {
            var instructionBytesFromShort = BitConverter.GetBytes(instruction).Reverse().ToArray();

            var instructionHexString = BitConverter
                .ToString(instructionBytesFromShort, 0, 2 )
                .Replace("-", "");
            
            if (Regex.IsMatch(instructionHexString, "00EE"))
            {
                PC = Stack.Pop();
            }
            if (Regex.IsMatch(instructionHexString, "1..."))
            {
                PC = instruction & 0xFFF;
            }
            if (Regex.IsMatch(instructionHexString, "2..."))
            {
                Stack.Push(PC);
                PC = instruction & 0xFFF;
            }
            if (Regex.IsMatch(instructionHexString, "3..."))
            {
                var instructionBytes = BitConverter.GetBytes(instruction).Reverse().ToArray();
                var upperByte = instructionBytes[0];
                var valueToCompare = instructionBytes[1];
                
                var register = upperByte & 0x0F;

                if (V[register] != valueToCompare) return;

                PC += 2;
            }
            if (Regex.IsMatch(instructionHexString, "A..."))
            {
                I = instruction & 0xFFF;
            }
            if (Regex.IsMatch(instructionHexString, "B..."))
            {
                PC = (instruction & 0xFFF) + V[0];
            }
        }
    }
    
    private readonly ITestOutputHelper _testOutputHelper;

    public Chip8Should(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact(DisplayName = "00EE - RET - Return from a subroutine.")]
    public void process_instruction_00ee()
    {
        const int initialProgramCounterLocation = 500;

        var callSubroutineInstruction = Convert.ToInt16("0x2326", 16);
        var returnFromSubroutineInstruction = Convert.ToInt16("0x00EE", 16);
        
        var sut = new Chip8(Array.Empty<int>(), initialProgramCounterLocation, _testOutputHelper);
        
        sut.ReadInstruction(callSubroutineInstruction);
        
        Assert.Equal(806, sut.PC);
        Assert.Equal(500, sut.Stack.Peek());

        sut.ReadInstruction(returnFromSubroutineInstruction);
       
        Assert.Equal(initialProgramCounterLocation, sut.PC);
        Assert.True(IsEmpty(sut.Stack));
    }

    [Fact(DisplayName = "1nnn - JP addr - Jump to location nnn.")]
    public void process_instruction_1nnn()
    {
        var instruction = Convert.ToInt16("0x1217", 16);

        var sut = new Chip8(Array.Empty<int>(), 500, _testOutputHelper);
        
        sut.ReadInstruction(instruction);
        
        Assert.Equal(535, sut.PC);
    }
    
    [Fact(DisplayName = "2nnn - CALL addr - Call subroutine at nnn.")]
    public void process_instruction_2nnn()
    {
        var instruction = Convert.ToInt16("0x2326", 16);
    
        var sut = new Chip8(Array.Empty<int>(), 520, _testOutputHelper);
        
        sut.ReadInstruction(instruction);
        
        Assert.Equal(806, sut.PC);
        Assert.Equal(520, sut.Stack.Peek());
    }
    
    [Fact(DisplayName = "3xkk - SE Vx, byte - Skip next instruction if Vx = kk. ✅ Positive.")]
    public void increment_the_program_counter_by_2_when_vx_matches_value_kk()
    {
        var registers = new int[15];
        
        registers[3] = 64;
        
        var instruction = Convert.ToInt16("0x3340", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ReadInstruction(instruction);
        
        Assert.Equal(502, sut.PC);
    }
    
    [Fact(DisplayName = "3xkk - SE Vx, byte - Skip next instruction if Vx = kk. ❌ Negative.")]
    public void not_increment_the_program_counter_by_2_when_vx_does_not_match_value_kk()
    {
        var registers = new int[15];
        
        registers[3] = 63;
        
        var instruction = Convert.ToInt16("0x3340", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ReadInstruction(instruction);
        
        Assert.Equal(500, sut.PC);
    }

    [Fact(DisplayName = "Annn - LD I, addr - Set I = nnn.")]
    public void process_instruction_annn()
    {
        var instruction = Convert.ToInt16("0xA2B4", 16);

        var sut = new Chip8(Array.Empty<int>(), 500, _testOutputHelper);
        
        sut.ReadInstruction(instruction);
        
        Assert.Equal(692, sut.I);
    }
    
    [Fact(DisplayName = "Bnnn - JP V0, addr - Jump to location nnn + V0.")]
    public void process_instruction_bnnn()
    {
        var registers = new int[15];
        
        registers[0] = 10;
        
        var instruction = Convert.ToInt16("0xB23F", 16);

        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ReadInstruction(instruction);
        
        Assert.Equal(585, sut.PC);
    }
    
    [Fact]
    public void print_memory()
    {
        var memory = new byte[4096];
       
        var gameInstructions = File.ReadAllBytes("../../../games/Airplane.ch8");
        
        gameInstructions.CopyTo(memory, 512);
        
        PrintMemoryBinary(memory);
    }

    [Fact]
    public void describe_instructions()
    {
        Assert.Equal("0nnn - SYS addr - Jump to a machine code routine at nnn.", DescribeInstruction("0111"));
        Assert.Equal("00E0 - CLS - Clear the display.", DescribeInstruction("00E0"));
        Assert.Equal("00EE - RET - Return from a subroutine.", DescribeInstruction("00EE"));
        Assert.Equal("1nnn - JP addr - Jump to location nnn.", DescribeInstruction("121C"));
        Assert.Equal("2nnn - CALL addr - Call subroutine at nnn.", DescribeInstruction("2334"));
        Assert.Equal("3xkk - SE Vx, byte - Skip next instruction if Vx = kk.", DescribeInstruction("3025"));
        Assert.Equal("4xkk - SNE Vx, byte - Skip next instruction if Vx != kk.", DescribeInstruction("4470"));
        Assert.Equal("5xy0 - SE Vx, Vy - Skip next instruction if Vx = Vy.", DescribeInstruction("5120"));
        Assert.Equal("6xkk - LD Vx, byte - Set Vx = kk.", DescribeInstruction("601A"));
        Assert.Equal("7xkk - ADD Vx, byte - Set Vx = Vx + kk.", DescribeInstruction("71FF"));
        Assert.Equal("8xy0 - LD Vx, Vy - Set Vx = Vy.", DescribeInstruction("80E0"));
        Assert.Equal("8xy1 - OR Vx, Vy - Set Vx = Vx OR Vy.", DescribeInstruction("80E1"));
        Assert.Equal("8xy2 - AND Vx, Vy - Set Vx = Vx AND Vy.", DescribeInstruction("80E2"));
        Assert.Equal("8xy3 - XOR Vx, Vy - Set Vx = Vx XOR Vy.", DescribeInstruction("80E3"));
        Assert.Equal("8xy4 - ADD Vx, Vy - Set Vx = Vx + Vy, set VF = carry.", DescribeInstruction("80E4"));
        Assert.Equal("8xy5 - SUB Vx, Vy - Set Vx = Vx - Vy, set VF = NOT borrow.", DescribeInstruction("80E5"));
        Assert.Equal("8xy6 - SHR Vx {, Vy} - Set Vx = Vx SHR 1.", DescribeInstruction("80E6"));
        Assert.Equal("8xy7 - SUBN Vx, Vy - Set Vx = Vy - Vx, set VF = NOT borrow.", DescribeInstruction("80E7"));
        Assert.Equal("8xyE - SHL Vx {, Vy} - Set Vx = Vx SHL 1.", DescribeInstruction("80EE"));
        Assert.Equal("9xy0 - SNE Vx, Vy - Skip next instruction if Vx != Vy.", DescribeInstruction("91C0"));
        Assert.Equal("Annn - LD I, addr - Set I = nnn.", DescribeInstruction("A2B4"));
        Assert.Equal("Bnnn - JP V0, addr - Jump to location nnn + V0.", DescribeInstruction("B000"));
        Assert.Equal("Cxkk - RND Vx, byte - Set Vx = random byte AND kk.", DescribeInstruction("C470"));
        Assert.Equal("Dxyn - DRW Vx, Vy, nibble - Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.", DescribeInstruction("D014"));
        Assert.Equal("Ex9E - SKP Vx - Skip next instruction if key with the value of Vx is pressed.", DescribeInstruction("E29E"));
        Assert.Equal("ExA1 - SKNP Vx - Skip next instruction if key with the value of Vx is not pressed.", DescribeInstruction("E7A1"));
        Assert.Equal("Fx07 - LD Vx, DT - Set Vx = delay timer value.", DescribeInstruction("F607"));
        Assert.Equal("Fx0A - LD Vx, K - Wait for a key press, store the value of the key in Vx.", DescribeInstruction("F60A"));
        Assert.Equal("Fx15 - LD DT, Vx - Set delay timer = Vx.", DescribeInstruction("F515"));
        Assert.Equal("Fx18 - LD ST, Vx - Set sound timer = Vx.", DescribeInstruction("F518"));
        Assert.Equal("Fx1E - ADD I, Vx - Set I = I + Vx.", DescribeInstruction("F41E"));
        Assert.Equal("Fx29 - LD F, Vx - Set I = location of sprite for digit Vx.", DescribeInstruction("F029"));
        Assert.Equal("Fx33 - LD B, Vx - Store BCD representation of Vx in memory locations I, I+1, and I+2.", DescribeInstruction("FA33"));
        Assert.Equal("Fx55 - LD [I], Vx - Store registers V0 through Vx in memory starting at location I.", DescribeInstruction("F255"));
        Assert.Equal("Fx65 - LD Vx, [I] - Read registers V0 through Vx from memory starting at location I.", DescribeInstruction("F265"));
    }

    private static string DescribeInstruction(string instruction)
    {
        if (Regex.IsMatch(instruction, "00E0"))
        {
            return "00E0 - CLS - Clear the display.";
        }
        if (Regex.IsMatch(instruction, "00EE"))
        {
            return "00EE - RET - Return from a subroutine.";
        }
        if (Regex.IsMatch(instruction, "0..."))
        {
            return "0nnn - SYS addr - Jump to a machine code routine at nnn.";
        }
        if (Regex.IsMatch(instruction, "1..."))
        {
            return "1nnn - JP addr - Jump to location nnn.";
        }
        if (Regex.IsMatch(instruction, "2..."))
        {
            return "2nnn - CALL addr - Call subroutine at nnn.";
        }
        if (Regex.IsMatch(instruction, "3..."))
        {
            return "3xkk - SE Vx, byte - Skip next instruction if Vx = kk.";
        }
        if (Regex.IsMatch(instruction, "4..."))
        {
            return "4xkk - SNE Vx, byte - Skip next instruction if Vx != kk.";
        }
        if (Regex.IsMatch(instruction, "5..0"))
        {
            return "5xy0 - SE Vx, Vy - Skip next instruction if Vx = Vy.";
        }
        if (Regex.IsMatch(instruction, "6..."))
        {
            return "6xkk - LD Vx, byte - Set Vx = kk.";
        }
        if (Regex.IsMatch(instruction, "7..."))
        {
            return "7xkk - ADD Vx, byte - Set Vx = Vx + kk.";
        }
        if (Regex.IsMatch(instruction, "8..0"))
        {
            return "8xy0 - LD Vx, Vy - Set Vx = Vy.";
        }
        if (Regex.IsMatch(instruction, "8..1"))
        {
            return "8xy1 - OR Vx, Vy - Set Vx = Vx OR Vy.";
        }
        if (Regex.IsMatch(instruction, "8..2"))
        {
            return "8xy2 - AND Vx, Vy - Set Vx = Vx AND Vy.";
        }
        if (Regex.IsMatch(instruction, "8..3"))
        {
            return "8xy3 - XOR Vx, Vy - Set Vx = Vx XOR Vy.";
        }
        if (Regex.IsMatch(instruction, "8..4"))
        {
            return "8xy4 - ADD Vx, Vy - Set Vx = Vx + Vy, set VF = carry.";
        }
        if (Regex.IsMatch(instruction, "8..5"))
        {
            return "8xy5 - SUB Vx, Vy - Set Vx = Vx - Vy, set VF = NOT borrow.";
        }
        if (Regex.IsMatch(instruction, "8..6"))
        {
            return "8xy6 - SHR Vx {, Vy} - Set Vx = Vx SHR 1.";
        }
        if (Regex.IsMatch(instruction, "8..7"))
        {
            return "8xy7 - SUBN Vx, Vy - Set Vx = Vy - Vx, set VF = NOT borrow.";
        }
        if (Regex.IsMatch(instruction, "8..E"))
        {
            return "8xyE - SHL Vx {, Vy} - Set Vx = Vx SHL 1.";
        }
        if (Regex.IsMatch(instruction, "8..E"))
        {
            return "8xyE - SHL Vx {, Vy} - Set Vx = Vx SHL 1.";
        }
        if (Regex.IsMatch(instruction, "9..0"))
        {
            return "9xy0 - SNE Vx, Vy - Skip next instruction if Vx != Vy.";
        }
        if (Regex.IsMatch(instruction, "A..."))
        {
            return "Annn - LD I, addr - Set I = nnn.";
        }
        if (Regex.IsMatch(instruction, "B..."))
        {
            return "Bnnn - JP V0, addr - Jump to location nnn + V0.";
        }
        if (Regex.IsMatch(instruction, "C..."))
        {
            return "Cxkk - RND Vx, byte - Set Vx = random byte AND kk.";
        }
        if (Regex.IsMatch(instruction, "D..."))
        {
            return "Dxyn - DRW Vx, Vy, nibble - Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.";
        }
        if (Regex.IsMatch(instruction, "E.9E"))
        {
            return "Ex9E - SKP Vx - Skip next instruction if key with the value of Vx is pressed.";
        }
        if (Regex.IsMatch(instruction, "E.A1"))
        {
            return "ExA1 - SKNP Vx - Skip next instruction if key with the value of Vx is not pressed.";
        }
        if (Regex.IsMatch(instruction, "F.07"))
        {
            return "Fx07 - LD Vx, DT - Set Vx = delay timer value.";
        }
        if (Regex.IsMatch(instruction, "F.0A"))
        {
            return "Fx0A - LD Vx, K - Wait for a key press, store the value of the key in Vx.";
        }
        if (Regex.IsMatch(instruction, "F.15"))
        {
            return "Fx15 - LD DT, Vx - Set delay timer = Vx.";
        }
        if (Regex.IsMatch(instruction, "F.18"))
        {
            return "Fx18 - LD ST, Vx - Set sound timer = Vx.";
        }
        if (Regex.IsMatch(instruction, "F.1E"))
        {
            return "Fx1E - ADD I, Vx - Set I = I + Vx.";
        }
        if (Regex.IsMatch(instruction, "F.29"))
        {
            return "Fx29 - LD F, Vx - Set I = location of sprite for digit Vx.";
        }
        if (Regex.IsMatch(instruction, "F.33"))
        {
            return "Fx33 - LD B, Vx - Store BCD representation of Vx in memory locations I, I+1, and I+2.";
        }
        if (Regex.IsMatch(instruction, "F.55"))
        {
            return "Fx55 - LD [I], Vx - Store registers V0 through Vx in memory starting at location I.";
        }
        if (Regex.IsMatch(instruction, "F.65"))
        {
            return "Fx65 - LD Vx, [I] - Read registers V0 through Vx from memory starting at location I.";
        }
        
        return "Unknown";
    }

    private void PrintMemoryHex(IReadOnlyList<byte> memory)
    {
        for (var i = 512; i < memory.Count; i += 2)
        {
            var instructionByte1 = memory[i];
            var instructionByte2 = memory[i + 1];

            var instruction = new[] { instructionByte1, instructionByte2 };

            var instructionHexString = BitConverter.ToString(instruction).Replace("-", "");

            _testOutputHelper.WriteLine(instructionHexString);
        }
    }

    private void PrintMemoryBinary(IReadOnlyList<byte> memory)
    {
        for (var i = 512; i < memory.Count; i += 2)
        {
            var instructionByte1 = memory[i];
            var instructionByte2 = memory[i + 1];

            var instructionBytes = new[] { instructionByte2, instructionByte1 };
            
            var instruction = BitConverter.ToInt16(instructionBytes, 0);
            var binary = Convert.ToString(instruction, 2).PadLeft(16, '0');

            _testOutputHelper.WriteLine(binary);
            
            var instructionBytesFromShort = BitConverter.GetBytes(instruction).Reverse().ToArray();

            var instructionHexString = BitConverter.ToString(instructionBytesFromShort, 0, 2 );
            
            _testOutputHelper.WriteLine(instructionHexString);
            _testOutputHelper.WriteLine(instructionHexString.Replace("-", ""));
            _testOutputHelper.WriteLine(DescribeInstruction(instructionHexString.Replace("-", "")));
            _testOutputHelper.WriteLine("");
        }
    }
    
    private static bool IsEmpty(Stack<int> stack) 
        => ExceptionMessage(() => stack.Peek()).Equals("Stack empty.");

    private static string ExceptionMessage(Action action) 
        => Record.Exception(action)!.Message;
}