using System;
using System.Collections.Generic;
using System.Text;

namespace LeDi.Display2.Effects
{
    public abstract class IEffect
    {
        public abstract void Execute(CancellationToken EffectCancellationToken);
    }
}
