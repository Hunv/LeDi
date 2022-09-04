﻿using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tiwaz.Display.Effects
{
    public class SetBlack : IEffect
    {
        public override void Execute()
        {
            Console.WriteLine("Running SetBlack");

            Display.Display.SetAll(Color.Black);
            Display.Display.Render();
        }
    }
}