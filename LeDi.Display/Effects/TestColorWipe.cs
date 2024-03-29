﻿using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display.Effects
{
    public class TestColorWipe : IEffect
    {
        public override void Execute()
        {
            Console.WriteLine("Running ColorWipe");
            for (int i = 0; i < 60; i++)
            {
                var color = i % 3;
                if (color == 0)
                    Display.Display.SetAll(Color.Red);
                else if (color == 1)
                    Display.Display.SetAll(Color.Green);
                else if (color == 2)
                    Display.Display.SetAll(Color.Blue);

                Display.Display.Render();
                Thread.Sleep(1000);
            }

            Display.Display.SetAll(Color.Black);
            Display.Display.Render();
            
        }
    }
}
