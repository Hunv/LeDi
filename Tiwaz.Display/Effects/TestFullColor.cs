using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tiwaz.Display.Effects
{
    public class TestFullColor : IEffect
    {
        public override void Execute()
        {
            Console.WriteLine("Running TestFullColor");
            
            Display.Display.SetAll(Color.White);
            Display.Display.Render();
            Thread.Sleep(1000);
            Display.Display.SetAll(Color.Green);
            Display.Display.Render();
            Thread.Sleep(1000);
            Display.Display.SetAll(Color.Red);
            Display.Display.Render();
            Thread.Sleep(1000);
            Display.Display.SetAll(Color.Blue);
            Display.Display.Render();
            Thread.Sleep(1000);            
            Display.Display.SetAll(Color.Black);            
            Display.Display.Render();
        }
    }
}
