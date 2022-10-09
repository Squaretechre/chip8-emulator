using Microsoft.AspNetCore.Mvc;

namespace Chip8Emulator.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class Chip8Controller : ControllerBase
{
    private readonly Chip8 _chip8;
    private readonly IDebugger _debugger;

    public Chip8Controller(Chip8 chip8, IDebugger debugger)
    {
        _chip8 = chip8;
        _debugger = debugger;
    }

    [HttpGet(Name = "GetChip8")]
    public Chip8StateViewModel Get()
    {
        return new Chip8StateViewModel(_chip8, _debugger);
    }
    
    [HttpGet("step", Name = "StepInstruction")]
    public Chip8StateViewModel StepInstruction()
    {
        _chip8.Step();
        
        return new Chip8StateViewModel(_chip8, _debugger);
    }
    
    [HttpPost("load", Name = "LoadRom")]
    public IResult Load(IFormFile rom)
    {
        var stream = new MemoryStream();
        rom.CopyTo(stream); 
        
        _chip8.Load(new Rom(stream));
        
        return Results.Ok();
    }
}