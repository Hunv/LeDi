﻿using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display2.Effects
{
    public class TestBlack : IEffect
    {
        public override void Execute(CancellationToken EffectCancellationToken)
        {
            Console.WriteLine("Running SetBlack");

            Display.Display.SetAll(Color.Black);
            Display.Display.Render();
        }
    }
}