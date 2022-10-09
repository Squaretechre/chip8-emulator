namespace Chip8Emulator;

public interface IDebugger
{
    public void Log(string message);
    public void LogInstruction(short instruction);
    IEnumerable<string> GetMessages();
    void Clear();
}