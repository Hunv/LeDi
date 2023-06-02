using System;
using System.Collections.Generic;
using System.Text;

namespace LeDi.Shared2.Display
{
    public class CharacterSet
    {
        public string? Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Character>? Characters { get; set; }
    }
}
