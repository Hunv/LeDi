using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;

namespace Tiwaz.Display.Display
{
    public class DisplayManager
    {
        /// <summary>
        /// Is the DisplayManager initalized?
        /// </summary>
        public bool IsInitialized { get { return Display.LayoutConfig == null ? false : true; } }

        public DisplayManager(string? layout)
        {
            Initialize(layout);
        }

        /// <summary>
        /// Show a text on the Display
        /// </summary>
        /// <param name="text"></param>
        /// <param name="area"></param>
        public void ShowText(string text, AreaName area, DateTime? experationTime = null)
        {
            if (Display.LayoutConfig == null)
            {
                Console.WriteLine("Display not initialized. Call initialize function first.");
                return;
            }

            if (Display.ChangeQueue == null)
                return;

            Display.ChangeQueue.Enqueue(new Tuple<string, string, DateTime?>(area.ToString(), text, experationTime));
        }

        public void Clear()
        {
            Display.SetAll(Color.Black);
        }

        /// <summary>
        /// Initializes the Display Manager
        /// </summary>
        /// <param name="layoutName"></param>
        public void Initialize(string? layoutName)
        {
            if (Display.LayoutConfig == null && layoutName != null)
            {
                Display.Initialize(layoutName);
            }
        }

    }
}
