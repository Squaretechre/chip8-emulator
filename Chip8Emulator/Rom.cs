namespace Chip8Emulator;

public class Rom
{
    private readonly byte[] _bytes;

    public Rom(byte[] @bytes)
    {
        _bytes = bytes;
    }

    public Rom(MemoryStream stream)
    {
        _bytes = stream.ToArray();
    }

    public void CopyTo(byte[] memory, int position)
    {
        _bytes.CopyTo(memory, position);
    }
}