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
            
            Display.Display.SetAll(Color.White); //FFFFFF
            Display.Display.Render();
            Thread.Sleep(10000);
            Display.Display.SetAll(Color.Green); //00FF00
            Display.Display.Render();
            Thread.Sleep(10000);
            Display.Display.SetAll(Color.Red); //FF0000
            Display.Display.Render();
            Thread.Sleep(10000);
            Display.Display.SetAll(Color.Blue); //0000FF
            Display.Display.Render();
            Thread.Sleep(10000);
            Display.Display.SetAll(Color.Cyan); //00FFFF
            Display.Display.Render();
            Thread.Sleep(10000);
            Display.Display.SetAll(Color.Yellow); //FFFF00
            Display.Display.Render();
            Thread.Sleep(10000);
            Display.Display.SetAll(Color.Magenta); //FF00FF
            Display.Display.Render();
            Thread.Sleep(10000);            
            Display.Display.SetAll(Color.Black);            
            Display.Display.Render();
        }
    }
}
