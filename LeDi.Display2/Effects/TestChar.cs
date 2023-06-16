using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Linq;
using rpi_ws281x;

namespace LeDi.Display2.Effects
{
    public class TestChar : IEffect
    {
        public override void Execute(CancellationToken EffectCancellationToken)
        {
            Console.WriteLine("Running CharTest");
            if (Display.Display.LayoutConfig == null || Display.Display.CharacterSets == null || Display.Display.LayoutConfig.AreaList == null)
            {
                return;
            }
            Display.Display.SetAll(Color.Black);

            foreach (var aArea in Display.Display.LayoutConfig.AreaList)
            {

                foreach (var aCharSet in Display.Display.CharacterSets)
                {
                    if (aCharSet.Characters == null)
                        continue;

                    foreach(var aChar in aCharSet.Characters)
                    {
                        if (EffectCancellationToken.IsCancellationRequested)
                        {
                            Console.WriteLine("CharTest stopped.");
                            break;
                        }

                        Console.WriteLine("Show Character {0} of Charset {1} in Area {2}", aChar.Char, aCharSet.Name ,aArea.Name);
                        Display.Display.ShowChar(aChar, aArea.Name);
                        Display.Display.Render();
                        Thread.Sleep(100);
                    }

                    if (EffectCancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("CharTest stopped.");
                        break;
                    }
                }

                if (EffectCancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("CharTest stopped.");
                    break;
                }
            }
        }

        
    }
}
