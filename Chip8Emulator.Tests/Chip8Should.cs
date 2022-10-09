using Xunit;
using Xunit.Abstractions;

namespace Chip8Emulator.Tests;

public class Chip8Should
{
    private const int InitialProgramCounter = 512;

    private readonly IDebugger _debugger;
    private readonly Func<int> _stubbedRandomNumber;
    private readonly int[] _emptyRegisters;

    public Chip8Should(ITestOutputHelper testOutputHelper)
    {
        _debugger = new XUnitDebugger(testOutputHelper);
        _stubbedRandomNumber = () => 1;
        _emptyRegisters = new int[16];
    }

    [Fact]
    public void load_a_rom_into_memory_at_location_of_the_program_counter()
    {
        var romBytes = new byte[] { 0xBA, 0x7C, 0xD6, 0xFE, 0x54, 0xAA };

        var sut = new Chip8(
            _emptyRegisters,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        sut.Load(new Rom(romBytes));

        Assert.Equal(0xBA, sut.Memory[512]);
        Assert.Equal(0x7C, sut.Memory[513]);
        Assert.Equal(0xD6, sut.Memory[514]);
        Assert.Equal(0xFE, sut.Memory[515]);
        Assert.Equal(0x54, sut.Memory[516]);
        Assert.Equal(0xAA, sut.Memory[517]);
    }
    
    [Fact]
    public void reset_all_state_when_a_rom_is_loaded() {
        var debugger = new Debugger();
        
        var alienSprite = new byte[] { 0xBA, 0x7C, 0xD6, 0xFE, 0x54, 0xAA };

        var registers = new int[16];

        registers[0] = alienSprite[0];
        registers[1] = alienSprite[1];
        registers[2] = alienSprite[2];
        registers[3] = alienSprite[3];
        registers[4] = alienSprite[4];
        registers[5] = alienSprite[5];
        
        var setRegister0 = new byte[] { 0x60, alienSprite[0] };
        var setRegister1 = new byte[] { 0x61, alienSprite[1] };
        var setRegister2 = new byte[] { 0x62, alienSprite[2] };
        var setRegister3 = new byte[] { 0x63, alienSprite[3] };
        var setRegister4 = new byte[] { 0x64, alienSprite[4] };
        var setRegister5 = new byte[] { 0x65, alienSprite[5] };
        var setRegister6 = new byte[] { 0x66, 0x04 };
        var setRegister7 = new byte[] { 0x67, 0x02 };
        var setRegister8 = new byte[] { 0x68, 0x09 };
        var setRegister9 = new byte[] { 0x69, 0x0A };
        var setRegisterA = new byte[] { 0x6A, 0x0B };
        var setRegisterB = new byte[] { 0x6B, 0x0C };
        var setRegisterC = new byte[] { 0x6C, 0x0D };
        var setRegisterD = new byte[] { 0x6D, 0x0E };
        var setRegisterE = new byte[] { 0x6E, 0x0F };
        var setRegisterF = new byte[] { 0x6F, 0x10 };
        var callSubroutineAt806 = new byte[] { 0x23, 0x26 };
        var setITo692 = new byte[] { 0xA2, 0xB4 };
        var storeRegistersV0ThroughVxInMemoryStartingAtI = new byte[] { 0xF6, 0x55 };
        var drawAlienSpriteAtX6Y7 = new byte[] { 0xD6, 0x76 };

        var romBytes = new[]
        {
            setRegister0[0], setRegister0[1],
            setRegister1[0], setRegister1[1],
            setRegister2[0], setRegister2[1],
            setRegister3[0], setRegister3[1],
            setRegister4[0], setRegister4[1],
            setRegister5[0], setRegister5[1],
            setRegister6[0], setRegister6[1],
            setRegister7[0], setRegister7[1],
            setRegister8[0], setRegister8[1],
            setRegister9[0], setRegister9[1],
            setRegisterA[0], setRegisterA[1],
            setRegisterB[0], setRegisterB[1],
            setRegisterC[0], setRegisterC[1],
            setRegisterD[0], setRegisterD[1],
            setRegisterE[0], setRegisterE[1],
            setRegisterF[0], setRegisterF[1],
            setITo692[0], setITo692[1],
            storeRegistersV0ThroughVxInMemoryStartingAtI[0], storeRegistersV0ThroughVxInMemoryStartingAtI[1],
            drawAlienSpriteAtX6Y7[0], drawAlienSpriteAtX6Y7[1],
            callSubroutineAt806[0], callSubroutineAt806[1],
        };
        
        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            debugger);
        
        sut.Load(new Rom(romBytes));
        
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();
        sut.Step();

        Assert.Equal(alienSprite[0], sut.V[0]);
        Assert.Equal(alienSprite[1], sut.V[1]);
        Assert.Equal(alienSprite[2], sut.V[2]);
        Assert.Equal(alienSprite[3], sut.V[3]);
        Assert.Equal(alienSprite[4], sut.V[4]);
        Assert.Equal(alienSprite[5], sut.V[5]);
        Assert.Equal(4, sut.V[6]);
        Assert.Equal(2, sut.V[7]);
        Assert.Equal(9, sut.V[8]);
        Assert.Equal(10, sut.V[9]);
        Assert.Equal(11, sut.V[10]);
        Assert.Equal(12, sut.V[11]);
        Assert.Equal(13, sut.V[12]);
        Assert.Equal(14, sut.V[13]);
        Assert.Equal(15, sut.V[14]);
        Assert.Equal(16, sut.V[15]);
        Assert.Equal(550, sut.Stack.Peek());
        Assert.Equal(692, sut.I);
        Assert.Equal(806, sut.PC);
        Assert.Equal(DisplayTestData.ExpectedAlienSpriteDisplay, sut.Display());
        Assert.Equal(20, debugger.GetMessages().Count());
        
        sut.Load(new Rom(romBytes));

        var expectedMemory = new byte[4096];
        romBytes.CopyTo(expectedMemory, InitialProgramCounter);

        Assert.Equal(0, sut.I);
        Assert.Equal(InitialProgramCounter, sut.PC);
        Assert.True(sut.V.SequenceEqual(_emptyRegisters));
        Assert.True(sut.Memory.SequenceEqual(expectedMemory));
        Assert.True(!sut.Stack.Any());
        Assert.Equal(DisplayTestData.ExpectedEmptyDisplay, sut.Display());
        Assert.Empty(debugger.GetMessages());
    }

