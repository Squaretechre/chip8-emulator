using Xunit;

namespace Chip8Emulator.Tests;

public class RomShould
{
    [Fact]
    public void be_created_from_a_byte_array()
    {
        var memory = new byte[6];
        var romBytes = new byte[] { 0xBA, 0x7C, 0xD6, 0xFE, 0x54, 0xAA };
        
        var sut = new Rom(romBytes);

        sut.CopyTo(memory, 0);
        
        Assert.Equal(0xBA, memory[0]);
        Assert.Equal(0x7C, memory[1]);
        Assert.Equal(0xD6, memory[2]);
        Assert.Equal(0xFE, memory[3]);
        Assert.Equal(0x54, memory[4]);
        Assert.Equal(0xAA, memory[5]);
    }
    
    [Fact]
    public void be_created_from_a_stream()
    {
        var memory = new byte[6];
        var romBytes = new byte[] { 0xBA, 0x7C, 0xD6, 0xFE, 0x54, 0xAA };
        var stream = new MemoryStream(romBytes);
        
        var sut = new Rom(stream);

        sut.CopyTo(memory, 0);
        
        Assert.Equal(0xBA, memory[0]);
        Assert.Equal(0x7C, memory[1]);
        Assert.Equal(0xD6, memory[2]);
        Assert.Equal(0xFE, memory[3]);
        Assert.Equal(0x54, memory[4]);
        Assert.Equal(0xAA, memory[5]);
    }
}