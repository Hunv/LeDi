using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display.Effects
{
    public class TestPixelWipe:IEffect
    {
        public Color Color { get; set; } = Color.White;

        public override void Execute()
        {
            Console.WriteLine("Running PixelWipe");
            for (int i = 0; i < Display.Display.LedCount; i++)
            {
                Display.Display.SetLed(i, Color);
                Display.Display.Render();
            }
        }
    }
}