    [Theory]
    [InlineData("0xFFFF", "FFFF - Unknown.")]
    [InlineData("0x00E0", "00E0 - CLS - Clear the display.")]
    [InlineData("0x00EE", "00EE - RET - Return from a subroutine.")]
    [InlineData("0x0111", "[NOOP] - 0111 - SYS addr - Jump to routine at 273.")]
    [InlineData("0x1217", "1217 - JP addr - Jump to location 535.")]
    [InlineData("0x2326", "2326 - CALL addr - Call subroutine at 806.")]
    [InlineData("0x3340", "3340 - SE V3, byte - Skip next instruction if V3 = 64.")]
    [InlineData("0x4918", "4918 - SNE V9, byte - Skip next instruction if V9 != 24.")]
    [InlineData("0x5A20", "5A20 - SE VA, V2 - Skip next instruction if VA = V2.")]
    [InlineData("0x6311", "6311 - LD V3, byte - Set V3 = 17.")]
    [InlineData("0x7F03", "7F03 - ADD VF, byte - Set VF = VF + 3.")]
    [InlineData("0x8160", "8160 - LD V1, V6 - Set V1 = V6.")]
    [InlineData("0x8281", "8281 - OR V2, V8 - Set V2 = V2 OR V8.")]
    [InlineData("0x81D2", "81D2 - AND V1, VD - Set V1 = V1 AND VD.")]
    [InlineData("0x8283", "8283 - XOR V2, V8 - Set V2 = V2 XOR V8.")]
    [InlineData("0x8674", "8674 - ADD V6, V7 - Set V6 = V6 + V7, set VF = carry.")]
    [InlineData("0x8805", "8805 - SUB V8, V0 - Set V8 = V8 - V0, set VF = NOT borrow.")]
    [InlineData("0x8506", "8506 - SHR V5 {, V0} - Set V5 = V5 SHR 1.")]
    [InlineData("0x8347", "8347 - SUBN V3, V4 - Set V3 = V4 - V3, set VF = NOT borrow.")]
    [InlineData("0x812E", "812E - SHL V1 {, V2} - Set V1 = V1 SHL 1.")]
    [InlineData("0x9120", "9120 - SNE V1, V2 - Skip next instruction if V1 != V2.")]
    [InlineData("0xA2B4", "A2B4 - LD I, addr - Set I = 692.")]
    [InlineData("0xB23F", "B23F - JP V0, addr - Jump to location 575 + V0.")]
    [InlineData("0xC13B", "C13B - RND V1, byte - Set V1 = random byte AND 59.")]
    [InlineData("0xD426", "D426 - DRW V4, V2, nibble - Display 6-byte sprite starting at memory location I at (V4, V2), set VF = collision.")]
    [InlineData("0xEA9E", "[NOOP] - EA9E - SKP VA - Skip next instruction if key with the value of VA is pressed.")]
    [InlineData("0xEBA1", "[NOOP] - EBA1 - SKNP VB - Skip next instruction if key with the value of VB is not pressed.")]
    [InlineData("0xFC07", "[NOOP] - FC07 - LD VC, DT - Set VC = delay timer value.")]
    [InlineData("0xF10A", "[NOOP] - F10A - LD V1, K - Wait for a key press, store the value of the key in V1.")]
    [InlineData("0xF215", "[NOOP] - F215 - LD DT, V2 - Set delay timer = V2.")]
    [InlineData("0xF318", "[NOOP] - F318 - LD ST, V3 - Set sound timer = V3.")]
    [InlineData("0xF41E", "F41E - ADD I, V4 - Set I = I + V4.")]
    [InlineData("0xF129", "F129 - LD F, V1 - Set I = location of sprite for digit V1.")]
    [InlineData("0xFA33", "FA33 - LD B, VA - Store BCD representation of VA in memory locations I, I+1, and I+2.")]
    [InlineData("0xF255", "F255 - LD [I], V2 - Store registers V0 through V2 in memory starting at location I.")]
    [InlineData("0xF265", "F265 - LD V2, [I] - Read registers V0 through V2 from memory starting at location I.")]
    public void call_debugger_with_each_instruction(string instruction, string expectedMessage)
    {
        var debugger = new Debugger();
        var registers = new int[16];
        
        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            debugger);

