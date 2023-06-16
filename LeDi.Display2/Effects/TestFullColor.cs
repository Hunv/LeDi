using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display2.Effects
{
    public class TestFullColor : IEffect
    {
        public override void Execute(CancellationToken EffectCancellationToken)
        {
            Console.WriteLine("Running TestFullColor");

            Color[] testColors = new Color[] { Color.White, Color.Green, Color.Red, Color.Blue, Color.Cyan, Color.Yellow, Color.Magenta };

            foreach(var aColor in testColors)
            {
                if (EffectCancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("TestFullColor stopped.");
                    return;
                }

                Display.Display.SetAll(aColor);
                Display.Display.Render();
                Thread.Sleep(10000);
            }
        }
    }
}
