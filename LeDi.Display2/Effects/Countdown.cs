using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Linq;
using rpi_ws281x;
using LeDi.Shared2.Display;
using System.Threading;

namespace LeDi.Display2.Effects
{
    public class Countdown : IEffect
    {
        /// <summary>
        /// The Seconds to count down
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Text to show additionally to the countdown
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Show only seconds (i.e. 78 instead of 1:18)
        /// </summary>
        public bool ShowSecondsOnly { get; set; }

        /// <summary>
        /// The Area where the Countdown should be shown
        /// </summary>
        public string? AreaName { get; set; }

        /// <summary>
        /// The Charset to use
        /// </summary>
        public CharacterSet? CharSet { get; set; }

        private readonly System.Timers.Timer tmrCountdown = new(1000);

        private CancellationToken EffectCancellationToken = new CancellationToken();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override void Execute(CancellationToken effectCancellationToken)
        {
            EffectCancellationToken = effectCancellationToken;

            try
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    if (!Display.Display.LayoutConfig.AreaList.Any(x => x.Name == "countdownText"))
                    {
                        // Create the area for the text
                        var areaText = new Area();
                        areaText.Name = "countdownText";
                        areaText.PositionX = 0;
                        areaText.PositionY = Display.Display.Y / 2;
                        areaText.Width = Display.Display.X;
                        areaText.Height = Display.Display.Y / 2;
                        areaText.Align = "center";
                        Display.Display.LayoutConfig.AreaList.Add(areaText);

                        Logger.Debug("Created temporary area countdownText with PosX={0}, PosY={1}, Width={2}, Height={3}", areaText.PositionX, areaText.PositionY, areaText.Width, areaText.Height);
                    }

                    if (!Display.Display.LayoutConfig.AreaList.Any(x => x.Name == "countdownTime"))
                    {
                        // Create the area for the time
                        var areaTime = new Area();
                        areaTime.Name = "countdownTime";
                        areaTime.PositionX = 0;
                        areaTime.PositionY = 0;
                        areaTime.Width = Display.Display.X;
                        areaTime.Height = Display.Display.Y / 2;
                        areaTime.Align = "center";
                        Display.Display.LayoutConfig.AreaList.Add(areaTime);

                        Logger.Debug("Created temporary area countdownTime with PosX={0}, PosY={1}, Width={2}, Height={3}", areaTime.PositionX, areaTime.PositionY, areaTime.Width, areaTime.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Cannot create temporary areas.");
                return;
            }

            Logger.Info("Executing Countdown");
            tmrCountdown.Elapsed += TmrCountdown_Elapsed;
            tmrCountdown.Start();
        }

        private void TmrCountdown_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (EffectCancellationToken.IsCancellationRequested)
            {
                Logger.Info("Countdown stopped.");
                tmrCountdown.Stop();
            }

            string time = (Seconds / 60) + ":" + (Seconds % 60).ToString("00");
            if (ShowSecondsOnly)
            {
                time = Seconds.ToString();
            }

            //show it in area time if there is an area "time"
            if (string.IsNullOrWhiteSpace(Text))
            {
                Display.Display.ShowString(time);
            }
            else
            {
                Display.Display.ShowString(time, "countdownTime");
                Display.Display.ShowString(Text, "countdownText");
            }            

            Display.Display.Render();

            Seconds -= 1;
            if (Seconds < 0)
            {
                Logger.Info("Countdown finished");
                tmrCountdown.Stop();

                //Cleanup the temporary areas for the countdown
                if (Display.Display.LayoutConfig.AreaList.Any(x => x.Name == "countdownText"))
                {
                    Display.Display.LayoutConfig.AreaList.Remove(Display.Display.LayoutConfig.AreaList.Single(x => x.Name == "countdownText"));
                }

                if (Display.Display.LayoutConfig.AreaList.Any(x => x.Name == "countdownTime"))
                {
                    Display.Display.LayoutConfig.AreaList.Remove(Display.Display.LayoutConfig.AreaList.Single(x => x.Name == "countdownTime"));
                }
            }
        }
    }
}
