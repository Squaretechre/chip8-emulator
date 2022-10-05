using Xunit.Abstractions;

namespace Chip8Emulator;

public class Chip8Should
{
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
    
    [Fact(DisplayName = "8xy6 - SHR Vx {, Vy} - Set Vx = Vx SHR 1. When LSB of Vx is 1.")]
    public void set_vf_to_1_when_least_significant_bit_of_vx_is_1_when_processing_instruction_8xy6()
    {
        var registers = new int[16];

        registers[5] = 13;
        registers[15] = 0;

        var instruction = Convert.ToInt16("0x8506", 16);

        var sut = new Chip8(registers, 500, _testOutputHelper);

        sut.ProcessInstruction(instruction);

        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(6, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(0, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(0, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(1, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xy6 - SHR Vx {, Vy} - Set Vx = Vx SHR 1. When LSB of Vx is 0.")]
    public void set_vf_to_0_when_least_significant_bit_of_vx_is_0_when_processing_instruction_8xy6()
    {
        var registers = new int[16];

        registers[5] = 14;
        registers[15] = 1;

        var instruction = Convert.ToInt16("0x8506", 16);

        var sut = new Chip8(registers, 500, _testOutputHelper);

        sut.ProcessInstruction(instruction);

        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(7, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(0, sut.V[8]);
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


    private static bool IsEmpty(Stack<int> stack)
        => ExceptionMessageFor(() => stack.Peek()).Equals("Stack empty.");

    private static string ExceptionMessageFor(Action action)
        => Record.Exception(action)!.Message;
}