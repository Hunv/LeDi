using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tiwaz.Display.Effects
{
    public class TestPattern : IEffect
    {
        public override void Execute()
        {
            Console.WriteLine("Running TestPattern");
            var tileSize = Display.Display.X / 6;
            var ColorList = new List<Color>
            {
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.White
            };

            // As long as the full numbers of tileSize is smaller than the exact number, there are LEDs left
            for (var x = 0; x < Display.Display.X; x+=tileSize)
            {
                for (var y = 0; y < Display.Display.Y; y += tileSize)
                {
                    var color = ColorList[((x + y)/tileSize) % 4];
                    for (var i = 0; i < tileSize; i++)
                    {
                        for (var j = 0; j < tileSize; j++)
                        {
                            if(x+i < Display.Display.X && y+j < Display.Display.Y)
                                Display.Display.SetLed(Display.Display.GetLedNumber(x + i, y + j), color);
                        }
                    }
                }
            }
            Display.Display.Render();
        }
    }
}
