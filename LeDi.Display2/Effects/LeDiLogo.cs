using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using LeDi.Display2.Display;

namespace LeDi.Display2.Effects
{
    public class LeDiLogo : IEffect
    {
        public override void Execute(CancellationToken EffectCancellationToken)
        {
            Console.WriteLine("Showing LeDi Logo");
            Display.Display.SetAll(Color.Black);

            var spacer = Display.Display.LayoutConfig.Height * 0.1;
            var squaresize = Display.Display.LayoutConfig.Height * 0.4;
            
            //Square1
            var sq1X = (int)(Display.Display.LayoutConfig.Width / 2 - spacer / 2 - squaresize);
            var sq1Y = (int)(Display.Display.LayoutConfig.Height / 2 - spacer / 2 - squaresize);
            for (var a = sq1X; a < squaresize + sq1X; a++)
            {
                for (var b = sq1Y; b < squaresize + sq1Y; b++)
                {
                    Display.Display.SetLed(Display.Display.GetLedNumber(a, b), Color.White);
                }
            }

            //Square2
            var sq2X = (int)(Display.Display.LayoutConfig.Width / 2 + spacer / 2);
            var sq2Y = (int)(Display.Display.LayoutConfig.Height / 2 + spacer / 2);
            for (var a = sq2X; a < squaresize + sq2X; a++)
            {
                for (var b = sq2Y; b < squaresize + sq2Y; b++)
                {
                    Display.Display.SetLed(Display.Display.GetLedNumber(a, b), Color.White);
                }
            }

            //Square3
            var sq3X = (int)(Display.Display.LayoutConfig.Width / 2 + spacer / 2);
            var sq3Y = (int)(Display.Display.LayoutConfig.Height / 2 - spacer / 2 - squaresize);
            for (var a = sq3X; a < squaresize + sq3X; a++)
            {
                for (var b = sq3Y; b < squaresize + sq3Y; b++)
                {
                    Display.Display.SetLed(Display.Display.GetLedNumber(a, b), Color.Red);
                }
            }

            //Square4
            var sq4X = (int)(Display.Display.LayoutConfig.Width / 2 - spacer / 2 - squaresize);
            var sq4Y = (int)(Display.Display.LayoutConfig.Height / 2 + spacer / 2);
            for (var a = sq4X; a < squaresize + sq4X; a++)
            {
                for (var b = sq4Y; b < squaresize + sq4Y; b++)
                {
                    Display.Display.SetLed(Display.Display.GetLedNumber(a, b), Color.Red);
                }
            }

            Display.Display.Render();
        }
    }
}
