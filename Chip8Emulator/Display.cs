using System.Collections;

namespace Chip8Emulator;

public class Display
{
    private readonly int[,] _display;
    private bool _pixelErased;

    public Display()
    {
        _display = new int[32, 64];
        _pixelErased = false;
        Clear();
    }

    public void Clear()
    {
        for(var row = 0; row < _display.GetLength(0); row++)  { 
            for(var column = 0; column < _display.GetLength(1); column++)  { 
                _display[row, column] = 0;
            }
        }
    }

    public void DrawSpriteAt(int x, int y, IEnumerable<byte> spriteBytes)
    {
        _pixelErased = false;
        
        var spriteBits = spriteBytes
            .Select(@byte => new BitArray(new[] { @byte }))
            .ToList();
        
        var row = y;

        foreach (var rowOfSprite in spriteBits)
        {
            var pixel = x;
            
            for (var bit = 7; bit >= 0; bit--)
            {
                var currentPixel = _display[row, pixel];
                var newPixel = Convert.ToInt32(currentPixel ^ Convert.ToInt32(rowOfSprite[bit]));
                
                if (currentPixel == 1 && newPixel == 0) _pixelErased = true;

                _display[row, pixel] = newPixel;
                pixel += 1;
            }

            row += 1;
        }
    }

    public bool PixelsErasedDuringLastDraw()
    {
        return _pixelErased;
    }
    
    public override string ToString()
    {
        var rows = "";
        
        for(var row = 0; row < _display.GetLength(0); row++)
        {
            var pixels = "";
            
            for(var column = 0; column < _display.GetLength(1); column++)
            {
                pixels += _display[row, column];
            }

            rows += $"{pixels}{Environment.NewLine}";
        }

        return rows;
    }
}