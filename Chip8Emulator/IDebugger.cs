namespace Chip8Emulator;

public interface IDebugger
{
    public void Log(string message);
    public void LogState(Chip8 chip8);
}