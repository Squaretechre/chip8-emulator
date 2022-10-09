namespace Chip8Emulator;

public class DigitSprites
{
    private const int Chip8DigitSpriteLengthInBytes = 5;
    
    private readonly byte[] _zero = { 0xF0, 0x90, 0x90, 0x90, 0xF0 };
    private readonly byte[] _one = { 0x20, 0x60, 0x20, 0x20, 0x70 };
    private readonly byte[] _two = { 0xF0, 0x10, 0xF0, 0x80, 0xF0 };
    private readonly byte[] _three = { 0xF0, 0x10, 0xF0, 0x10, 0xF0 };
    private readonly byte[] _four = { 0x90, 0x90, 0xF0, 0x10, 0x10 };
    private readonly byte[] _five = { 0xF0, 0x80, 0xF0, 0x10, 0xF0 };
    private readonly byte[] _six = { 0xF0, 0x80, 0xF0, 0x90, 0xF0 };
    private readonly byte[] _seven = { 0xF0, 0x10, 0x20, 0x40, 0x40 };
    private readonly byte[] _eight = { 0xF0, 0x90, 0xF0, 0x90, 0xF0 };
    private readonly byte[] _nine = { 0xF0, 0x90, 0xF0, 0x10, 0xF0 };
    private readonly byte[] _a = { 0xF0, 0x90, 0xF0, 0x90, 0x90 };
    private readonly byte[] _b = { 0xE0, 0x90, 0xE0, 0x90, 0xE0 };
    private readonly byte[] _c = { 0xF0, 0x80, 0x80, 0x80, 0xF0 };
    private readonly byte[] _d = { 0xE0, 0x90, 0x90, 0x90, 0xE0 };
    private readonly byte[] _e = { 0xF0, 0x80, 0xF0, 0x80, 0xF0 };
    private readonly byte[] _f = { 0xF0, 0x80, 0xF0, 0x80, 0x80 };

    public void CopyTo(byte[] memory)
    {
        var sprites = new[]
        {
            _zero, _one, _two, _three, _four, _five, _six, _seven, _eight, _nine, _a, _b, _c, _d, _e, _f
        };
      
        var offset = 0;
        
        foreach (var sprite in sprites)
        {
            sprite.CopyTo(memory, offset);
            offset += Chip8DigitSpriteLengthInBytes;
        }
    }

    public int MemoryLocationFor(int digit)
    {
        return digit * Chip8DigitSpriteLengthInBytes;
    }
}