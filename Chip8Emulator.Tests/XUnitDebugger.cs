using Xunit.Abstractions;

namespace Chip8Emulator.Tests;

public class XUnitDebugger : IDebugger
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitDebugger(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    public void Log(string message)
    {
        _testOutputHelper.WriteLine(message);
    }
}