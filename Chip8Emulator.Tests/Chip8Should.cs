using Xunit;
using Xunit.Abstractions;

namespace Chip8Emulator.Tests;

public class Chip8Should
{
    private const int InitialProgramCounter = 500;
    private readonly IDebugger _debugger;
    private readonly Func<int> _stubbedRandomNumber;
    private readonly int[] _emptyRegisters;

    public Chip8Should(ITestOutputHelper testOutputHelper)
    {
        _debugger = new XUnitDebugger(testOutputHelper);
        _stubbedRandomNumber = () => 1;
        _emptyRegisters = Array.Empty<int>();
    }

    [Fact(DisplayName = "00EE - RET - Return from a subroutine.")]
    public void process_instruction_00ee()
    {
        var callSubroutineInstruction = Convert.ToInt16("0x2326", 16);
        var returnFromSubroutineInstruction = Convert.ToInt16("0x00EE", 16);

        var sut = new Chip8(
            _emptyRegisters, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(callSubroutineInstruction);

        Assert.Equal(806, sut.PC);
        Assert.Equal(500, sut.Stack.Peek());

        sut.Process(returnFromSubroutineInstruction);

        Assert.Equal(InitialProgramCounter, sut.PC);
        Assert.False(sut.Stack.Any());
    }

    [Fact(DisplayName = "1nnn - JP addr - Jump to location nnn.")]
    public void process_instruction_1nnn()
    {
        var instruction = Convert.ToInt16("0x1217", 16);

        var sut = new Chip8(
            _emptyRegisters, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(535, sut.PC);
    }

    [Fact(DisplayName = "2nnn - CALL addr - Call subroutine at nnn.")]
    public void process_instruction_2nnn()
    {
        var instruction = Convert.ToInt16("0x2326", 16);

        var sut = new Chip8(
            _emptyRegisters, 
            520, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(806, sut.PC);
        Assert.Equal(520, sut.Stack.Peek());
    }

    [Fact(DisplayName = "3xkk - SE Vx, byte - Skip next instruction if Vx = kk. When Vx = kk.")]
    public void increment_the_program_counter_by_2_when_vx_matches_value_kk_for_instruction_3xkk()
    {
        var registers = new int[16];

        registers[3] = 64;

        var instruction = Convert.ToInt16("0x3340", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(502, sut.PC);
    }

    [Fact(DisplayName = "3xkk - SE Vx, byte - Skip next instruction if Vx = kk. When Vx != kk.")]
    public void not_increment_the_program_counter_by_2_when_vx_does_not_match_value_kk_for_instruction_3xkk()
    {
        var registers = new int[16];

        registers[3] = 63;

        var instruction = Convert.ToInt16("0x3340", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(500, sut.PC);
    }

    [Fact(DisplayName = "4xkk - SNE Vx, byte - Skip next instruction if Vx != kk. When Vx != kk.")]
    public void increment_the_program_counter_by_2_when_vx_does_not_matches_value_kk_for_instruction_4xkk()
    {
        var registers = new int[16];

        registers[9] = 25;

        var instruction = Convert.ToInt16("0x4918", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(502, sut.PC);
    }

    [Fact(DisplayName = "4xkk - SNE Vx, byte - Skip next instruction if Vx != kk. When Vx = kk.")]
    public void not_increment_the_program_counter_by_2_when_vx_does_match_value_kk_for_instruction_4xkk()
    {
        var registers = new int[16];

        registers[9] = 24;

        var instruction = Convert.ToInt16("0x4918", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(500, sut.PC);
    }

    [Fact(DisplayName = "5xy0 - SE Vx, Vy - Skip next instruction if Vx = Vy. When Vx = Vy.")]
    public void increment_the_program_counter_by_2_when_vx_equals_vy_for_instruction_5xy0()
    {
        var registers = new int[16];

        registers[2] = 100;
        registers[10] = 100;

        var instruction = Convert.ToInt16("0x5A20", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(502, sut.PC);
    }

    [Fact(DisplayName = "5xy0 - SE Vx, Vy - Skip next instruction if Vx = Vy. When Vx != Vy.")]
    public void not_increment_the_program_counter_by_2_when_vx_does_not_equal_vy_for_instruction_5xy0()
    {
        var registers = new int[16];

        registers[2] = 99;
        registers[10] = 100;

        var instruction = Convert.ToInt16("0x5A20", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instructionForRegister0);
        sut.Process(instructionForRegister1);
        sut.Process(instructionForRegister2);
        sut.Process(instructionForRegister3);
        sut.Process(instructionForRegister4);
        sut.Process(instructionForRegister5);
        sut.Process(instructionForRegister6);
        sut.Process(instructionForRegister7);
        sut.Process(instructionForRegister8);
        sut.Process(instructionForRegister9);
        sut.Process(instructionForRegister10);
        sut.Process(instructionForRegister11);
        sut.Process(instructionForRegister12);
        sut.Process(instructionForRegister13);
        sut.Process(instructionForRegister14);
        sut.Process(instructionForRegister15);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(add5ToRegister0);
        sut.Process(add1ToRegister10);
        sut.Process(add3ToRegister16);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(storeValueOfRegister15InRegister7);
        sut.Process(storeValueOfRegister6InRegister1);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(orRegister2WithRegister8AndStoreInRegister2);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(andRegister13WithRegister1AndStoreInRegister1);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(xorRegister8WithRegister2AndStoreInRegister2);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(xorRegister8WithRegister2AndStoreInRegister2);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(xorRegister8WithRegister2AndStoreInRegister2);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(xorRegister8WithRegister2AndStoreInRegister2);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(xorRegister8WithRegister2AndStoreInRegister2);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

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

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

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
    
    [Fact(DisplayName = "8xy7 - SUBN Vx, Vy - Set Vx = Vy - Vx, set VF = NOT borrow. With NOT borrow.")]
    public void set_vf_to_1_when_vy_greater_than_vx_when_processing_instruction_8xy6()
    {
        var registers = new int[16];

        registers[3] = 10;
        registers[4] = 15;
        registers[15] = 0;

        var instruction = Convert.ToInt16("0x8347", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(5, sut.V[3]);
        Assert.Equal(15, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
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
    
    [Fact(DisplayName = "8xy7 - SUBN Vx, Vy - Set Vx = Vy - Vx, set VF = NOT borrow. Without NOT borrow.")]
    public void set_vf_to_0_when_vy_is_not_greater_than_vx_when_processing_instruction_8xy6()
    {
        var registers = new int[16];

        registers[3] = 10;
        registers[4] = 9;
        registers[15] = 1;

        var instruction = Convert.ToInt16("0x8347", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(0, sut.V[0]);
        Assert.Equal(0, sut.V[1]);
        Assert.Equal(0, sut.V[2]);
        Assert.Equal(-1, sut.V[3]);
        Assert.Equal(9, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
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
    
    [Fact(DisplayName = "8xyE - SHL Vx {, Vy} - Set Vx = Vx SHL 1. When MSB of Vx is 1")]
    public void set_vf_to_1_when_most_significant_bit_of_vx_is_1_when_processing_instruction_8xyE()
    {
        var registers = new int[16];

        registers[1] = 128;
        registers[15] = 0;

        var instruction = Convert.ToInt16("0x812E", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(0, sut.V[0]);
        Assert.Equal(256, sut.V[1]);
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
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(1, sut.V[15]);
    }
    
    [Fact(DisplayName = "8xyE - SHL Vx {, Vy} - Set Vx = Vx SHL 1. When MSB of Vx is 1")]
    public void set_vf_to_0_when_most_significant_bit_of_vx_is_0_when_processing_instruction_8xyE()
    {
        var registers = new int[16];

        registers[1] = 127;
        registers[15] = 0;

        var instruction = Convert.ToInt16("0x812E", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(0, sut.V[0]);
        Assert.Equal(254, sut.V[1]);
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
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);
    }
    
    [Fact(DisplayName = "9xy0 - SNE Vx, Vy - Skip next instruction if Vx != Vy. When Vx != Vy.")]
    public void increment_program_counter_by_2_when_vx_not_equal_to_vy_when_processing_instruction_9xy0()
    {
        var registers = new int[16];

        registers[1] = 10;
        registers[2] = 20;

        var instruction = Convert.ToInt16("0x9120", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);
        
        Assert.Equal(502, sut.PC);
    }
    
    [Fact(DisplayName = "9xy0 - SNE Vx, Vy - Skip next instruction if Vx != Vy. When Vx = Vy.")]
    public void not_increment_program_counter_by_2_when_vx_is_equal_to_vy_when_processing_instruction_9xy0()
    {
        var registers = new int[16];

        registers[1] = 10;
        registers[2] = 10;

        var instruction = Convert.ToInt16("0x9120", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);
        
        Assert.Equal(500, sut.PC);
    }

    [Fact(DisplayName = "Annn - LD I, addr - Set I = nnn.")]
    public void process_instruction_annn()
    {
        var instruction = Convert.ToInt16("0xA2B4", 16);

        var sut = new Chip8(
            _emptyRegisters, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(692, sut.I);
    }

    [Fact(DisplayName = "Bnnn - JP V0, addr - Jump to location nnn + V0.")]
    public void process_instruction_bnnn()
    {
        var registers = new int[16];

        registers[0] = 10;

        var instruction = Convert.ToInt16("0xB23F", 16);

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            _stubbedRandomNumber,
            _debugger);

        sut.Process(instruction);

        Assert.Equal(585, sut.PC);
    }
    
    [Fact(DisplayName = "Cxkk - RND Vx, byte - Set Vx = random byte AND kk.")]
    public void process_instruction_cxkk()
    {
        var registers = new int[16];

        registers[1] = 0;

        var instruction = Convert.ToInt16("0xC13B", 16);

        int RandomNumber() => 20;

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            RandomNumber,
            _debugger);

        sut.Process(instruction);
        
        Assert.Equal(0, sut.V[0]);
        Assert.Equal(16, sut.V[1]);
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
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);
    }
    
    [Fact(DisplayName = "Fx1E - ADD I, Vx - Set I = I + Vx.")]
    public void process_instruction_fx1e()
    {
        var registers = new int[16];

        registers[4] = 10;
        
        var setITo692 = Convert.ToInt16("0xA2B4", 16);
        var addVxToIAndStoreInI = Convert.ToInt16("0xF41E", 16);

        int RandomNumber() => 20;

        var sut = new Chip8(
            registers, 
            InitialProgramCounter, 
            RandomNumber,
            _debugger);

        sut.Process(setITo692);
        sut.Process(addVxToIAndStoreInI);
       
        Assert.Equal(702, sut.I);
    }
}