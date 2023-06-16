using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using LeDi.Display2.Display;

namespace LeDi.Display2.Effects
{
    public class TestArea : IEffect
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override void Execute(CancellationToken EffectCancellationToken)
        {
            Console.WriteLine("Running Area Test");

            if (Display.Display.LayoutConfig == null)
            {
                Logger.Warn("No LayoutConfig set.");
                return;
            }

            if (Display.Display.LayoutConfig.AreaList == null)
            {
                Logger.Warn("No AreaList available.");
                return;
            }

            Logger.Info("Showing areas of layout {0}", Display.Display.LayoutConfig.Name);

            var colors = new Color[] {Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Cyan, Color.Purple, Color.Orange, Color.Navy, Color.White, Color.Violet, Color.Tomato, Color.Turquoise, Color.Teal, Color.Tan };
            var colorcount = 0;

            Display.Display.SetAll(Color.Black);

            foreach (var aArea in Display.Display.LayoutConfig.AreaList)
            {
                Logger.Info("Rendering area {0}", aArea.Name);
                var rnd = new Random();
                var color = Color.FromArgb(255, colors[colorcount]);

                for (int x = aArea.PositionX; x < aArea.PositionX + aArea.Width; x++)
                {
                    for (int y = aArea.PositionY; y < aArea.PositionY + aArea.Height; y++)
                    {
                        Display.Display.SetLed(Display.Display.GetLedNumber(x, y), color);
                    }
                }

                Display.Display.Render();
                colorcount++;
            }
        }
    }
}