        var callSubroutineInstruction = Convert.ToInt16("0x2326", 16);
        
        sut.Process(callSubroutineInstruction);
        sut.Process(Convert.ToInt16(instruction, 16));
        
        Assert.Equal(expectedMessage, debugger.GetMessages().Last());
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
        Assert.Equal(512, sut.Stack.Peek());

        sut.Process(returnFromSubroutineInstruction);

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(516, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(516, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(516, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(544, sut.PC);
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

        Assert.Equal(518, sut.PC);
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

        Assert.Equal(516, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
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

        Assert.Equal(516, sut.PC);
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

        Assert.Equal(514, sut.PC);
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
        Assert.Equal(514, sut.PC);
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

        Assert.Equal(514, sut.PC);
    }

    [Fact(DisplayName = "Fx1E - ADD I, Vx - Set I = I + Vx.")]
    public void process_instruction_fx1e()
    {
        var registers = new int[16];

        registers[4] = 10;

        var setITo692 = Convert.ToInt16("0xA2B4", 16);
        var addVxToIAndStoreInI = Convert.ToInt16("0xF41E", 16);

        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        sut.Process(setITo692);
        sut.Process(addVxToIAndStoreInI);

        Assert.Equal(702, sut.I);
        Assert.Equal(516, sut.PC);
    }

    [Theory(DisplayName = "Fx29 - LD F, Vx - Set I = location of sprite for digit Vx.")]
    [InlineData("0", 0)]
    [InlineData("1", 5)]
    [InlineData("2", 10)]
    [InlineData("3", 15)]
    [InlineData("4", 20)]
    [InlineData("5", 25)]
    [InlineData("6", 30)]
    [InlineData("7", 35)]
    [InlineData("8", 40)]
    [InlineData("9", 45)]
    [InlineData("A", 50)]
    [InlineData("B", 55)]
    [InlineData("C", 60)]
    [InlineData("D", 65)]
    [InlineData("E", 70)]
    [InlineData("F", 75)]
    public void process_instruction_fx29(string digitSpriteX, int expectedMemoryLocation)
    {
        var sut = new Chip8(
            _emptyRegisters,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        var setIToAddressOfDigitSprite0 = Convert.ToInt16($"0xF{digitSpriteX}29", 16);

        sut.Process(setIToAddressOfDigitSprite0);

        Assert.Equal(expectedMemoryLocation, sut.I);
        Assert.Equal(514, sut.PC);
    }

    [Fact(DisplayName = "Fx33 - LD B, Vx - Store BCD representation of Vx in memory locations I, I+1, and I+2.")]
    public void process_instruction_fx33()
    {
        var registers = new int[16];

        registers[10] = 255;

        var setITo692 = Convert.ToInt16("0xA2B4", 16);
        var storeBcdRepresentationOfVxInMemory = Convert.ToInt16("0xFA33", 16);

        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        sut.Process(setITo692);
        sut.Process(storeBcdRepresentationOfVxInMemory);

        Assert.Equal(2, sut.Memory[692]);
        Assert.Equal(5, sut.Memory[693]);
        Assert.Equal(5, sut.Memory[694]);

        Assert.Equal(516, sut.PC);
    }

    [Fact(DisplayName = "Fx55 - LD [I], Vx - Store registers V0 through Vx in memory starting at location I.")]
    public void process_instruction_fx55()
    {
        var registers = new int[16];

        registers[0] = 10;
        registers[1] = 20;
        registers[2] = 30;

        var setITo692 = Convert.ToInt16("0xA2B4", 16);
        var storeRegistersV0ThroughVxInMemoryStartingAtI = Convert.ToInt16("0xF255", 16);

        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        sut.Process(setITo692);
        sut.Process(storeRegistersV0ThroughVxInMemoryStartingAtI);

        Assert.Equal(10, sut.Memory[692]);
        Assert.Equal(20, sut.Memory[693]);
        Assert.Equal(30, sut.Memory[694]);

        Assert.Equal(516, sut.PC);
    }

    [Fact(DisplayName = "Fx65 - LD Vx, [I] - Read registers V0 through Vx from memory starting at location I.")]
    public void process_instruction_fx65()
    {
        var registers = new int[16];

        registers[0] = 0;
        registers[1] = 0;
        registers[2] = 0;
        registers[10] = 254;

        var setITo692 = Convert.ToInt16("0xA2B4", 16);
        var storeBcdRepresentationOfVxInMemory = Convert.ToInt16("0xFA33", 16);
        var readRegistersV0ThroughVxFromMemoryStartingAtI = Convert.ToInt16("0xF265", 16);

        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        sut.Process(setITo692);
        sut.Process(storeBcdRepresentationOfVxInMemory);
        sut.Process(readRegistersV0ThroughVxFromMemoryStartingAtI);

        Assert.Equal(2, sut.V[0]);
        Assert.Equal(5, sut.V[1]);
        Assert.Equal(4, sut.V[2]);
        Assert.Equal(0, sut.V[3]);
        Assert.Equal(0, sut.V[4]);
        Assert.Equal(0, sut.V[5]);
        Assert.Equal(0, sut.V[6]);
        Assert.Equal(0, sut.V[7]);
        Assert.Equal(0, sut.V[8]);
        Assert.Equal(0, sut.V[9]);
        Assert.Equal(254, sut.V[10]);
        Assert.Equal(0, sut.V[11]);
        Assert.Equal(0, sut.V[12]);
        Assert.Equal(0, sut.V[13]);
        Assert.Equal(0, sut.V[14]);
        Assert.Equal(0, sut.V[15]);

        Assert.Equal(518, sut.PC);
    }

    [Fact(DisplayName =
        "Dxyn - DRW Vx, Vy, nibble - Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.")]
    public void process_instruction_dxyn()
    {
        var alienSprite = new byte[] { 0xBA, 0x7C, 0xD6, 0xFE, 0x54, 0xAA };

        var registers = new int[16];

        registers[0] = alienSprite[0];
        registers[1] = alienSprite[1];
        registers[2] = alienSprite[2];
        registers[3] = alienSprite[3];
        registers[4] = alienSprite[4];
        registers[5] = alienSprite[5];
        
        registers[6] = 4;
        registers[7] = 2;

        var setITo692 = Convert.ToInt16("0xA2B4", 16);
        var storeRegistersV0ThroughVxInMemoryStartingAtI = Convert.ToInt16("0xF655", 16);
        var drawAlienSpriteAtX6Y7 = Convert.ToInt16("0xD676", 16);

        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        sut.Process(setITo692);
        sut.Process(storeRegistersV0ThroughVxInMemoryStartingAtI);
        sut.Process(drawAlienSpriteAtX6Y7);

        Assert.Equal(DisplayTestData.ExpectedAlienSpriteDisplay, sut.Display());

        Assert.Equal(518, sut.PC);
    }

    [Fact(DisplayName = "00E0 - CLS - Clear the display.")]
    public void process_instruction_00e0()
    {
        var alienSprite = new byte[] { 0xBA, 0x7C, 0xD6, 0xFE, 0x54, 0xAA };

        var registers = new int[16];

        registers[0] = alienSprite[0];
        registers[1] = alienSprite[1];
        registers[2] = alienSprite[2];
        registers[3] = alienSprite[3];
        registers[4] = alienSprite[4];
        registers[5] = alienSprite[5];
        
        registers[6] = 4;
        registers[7] = 2;

        var setITo692 = Convert.ToInt16("0xA2B4", 16);
        var storeRegistersV0ThroughVxInMemoryStartingAtI = Convert.ToInt16("0xF655", 16);
        var drawAlienSpriteAtX6Y7 = Convert.ToInt16("0xD676", 16);
        var clearTheDisplay = Convert.ToInt16("0x00E0", 16);

        var sut = new Chip8(
            registers,
            InitialProgramCounter,
            _stubbedRandomNumber,
            _debugger);

        sut.Process(setITo692);
        sut.Process(storeRegistersV0ThroughVxInMemoryStartingAtI);
        sut.Process(drawAlienSpriteAtX6Y7);

        Assert.Equal(DisplayTestData.ExpectedAlienSpriteDisplay, sut.Display());

        sut.Process(clearTheDisplay);

        Assert.Equal(DisplayTestData.ExpectedEmptyDisplay, sut.Display());

        Assert.Equal(520, sut.PC);
    }
}