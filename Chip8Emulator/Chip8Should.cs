using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace Chip8Emulator;

public class Chip8Should
{
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
            var instructionBytesFromShort = BitConverter
                .GetBytes(instruction)
                .Reverse()
                .ToArray();

            var instructionHexString = BitConverter
                .ToString(instructionBytesFromShort, 0, 2 )
                .Replace("-", "");
            
            if (Regex.IsMatch(instructionHexString, "00EE"))
            {
                PC = Stack.Pop();
            }
            if (Regex.IsMatch(instructionHexString, "1..."))
            {
                PC = Lower12BitsOf(instruction);
            }
            if (Regex.IsMatch(instructionHexString, "2..."))
            {
                Stack.Push(PC);
                PC = Lower12BitsOf(instruction);
            }
            if (Regex.IsMatch(instructionHexString, "3..."))
            {
                var (upperByte, valueToCompare) = UpperAndLowerBytesOf(instruction);
                
                var register = LowerNibbleOf(upperByte);

                if (V[register] != valueToCompare) return;

                PC += 2;
            }
            if (Regex.IsMatch(instructionHexString, "4..."))
            {
                var (upperByte, valueToCompare) = UpperAndLowerBytesOf(instruction);
                
                var register = LowerNibbleOf(upperByte);

                if (V[register] == valueToCompare) return;

                PC += 2;
            }
            if (Regex.IsMatch(instructionHexString, "5..0"))
            {
                var (register1, register2) = MiddleTwoNibblesOf(instruction);
                
                if (V[register1] != V[register2]) return;
                
                PC += 2;
            }
            if (Regex.IsMatch(instructionHexString, "6..."))
            {
                var (upperByte, value) = UpperAndLowerBytesOf(instruction);

                var register = LowerNibbleOf(upperByte);
                
                V[register] = value;
            }
            if (Regex.IsMatch(instructionHexString, "7..."))
            {
                var (upperByte, value) = UpperAndLowerBytesOf(instruction);

                var register = LowerNibbleOf(upperByte);
                
                V[register] += value;
            }
            if (Regex.IsMatch(instructionHexString, "8..0"))
            {
                var (registerToAssign, registerWithValue) = MiddleTwoNibblesOf(instruction);
                
                V[registerToAssign] = V[registerWithValue];
            }
            if (Regex.IsMatch(instructionHexString, "8..1"))
            {
                var (registerToAssign, registerToOrWith) = MiddleTwoNibblesOf(instruction);

                V[registerToAssign] |= V[registerToOrWith];
            }
            if (Regex.IsMatch(instructionHexString, "8..2"))
            {
                var (registerToAssign, registerToAndWith) = MiddleTwoNibblesOf(instruction);

                V[registerToAssign] &= V[registerToAndWith];
            }
            if (Regex.IsMatch(instructionHexString, "8..3"))
            {
                var (registerToAssign, registerToXorWith) = MiddleTwoNibblesOf(instruction);

                V[registerToAssign] ^= V[registerToXorWith];
            }
            if (Regex.IsMatch(instructionHexString, "8..4"))
            {
                var (register1, register2) = MiddleTwoNibblesOf(instruction);

                var addResult = V[register1] + V[register2];

                V[15] = addResult > 255 ? 1 : 0;
                
                V[register1] = addResult & 0xFF;
            }
            if (Regex.IsMatch(instructionHexString, "8..5"))
            {
                var (registerToAssign, registerToSubtract) = MiddleTwoNibblesOf(instruction);
                
                V[15] = V[registerToAssign] > V[registerToSubtract] ? 1 : 0;

                V[registerToAssign] -= V[registerToSubtract];
            }
            if (Regex.IsMatch(instructionHexString, "A..."))
            {
                I = Lower12BitsOf(instruction);
            }
            if (Regex.IsMatch(instructionHexString, "B..."))
            {
                PC = Lower12BitsOf(instruction) + V[0];
            }
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
        
        sut.ProcessInstruction(callSubroutineInstruction);
        
        Assert.Equal(806, sut.PC);
        Assert.Equal(500, sut.Stack.Peek());

        sut.ProcessInstruction(returnFromSubroutineInstruction);
       
