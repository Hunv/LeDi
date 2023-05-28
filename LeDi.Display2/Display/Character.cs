using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display2.Display
{
    public class Character
    {
        public string? Name { get; set; }
        public char Char { get; set; }
        public string? File { get; set; }
        public Color[,] Pixels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Character(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[width,height];
        }
    }
}
