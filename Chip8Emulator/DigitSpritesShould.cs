namespace Chip8Emulator;

public class DigitSprites
{
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
         offset += 5;
      }
   }
}

public class DigitSpritesShould
{
   private readonly byte[] _memory;
   private readonly DigitSprites _sut;

   public DigitSpritesShould()
   {
      _memory = new byte[4096];
      _sut = new DigitSprites();
   }

   [Fact]
   public void copy_the_sprite_for_zero_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[0].ToBinaryString()); 
      Assert.Equal("10010000", _memory[1].ToBinaryString()); 
      Assert.Equal("10010000", _memory[2].ToBinaryString()); 
      Assert.Equal("10010000", _memory[3].ToBinaryString()); 
      Assert.Equal("11110000", _memory[4].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_one_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("00100000", _memory[5].ToBinaryString()); 
      Assert.Equal("01100000", _memory[6].ToBinaryString()); 
      Assert.Equal("00100000", _memory[7].ToBinaryString()); 
      Assert.Equal("00100000", _memory[8].ToBinaryString()); 
      Assert.Equal("01110000", _memory[9].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_two_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[10].ToBinaryString()); 
      Assert.Equal("00010000", _memory[11].ToBinaryString()); 
      Assert.Equal("11110000", _memory[12].ToBinaryString()); 
      Assert.Equal("10000000", _memory[13].ToBinaryString()); 
      Assert.Equal("11110000", _memory[14].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_three_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[15].ToBinaryString()); 
      Assert.Equal("00010000", _memory[16].ToBinaryString()); 
      Assert.Equal("11110000", _memory[17].ToBinaryString()); 
      Assert.Equal("00010000", _memory[18].ToBinaryString()); 
      Assert.Equal("11110000", _memory[19].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_four_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("10010000", _memory[20].ToBinaryString()); 
      Assert.Equal("10010000", _memory[21].ToBinaryString()); 
      Assert.Equal("11110000", _memory[22].ToBinaryString()); 
      Assert.Equal("00010000", _memory[23].ToBinaryString()); 
      Assert.Equal("00010000", _memory[24].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_five_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[25].ToBinaryString()); 
      Assert.Equal("10000000", _memory[26].ToBinaryString()); 
      Assert.Equal("11110000", _memory[27].ToBinaryString()); 
      Assert.Equal("00010000", _memory[28].ToBinaryString()); 
      Assert.Equal("11110000", _memory[29].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_six_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[30].ToBinaryString()); 
      Assert.Equal("10000000", _memory[31].ToBinaryString()); 
      Assert.Equal("11110000", _memory[32].ToBinaryString()); 
      Assert.Equal("10010000", _memory[33].ToBinaryString()); 
      Assert.Equal("11110000", _memory[34].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_seven_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[35].ToBinaryString()); 
      Assert.Equal("00010000", _memory[36].ToBinaryString()); 
      Assert.Equal("00100000", _memory[37].ToBinaryString()); 
      Assert.Equal("01000000", _memory[38].ToBinaryString()); 
      Assert.Equal("01000000", _memory[39].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_eight_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[40].ToBinaryString()); 
      Assert.Equal("10010000", _memory[41].ToBinaryString()); 
      Assert.Equal("11110000", _memory[42].ToBinaryString()); 
      Assert.Equal("10010000", _memory[43].ToBinaryString()); 
      Assert.Equal("11110000", _memory[44].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_nine_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[45].ToBinaryString()); 
      Assert.Equal("10010000", _memory[46].ToBinaryString()); 
      Assert.Equal("11110000", _memory[47].ToBinaryString()); 
      Assert.Equal("00010000", _memory[48].ToBinaryString()); 
      Assert.Equal("11110000", _memory[49].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_a_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[50].ToBinaryString()); 
      Assert.Equal("10010000", _memory[51].ToBinaryString()); 
      Assert.Equal("11110000", _memory[52].ToBinaryString()); 
      Assert.Equal("10010000", _memory[53].ToBinaryString()); 
      Assert.Equal("10010000", _memory[54].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_b_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11100000", _memory[55].ToBinaryString()); 
      Assert.Equal("10010000", _memory[56].ToBinaryString()); 
      Assert.Equal("11100000", _memory[57].ToBinaryString()); 
      Assert.Equal("10010000", _memory[58].ToBinaryString()); 
      Assert.Equal("11100000", _memory[59].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_c_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[60].ToBinaryString()); 
      Assert.Equal("10000000", _memory[61].ToBinaryString()); 
      Assert.Equal("10000000", _memory[62].ToBinaryString()); 
      Assert.Equal("10000000", _memory[63].ToBinaryString()); 
      Assert.Equal("11110000", _memory[64].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_d_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11100000", _memory[65].ToBinaryString()); 
      Assert.Equal("10010000", _memory[66].ToBinaryString()); 
      Assert.Equal("10010000", _memory[67].ToBinaryString()); 
      Assert.Equal("10010000", _memory[68].ToBinaryString()); 
      Assert.Equal("11100000", _memory[69].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_e_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[70].ToBinaryString()); 
      Assert.Equal("10000000", _memory[71].ToBinaryString()); 
      Assert.Equal("11110000", _memory[72].ToBinaryString()); 
      Assert.Equal("10000000", _memory[73].ToBinaryString()); 
      Assert.Equal("11110000", _memory[74].ToBinaryString()); 
   }
   
   [Fact]
   public void copy_the_sprite_for_f_to_the_correct_addresses()
   {
      _sut.CopyTo(_memory);
      
      Assert.Equal("11110000", _memory[75].ToBinaryString()); 
      Assert.Equal("10000000", _memory[76].ToBinaryString()); 
      Assert.Equal("11110000", _memory[77].ToBinaryString()); 
      Assert.Equal("10000000", _memory[78].ToBinaryString()); 
      Assert.Equal("10000000", _memory[79].ToBinaryString()); 
   }
}