        Assert.Equal(initialProgramCounterLocation, sut.PC);
        Assert.True(IsEmpty(sut.Stack));
    }

    [Fact(DisplayName = "1nnn - JP addr - Jump to location nnn.")]
    public void process_instruction_1nnn()
    {
        var instruction = Convert.ToInt16("0x1217", 16);

        var sut = new Chip8(Array.Empty<int>(), 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(535, sut.PC);
    }
    
    [Fact(DisplayName = "2nnn - CALL addr - Call subroutine at nnn.")]
    public void process_instruction_2nnn()
    {
        var instruction = Convert.ToInt16("0x2326", 16);
    
        var sut = new Chip8(Array.Empty<int>(), 520, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(806, sut.PC);
        Assert.Equal(520, sut.Stack.Peek());
    }
    
    [Fact(DisplayName = "3xkk - SE Vx, byte - Skip next instruction if Vx = kk. ✅ Positive.")]
    public void increment_the_program_counter_by_2_when_vx_matches_value_kk_for_instruction_3xkk()
    {
        var registers = new int[16];
        
        registers[3] = 64;
        
        var instruction = Convert.ToInt16("0x3340", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(502, sut.PC);
    }
    
    [Fact(DisplayName = "3xkk - SE Vx, byte - Skip next instruction if Vx = kk. ❌ Negative.")]
    public void not_increment_the_program_counter_by_2_when_vx_does_not_match_value_kk_for_instruction_3xkk()
    {
        var registers = new int[16];
        
        registers[3] = 63;
        
        var instruction = Convert.ToInt16("0x3340", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(500, sut.PC);
    }
    
    [Fact(DisplayName = "4xkk - SNE Vx, byte - Skip next instruction if Vx != kk. ✅ Positive.")]
    public void increment_the_program_counter_by_2_when_vx_does_not_matches_value_kk_for_instruction_4xkk()
    {
        var registers = new int[16];
        
        registers[9] = 25;
        
        var instruction = Convert.ToInt16("0x4918", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(502, sut.PC);
    }
    
    [Fact(DisplayName = "4xkk - SNE Vx, byte - Skip next instruction if Vx != kk. ❌ Negative.")]
    public void not_increment_the_program_counter_by_2_when_vx_does_match_value_kk_for_instruction_4xkk()
    {
        var registers = new int[16];
        
        registers[9] = 24;
        
        var instruction = Convert.ToInt16("0x4918", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(500, sut.PC);
    }
    
    [Fact(DisplayName = "5xy0 - SE Vx, Vy - Skip next instruction if Vx = Vy. ✅ Positive.")]
    public void increment_the_program_counter_by_2_when_vx_equals_vy_for_instruction_5xy0()
    {
        var registers = new int[16];
        
        registers[2] = 100;
        registers[10] = 100;
        
        var instruction = Convert.ToInt16("0x5A20", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(502, sut.PC);
    }
    
    [Fact(DisplayName = "5xy0 - SE Vx, Vy - Skip next instruction if Vx = Vy. ❌ Negative.")]
    public void not_increment_the_program_counter_by_2_when_vx_does_not_equal_vy_for_instruction_5xy0()
    {
        var registers = new int[16];
        
        registers[2] = 99;
        registers[10] = 100;
        
        var instruction = Convert.ToInt16("0x5A20", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(500, sut.PC);
    }
    
    [Fact(DisplayName = "6xkk - LD Vx, byte - Set Vx = kk.")]
    public void process_instruction_6xkk()
    {
        var registers = new int[16];

        var instructionForRegister0 = Convert.ToInt16("0x601A", 16);
        var instructionForRegister1 = Convert.ToInt16("0x6176", 16);
        var instructionForRegister2 = Convert.ToInt16("0x6207", 16);
        var instructionForRegister3 = Convert.ToInt16("0x6311", 16);
        var instructionForRegister4 = Convert.ToInt16("0x6428", 16);
        var instructionForRegister5 = Convert.ToInt16("0x6560", 16);
        var instructionForRegister6 = Convert.ToInt16("0x6635", 16);
        var instructionForRegister7 = Convert.ToInt16("0x6715", 16);
        var instructionForRegister8 = Convert.ToInt16("0x682E", 16);
        var instructionForRegister9 = Convert.ToInt16("0x6964", 16);
        var instructionForRegister10 = Convert.ToInt16("0x6A01", 16);
        var instructionForRegister11 = Convert.ToInt16("0x6B02", 16);
        var instructionForRegister12 = Convert.ToInt16("0x6C03", 16);
        var instructionForRegister13 = Convert.ToInt16("0x6D04", 16);
        var instructionForRegister14 = Convert.ToInt16("0x6E05", 16);
        var instructionForRegister15 = Convert.ToInt16("0x6F06", 16);
 
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instructionForRegister0);
        sut.ProcessInstruction(instructionForRegister1);
        sut.ProcessInstruction(instructionForRegister2);
        sut.ProcessInstruction(instructionForRegister3);
        sut.ProcessInstruction(instructionForRegister4);
        sut.ProcessInstruction(instructionForRegister5);
        sut.ProcessInstruction(instructionForRegister6);
        sut.ProcessInstruction(instructionForRegister7);
        sut.ProcessInstruction(instructionForRegister8);
        sut.ProcessInstruction(instructionForRegister9);
        sut.ProcessInstruction(instructionForRegister10);
        sut.ProcessInstruction(instructionForRegister11);
        sut.ProcessInstruction(instructionForRegister12);
        sut.ProcessInstruction(instructionForRegister13);
        sut.ProcessInstruction(instructionForRegister14);
        sut.ProcessInstruction(instructionForRegister15);
        
        Assert.Equal(26, sut.V[0]);
        Assert.Equal(118, sut.V[1]);
        Assert.Equal(7, sut.V[2]);
        Assert.Equal(17, sut.V[3]);
        Assert.Equal(40, sut.V[4]);
        Assert.Equal(96, sut.V[5]);
        Assert.Equal(53, sut.V[6]);
        Assert.Equal(21, sut.V[7]);
        Assert.Equal(46, sut.V[8]);
        Assert.Equal(100, sut.V[9]);
        Assert.Equal(1, sut.V[10]);
        Assert.Equal(2, sut.V[11]);
        Assert.Equal(3, sut.V[12]);
        Assert.Equal(4, sut.V[13]);
        Assert.Equal(5, sut.V[14]);
        Assert.Equal(6, sut.V[15]);
    }
    
    [Fact(DisplayName = "7xkk - ADD Vx, byte - Set Vx = Vx + kk.")]
    public void process_instruction_7xkk()
    {
        var registers = new int[16];
        
        registers[0] = 10;
        registers[10] = 20;
        registers[15] = 30;
        
        var add5ToRegister0 = Convert.ToInt16("0x7005", 16);
        var add1ToRegister10 = Convert.ToInt16("0x7A01", 16);
        var add3ToRegister16 = Convert.ToInt16("0x7F03", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(add5ToRegister0);
        sut.ProcessInstruction(add1ToRegister10);
        sut.ProcessInstruction(add3ToRegister16);
        
        Assert.Equal(15, sut.V[0]);
        Assert.Equal(21, sut.V[10]);
        Assert.Equal(33, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy0 - LD Vx, Vy - Set Vx = Vy.")]
    public void process_instruction_8xy0()
    {
        var registers = new int[16];
        
        registers[6] = 50;
        registers[15] = 100;
        
        var storeValueOfRegister15InRegister7 = Convert.ToInt16("0x87F0", 16);
        var storeValueOfRegister6InRegister1 = Convert.ToInt16("0x8160", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(storeValueOfRegister15InRegister7);
        sut.ProcessInstruction(storeValueOfRegister6InRegister1);
        
        Assert.Equal(0, sut.V[0]);
        Assert.Equal(50, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(50, sut.V[6]);
        Assert.Equal(100, sut.V[7]);
        Assert.Equal(0, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(100, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy1 - OR Vx, Vy - Set Vx = Vx OR Vy.")]
    public void process_instruction_8xy1()
    {
        var registers = new int[16];
        
        registers[2] = 20;
        registers[8] = 10;
        
        var orRegister2WithRegister8AndStoreInRegister2 = Convert.ToInt16("0x8281", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(orRegister2WithRegister8AndStoreInRegister2);
        
        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(30, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(10, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy2 - AND Vx, Vy - Set Vx = Vx AND Vy.")]
    public void process_instruction_8xy2()
    {
        var registers = new int[16];
        
        registers[1] = 30;
        registers[13] = 40;
        
        var andRegister13WithRegister1AndStoreInRegister1 = Convert.ToInt16("0x81D2", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(andRegister13WithRegister1AndStoreInRegister1);
        
        Assert.Equal(0, sut.V[0]);
        Assert.Equal(8, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(0, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(40, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy3 - XOR Vx, Vy - Set Vx = Vx XOR Vy.")]
    public void process_instruction_8xy3()
    {
        var registers = new int[16];
        
        registers[2] = 50;
        registers[8] = 60;
        
        var xorRegister8WithRegister2AndStoreInRegister2 = Convert.ToInt16("0x8283", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(xorRegister8WithRegister2AndStoreInRegister2);
        
        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(14, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(60, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy4 - ADD Vx, Vy - Set Vx = Vx + Vy, set VF = carry. With carry.")]
    public void set_the_carry_flag_when_processing_instruction_8xy4_and_the_result_is_greater_than_8_bits()
    {
        var registers = new int[16];
        
        registers[6] = 155;
        registers[7] = 105;
        registers[15] = 0;
        
        var xorRegister8WithRegister2AndStoreInRegister2 = Convert.ToInt16("0x8674", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(xorRegister8WithRegister2AndStoreInRegister2);
        
        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(4, sut.V[6]);
        Assert.Equal(105, sut.V[7]);
        Assert.Equal(0, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(1, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy4 - ADD Vx, Vy - Set Vx = Vx + Vy, set VF = carry. Without carry.")]
    public void not_set_the_carry_flag_when_processing_instruction_8xy4_and_the_result_is_8_bits()
    {
        var registers = new int[16];
        
        registers[6] = 155;
        registers[7] = 100;
        registers[15] = 1;
        
        var xorRegister8WithRegister2AndStoreInRegister2 = Convert.ToInt16("0x8674", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(xorRegister8WithRegister2AndStoreInRegister2);
        
        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(255, sut.V[6]);
        Assert.Equal(100, sut.V[7]);
        Assert.Equal(0, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy5 - SUB Vx, Vy - Set Vx = Vx - Vy, set VF = NOT borrow. With not borrow.")]
    public void set_the_not_borrow_flag_when_vx_is_greater_than_vy_when_processing_instruction_8xy5()
    {
        var registers = new int[16];
        
        registers[0] = 10;
        registers[8] = 15;
        registers[15] = 0;
        
        var xorRegister8WithRegister2AndStoreInRegister2 = Convert.ToInt16("0x8805", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(xorRegister8WithRegister2AndStoreInRegister2);
        
        Assert.Equal(10, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(5, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(1, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy5 - SUB Vx, Vy - Set Vx = Vx - Vy, set VF = NOT borrow. Without not borrow.")]
    public void not_set_the_not_borrow_flag_when_vx_is_not_greater_than_vy_when_processing_instruction_8xy5()
    {
        var registers = new int[16];
        
        registers[0] = 10;
        registers[8] = 6;
        registers[15] = 1;
        
        var xorRegister8WithRegister2AndStoreInRegister2 = Convert.ToInt16("0x8805", 16);
    
        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(xorRegister8WithRegister2AndStoreInRegister2);
        
        Assert.Equal(10, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(-4, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);
    }

    [Fact(DisplayName = "Annn - LD I, addr - Set I = nnn.")]
    public void process_instruction_annn()
    {
        var instruction = Convert.ToInt16("0xA2B4", 16);

        var sut = new Chip8(Array.Empty<int>(), 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(692, sut.I);
    }
    
    [Fact(DisplayName = "Bnnn - JP V0, addr - Jump to location nnn + V0.")]
    public void process_instruction_bnnn()
    {
        var registers = new int[16];
        
        registers[0] = 10;
        
        var instruction = Convert.ToInt16("0xB23F", 16);

        var sut = new Chip8(registers, 500, _testOutputHelper);
        
        sut.ProcessInstruction(instruction);
        
        Assert.Equal(585, sut.PC);
    }
    
    [Fact]
    public void print_memory()
    {
        var memory = new byte[4096];
       
        var gameInstructions = File.ReadAllBytes("../../../games/Blitz.ch8");
        
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