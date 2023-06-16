using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display2.Effects
{
    public class TestPixelWipe:IEffect
    {
        public Color Color { get; set; } = Color.White;

        public override void Execute(CancellationToken EffectCancellationToken)
        {
            Console.WriteLine("Running PixelWipe");
            for (int i = 0; i < Display.Display.LedCount; i++)
            {
                if (EffectCancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("PixelWipe stopped.");
                    return;
                }

                Display.Display.SetLed(i, Color);
                Display.Display.Render();
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
