using Chip8Emulator.Extensions;
using Xunit.Abstractions;

namespace Chip8Emulator.Tests;

public class XUnitDebugger : IDebugger
{
    private readonly ITestOutputHelper _testOutputHelper;
    private List<string> _messages = new();

    public XUnitDebugger(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    public void Log(string message)
    {
        _messages.Add(message);
        _testOutputHelper.WriteLine(message);
    }

    public void LogInstruction(short instruction)
    {
        _messages.Add(instruction.ToHexString());
        _testOutputHelper.WriteLine(instruction.ToHexString());
    }

    public IEnumerable<string> GetMessages()
    {
        return _messages;
    }

    public void Clear()
    {
        _messages = new List<string>();
    }
}