using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display2.Effects
{
    public class TestBrightness : IEffect
    {
        public override void Execute()
        {
            Console.WriteLine("Running Brightness Calibration");
            Console.WriteLine("Brightness: {0}", Display.Display.Brightness);
            Display.Display.SetAll(Color.Black);

            for(int x = 0; x < Display.Display.X;x++)
            {
                var colorFactor = 255 / Display.Display.X * x;
                for (int y = 0; y < Display.Display.Y;y++)
                {
                    Display.Display.SetLed(Display.Display.GetLedNumber(x, y), Color.FromArgb(colorFactor, colorFactor, colorFactor));
                }
            }
            
            Display.Display.Render();
            
        }
    }
}
