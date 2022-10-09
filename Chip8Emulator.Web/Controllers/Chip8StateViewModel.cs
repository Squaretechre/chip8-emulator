using Chip8Emulator.Extensions;

namespace Chip8Emulator.Web.Controllers;

public class Chip8StateViewModel
{
    public int[] Registers { get; }
    public int I { get; }
    public int PC { get; }
    public Stack<int> Stack { get; }
    public string Display { get; }
    public string[] Memory { get; }
    public IEnumerable<string> Instructions { get; }

    public Chip8StateViewModel(Chip8 chip8, IDebugger debugger)
    {
        I = chip8.I;
        PC = chip8.PC;
        Stack = chip8.Stack;
        Display = chip8.Display();
        Memory = chip8.Memory.ToHexArray();
        Registers = chip8.V;
        Instructions = debugger.GetMessages().Reverse();
    }
}