using Chip8Emulator.Extensions;

namespace Chip8Emulator;

public class Debugger : IDebugger
{
    private List<string> _messages = new();
    
    public IEnumerable<string> GetMessages()
    {
        return _messages;
    }

    public void Clear()
    {
        _messages = new List<string>();
    }

    public void LogInstruction(short instruction)
    { 
       var instructionHex = instruction.ToHexString();
        
        if (instruction.Matches("00E0"))
        {
            _messages.Add($"00E0 - CLS - Clear the display.");
            return;
        }

        if (instruction.Matches("00EE"))
        {
            _messages.Add($"00EE - RET - Return from a subroutine.");
            return;
        }

        if (instruction.Matches("0..."))
        {
            var location = instruction.Lower12Bits();
            _messages.Add($"[NOOP] - {instructionHex} - SYS addr - Jump to routine at {location}.");
            return;
        }

        if (instruction.Matches("1..."))
        {
            var location = instruction.Lower12Bits();
            _messages.Add($"{instructionHex} - JP addr - Jump to location {location}.");
            return;
        }

        if (instruction.Matches("2..."))
        {
            var location = instruction.Lower12Bits();
            _messages.Add($"{instructionHex} - CALL addr - Call subroutine at {location}.");
            return;
        }
        
        if (instruction.Matches("3..."))
        {
            var (upperByte, valueToCompare) = instruction.UpperAndLowerBytes();
            var x = upperByte.LowerNibble().Hex();

            _messages.Add($"{instructionHex} - SE V{x}, byte - Skip next instruction if V{x} = {valueToCompare}.");
            return;
        }
        
        if (instruction.Matches("4..."))
        {
            var (upperByte, valueToCompare) = instruction.UpperAndLowerBytes();
            var x = upperByte.LowerNibble().Hex();

            _messages.Add($"{instructionHex} - SNE V{x}, byte - Skip next instruction if V{x} != {valueToCompare}.");
            return;
        }
        
        if (instruction.Matches("5..0"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - SE V{xHex}, V{yHex} - Skip next instruction if V{xHex} = V{yHex}.");
            return;
        }
        
        if (instruction.Matches("6..."))
        {
            var (upperByte, valueToCompare) = instruction.UpperAndLowerBytes();
            var x = upperByte.LowerNibble().Hex();

            _messages.Add($"{instructionHex} - LD V{x}, byte - Set V{x} = {valueToCompare}.");
            return;
        }
        
        if (instruction.Matches("7..."))
        {
            var (upperByte, valueToCompare) = instruction.UpperAndLowerBytes();
            var x = upperByte.LowerNibble().Hex();

            _messages.Add($"{instructionHex} - ADD V{x}, byte - Set V{x} = V{x} + {valueToCompare}.");
            return;
        }
        
        if (instruction.Matches("8..0"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - LD V{xHex}, V{yHex} - Set V{xHex} = V{yHex}.");
            return;
        }
        
        if (instruction.Matches("8..1"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - OR V{xHex}, V{yHex} - Set V{xHex} = V{xHex} OR V{yHex}.");
            return;
        }
        
        if (instruction.Matches("8..2"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - AND V{xHex}, V{yHex} - Set V{xHex} = V{xHex} AND V{yHex}.");
            return;
        }
        
        if (instruction.Matches("8..3"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - XOR V{xHex}, V{yHex} - Set V{xHex} = V{xHex} XOR V{yHex}.");
            return;
        }
        
        if (instruction.Matches("8..4"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - ADD V{xHex}, V{yHex} - Set V{xHex} = V{xHex} + V{yHex}, set VF = carry.");
            return;
        }
        
        if (instruction.Matches("8..5"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - SUB V{xHex}, V{yHex} - Set V{xHex} = V{xHex} - V{yHex}, set VF = NOT borrow.");
            return;
        }
        
        if (instruction.Matches("8..6"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - SHR V{xHex} {{, V{yHex}}} - Set V{xHex} = V{xHex} SHR 1.");
            return;
        }
        
        if (instruction.Matches("8..7"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - SUBN V{xHex}, V{yHex} - Set V{xHex} = V{yHex} - V{xHex}, set VF = NOT borrow.");
            return;
        }
        
        if (instruction.Matches("8..E"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - SHL V{xHex} {{, V{yHex}}} - Set V{xHex} = V{xHex} SHL 1.");
            return;
        }
        
        if (instruction.Matches("9..0"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - SNE V{xHex}, V{yHex} - Skip next instruction if V{xHex} != V{yHex}.");
            return;
        }
        
        if (instruction.Matches("A..."))
        {
            var location = instruction.Lower12Bits();
            _messages.Add($"{instructionHex} - LD I, addr - Set I = {location}.");
            return;
        }
        
        if (instruction.Matches("B..."))
        {
            var location = instruction.Lower12Bits();
            _messages.Add($"{instructionHex} - JP V0, addr - Jump to location {location} + V0.");
            return;
        }
        
        if (instruction.Matches("C..."))
        {
            var (upperByte, kk) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble().Hex();

            _messages.Add($"{instructionHex} - RND V{x}, byte - Set V{x} = random byte AND {kk}.");
            return;
        }
        
        if (instruction.Matches("D..."))
        {
            var (upperByte, lowerByte) = instruction.UpperAndLowerBytes();

            var x = upperByte.LowerNibble().Hex();
            var y = lowerByte.UpperNibble().Hex();
            var n = lowerByte.LowerNibble().Hex();

            _messages.Add($"{instructionHex} - DRW V{x}, V{y}, nibble - Display {n}-byte sprite starting at memory location I at (V{x}, V{y}), set VF = collision.");
            return;
        }
        
        if (instruction.Matches("E.9E"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"[NOOP] - {instructionHex} - SKP V{xHex} - Skip next instruction if key with the value of V{xHex} is pressed.");
            return;
        }
        
        if (instruction.Matches("E.A1"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"[NOOP] - {instructionHex} - SKNP V{xHex} - Skip next instruction if key with the value of V{xHex} is not pressed.");
            return;
        }
        
        if (instruction.Matches("F.07"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"[NOOP] - {instructionHex} - LD V{xHex}, DT - Set V{xHex} = delay timer value.");
            return;
        }
        
        if (instruction.Matches("F.0A"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"[NOOP] - {instructionHex} - LD V{xHex}, K - Wait for a key press, store the value of the key in V{xHex}.");
            return;
        }
        
        if (instruction.Matches("F.15"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"[NOOP] - {instructionHex} - LD DT, V{xHex} - Set delay timer = V{xHex}.");
            return;
        }
        
        if (instruction.Matches("F.18"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"[NOOP] - {instructionHex} - LD ST, V{xHex} - Set sound timer = V{xHex}.");
            return;
        }
        
        if (instruction.Matches("F.1E"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();
            var yHex = y.Hex();

            _messages.Add($"{instructionHex} - ADD I, V{xHex} - Set I = I + V{xHex}.");
            return;
        }
        
        if (instruction.Matches("F.29"))
        {
            var (x, y) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"{instructionHex} - LD F, V{xHex} - Set I = location of sprite for digit V{xHex}.");
            return;
        }
        
        if (instruction.Matches("F.33"))
        {
            var (x, _) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"{instructionHex} - LD B, V{xHex} - Store BCD representation of V{xHex} in memory locations I, I+1, and I+2.");
            return;
        }
        
        if (instruction.Matches("F.55"))
        {
            var (x, _) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"{instructionHex} - LD [I], V{xHex} - Store registers V0 through V{xHex} in memory starting at location I.");
            return;
        }
        
        if (instruction.Matches("F.65"))
        {
            var (x, _) = instruction.MiddleTwoNibbles();
            var xHex = x.Hex();

            _messages.Add($"{instructionHex} - LD V{xHex}, [I] - Read registers V0 through V{xHex} from memory starting at location I.");
            return;
        }

        _messages.Add($"{instructionHex} - Unknown.");
    }

    public void Log(string message)
    {
        _messages.Add(message);
    }
}