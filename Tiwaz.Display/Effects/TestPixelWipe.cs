using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tiwaz.Display.Effects
{
    public class TestPixelWipe:IEffect
    {
        public Color Color { get; set; } = Color.White;

        public override void Execute()
        {
            for (int i = 0; i <= Display.Display.LedCount -1; i++)
            {
                Display.Display.SetLed(i, Color);
                Display.Display.Render();
            }
        }
    }
}
