using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LeDi.Display2.Effects
{
    public class NoEffect : IEffect
    {
        public override void Execute(CancellationToken EffectCancellationToken)
        {
        }
    }
}
