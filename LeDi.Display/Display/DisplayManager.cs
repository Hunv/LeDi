using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
using LeDi.Display.Effects;
using LeDi.Shared;
using LeDi.Shared.DtoModel;

namespace LeDi.Display.Display
{
    public class DisplayManager
    {
        /// <summary>
        /// Is the DisplayManager initalized?
        /// </summary>
        public static bool IsInitialized { get { return Display.LayoutConfig != null; } }

        /// <summary>
        /// Contains a hash of all match values, that can change (except time).
        /// </summary>
        int LastKnownMatchHash = 0;

        /// <summary>
        /// This match, that is currently shown on this device
        /// </summary>
        public DtoMatch? Match { get; set; }

        /// <summary>
        /// The action, that is currently shown on the display. i.e. "match", "time", "text"
        /// </summary>
        public DisplayActionEnum CurrentAction { get; set; } = DisplayActionEnum.None;

        /// <summary>
        /// action parameter (i.e. the text to show, start time of the countdown, ...)
        /// </summary>
        public object? ActionParameter { get; set; }

        /// <summary>
        /// Timer to query commands from server
        /// </summary>
        private readonly System.Timers.Timer _TmrMisc = new(1000);
        private readonly Api Api = new Api();
        private readonly Connector _Connector;
        private bool _MiscRunning = false;
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DisplayManager(Layout layout, Connector connector)
        {
            Logger.Info("Initializing DisplayManager...");
            Initialize(layout);
            _Connector = connector;
            _TmrMisc.Elapsed += _TmrMisc_Elapsed;
            _TmrMisc.Start();

            //Set the first row of LEDs to green
            for (int i = 0; i < Display.X; i++)
            {
                Display.SetLed(i, Color.Green);
            }
            Display.Render();
        }

        private async void _TmrMisc_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Logger.Trace("TmrMisc elapsed");

            if (_MiscRunning)
            {
                Logger.Debug("TmrMisc skipped due to previous run is still running");
                return;
            }

            _MiscRunning = true;

            // Check the current action of the display
            switch (CurrentAction)
            {
                default:
                case DisplayActionEnum.None:
                    break;
                case DisplayActionEnum.Match:
                    if (Match != null)
                        await ShowMatch();
                    break;
                case DisplayActionEnum.Time:
                    ShowTime();
                    break;
                case DisplayActionEnum.Text:
                    ShowText();
                    break;
                case DisplayActionEnum.Countdown:
                    ShowCountdown();
                    break;
            }

            // Check for new commands
            var commands = await _Connector.GetDeviceCommands();
            if (commands == null || commands.Count == 0)
            {
                _MiscRunning = false;
                Logger.Trace("No commands to execute.");
                return;
            }

            // Run the new commands
            foreach(var aCmd in commands)
            {
                IEffect effect = new NoEffect();
                switch (aCmd.Command)
                {
                    case "showtime":
                        Logger.Info("Running command \"showtime\"...");
                        CurrentAction = DisplayActionEnum.Time;
                        break;
                    case "showtext":
                        Logger.Info("Running command \"showtext\"...");
                        CurrentAction = DisplayActionEnum.Text;
                        ActionParameter = aCmd.Parameter;
                        break;
                    case "showcountdown":
                        Logger.Info("Running command \"showcountdown\"...");
                        CurrentAction = DisplayActionEnum.Countdown;
                        ActionParameter = aCmd.Parameter;
                        break;
                    case "loadmatch":
                        Logger.Info("Running command \"loadmatch\"...");
                        if (_Connector.DeviceId == null)
                        {
                            Logger.Warn("Cannot load match. DeviceId is not set.");
                        }
                        else
                        {
                            var devSet = await Api.GetDeviceSettingAsync(_Connector.DeviceId, "matchid");
                            if (devSet == null)
                            {
                                Logger.Warn("No match to show. matchid setting is not set.");
                                Display.ShowString("No Match");
                                Display.Render();
                            }
                            else 
                            {
                                Match = await Api.GetMatchFullAsync(Convert.ToInt32(devSet.Value));

                                if (Match == null)
                                {
                                    Logger.Error("Failed to update match.");
                                }
                                else
                                {
                                    CurrentAction = DisplayActionEnum.Match;
                                    Logger.Debug("Match set to match ID {0}", Match.Id);
                                    Display.SetAll(Color.Black);
                                    Display.ShowString(Match.Team1Name ?? "Team1", "team1name");
                                    Display.ShowString(Match.Team2Name ?? "Team2", "team2name");
                                    Display.ShowString((Match.Team1Score ?? 0).ToString(), "team1goals");
                                    Display.ShowString((Match.Team2Score ?? 0).ToString(), "team2goals");
                                    Display.ShowString(":", "goaldivider");
                                    await ShowMatch();
                                }
                            }
                        }
                        break;
                    case "showareas":
                        Logger.Info("Running command \"showareas\"...");
                        effect = new TestArea();
                        break;
                    case "showtestpattern":
                        Logger.Info("Running command \"showtestpattern\"...");
                        effect = new TestPattern();
                        break;
                    case "showcolortest":
                        Logger.Info("Running command \"showcolortest\"...");
                        effect = new TestColorWipe();
                        break;
                    case "showfullcolortest":
                        Logger.Info("Running command \"showfullcolortest\"...");
                        effect = new TestFullColor();
                        break;
                    case "showclock":
                        Logger.Info("Running command \"showclock\"...");
                        Display.SetAll(Color.Black);
                        Display.ShowString(DateTime.Now.ToString("HH:mm"), null, null, false, 5, 3);
                        Display.Render();
                        break;
                    case "idlebar":
                        Logger.Info("Running command \"idlebar\"...");
                        effect = new IdleBar();
                        break;
                    case "calibratefps":
                        Logger.Info("Running command \"calibratefps\"...");
                        Display.SetAll(Color.Black);
                        Display.Calibrate();
                        Display.ShowString(Display.FPS.ToString("F0"), null, null, false, 5, 3);
                        Display.Render();
                        break;
                    case "calibratebrightness":
                        Logger.Info("Running command \"calibratebrightness\"...");
                        Display.SetBrightness(255);
                        effect = new TestBrightness();
                        break;
                    case "reload":
                        Logger.Info("Running command \"reload\"...");
                        await _Connector.LoadLocalDeviceConfigAsync();
                        await _Connector.RegisterDevice();
                        var layout = await _Connector.GetDeviceSettings();
                        if (layout != null)
                            Initialize(layout, true);
                        Logger.Debug("Display settings reloaded...");
                        break;
                    case "restartsoft":
                        Logger.Info("Running command \"restartsoft\"...");
                        Display.SetAll(Color.Black);
                        Display.SetLed(1, Color.Red);
                        Display.Render();

                        //Remove the command already here. Otherwise it will not being removed because the system/service stops too fast.
                        await Api.RemoveDeviceCommand(aCmd);

                        try
                        {
                            Logger.Info("Executing restart of ledi.display daemon...");
                            System.Diagnostics.Process procSoft = new System.Diagnostics.Process();
                            procSoft.StartInfo.FileName = "/bin/bash";
                            procSoft.StartInfo.Arguments = "-c \"/usr/bin/systemctl restart ledi.display\"";
                            procSoft.StartInfo.UseShellExecute = false;
                            procSoft.StartInfo.RedirectStandardOutput = true;
                            procSoft.Start();
                        }
                        catch(Exception ea)
                        {
                            Logger.Error("Failed to run command. Error: " + ea.ToString());
                        }

                        break;
                    case "restarthard":
                        Logger.Info("Running command \"restarthard\"...");
                        Display.SetAll(Color.Black);
                        Display.SetLed(1, Color.Red);
                        Display.SetLed(2, Color.Red);
                        Display.Render();

                        //Remove the command already here. Otherwise it will not being removed because the system/service stops too fast.
                        await Api.RemoveDeviceCommand(aCmd);

                        try
                        {
                            Logger.Info("Executing restart of hardware device. See you soon...");
                            System.Diagnostics.Process procHard = new System.Diagnostics.Process();
                            procHard.StartInfo.FileName = "/bin/bash";
                            procHard.StartInfo.Arguments = "-c \"/usr/sbin/shutdown -r now\"";
                            procHard.StartInfo.UseShellExecute = false;
                            procHard.StartInfo.RedirectStandardOutput = true;
                            procHard.Start();
                        }
                        catch (Exception ea)
                        {
                            Logger.Error("Failed to run command. Error: " + ea.ToString());
                        }

                        break;
                    case "shutdown":
                        Logger.Info("Running command \"shutdown\"...");
                        Display.SetAll(Color.Black);
                        Display.SetLed(1, Color.Red);
                        Display.SetLed(2, Color.Red);
                        Display.SetLed(3, Color.Red);
                        Display.Render();

                        //Remove the command already here. Otherwise it will not being removed because the system/service stops too fast.
                        await Api.RemoveDeviceCommand(aCmd);

                        try
                        {
                            Logger.Info("Executing shutdown of hardware device. Goodbye...");
                            System.Diagnostics.Process procDown = new System.Diagnostics.Process();
                            procDown.StartInfo.FileName = "/bin/bash";
                            procDown.StartInfo.Arguments = "-c \"/usr/sbin/shutdown -h now\"";
                            procDown.StartInfo.UseShellExecute = false;
                            procDown.StartInfo.RedirectStandardOutput = true;
                            procDown.Start();
                        }
                        catch (Exception ea)
                        {
                            Logger.Error("Failed to run command. Error: " + ea.ToString());
                        }

                        break;
                    default:
                        Logger.Warn("Unknown command {0}", aCmd.Command);
                        break;
                }
                if (effect != null)
                {
                    Logger.Debug("Running Effect {0}...", aCmd.Command);
                    try
                    {
                        effect.Execute();
                    }
                    catch(Exception ea)
                    {
                        Logger.Error("Stopped effect {0} because of Error: {1}", aCmd.Command, ea.ToString());
                    }
                    Display.SetBrightness(Display.Brightness); //Setting Brightness back to configured value in case it was changed for calibration tests
                    Logger.Debug("Effect {0} done...", aCmd.Command);
                    await Api.RemoveDeviceCommand(aCmd);
                }
            }
            
            _MiscRunning = false;
            
        }

        private void ShowCountdown()
        {
            if (ActionParameter != null && ActionParameter.GetType() == typeof(int))
            {
                var countdown = (int)ActionParameter;
                Display.ShowString(countdown.ToString());
                countdown--;
                ActionParameter = countdown;
            }
            else
            {
                Logger.Warn("Cannot show countdown, because ActionParameter is not set.");
            }
        }

        private void ShowText()
        {
            if (ActionParameter != null && ActionParameter.GetType() == typeof(string))
            {
#pragma warning disable CS8604 // Possible null reference argument.
                Display.ShowString(ActionParameter.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
                Display.Render();
            }
            else
            {
                Logger.Warn("Cannot show text, because ActionParameter is not set.");
            }
        }

        /// <summary>
        /// Shows the current time on the display
        /// </summary>
        /// <returns></returns>
        private void ShowTime()
        {
            var timeString = DateTime.Now.ToString("HH:mm");
            Display.ShowString(timeString);
        }

        /// <summary>
        /// Shows the status of a match
        /// </summary>
        private async Task ShowMatch()
        {
            if (Match == null)
            {
                Logger.Debug("ShowMatch was called but no match was configured.");
                return;
            }

            // Get the time left and the hash of all match properties
            var matchCore = await Api.GetMatchCoreAsync(Match.Id);

            // If the core value query failed, cancel.
            if (matchCore == null)
            {
                Logger.Error("Failed to get MatchCore");
                return;
            }

            // Update match object in case the hash is different
            if (LastKnownMatchHash != matchCore.PropertyHash)
            {
                Logger.Info("New Hash is different. Refreshing Match infos...");
                var newMatchInfo = await Api.GetMatchAsync(Match.Id);
                if (newMatchInfo != null)
                {
                    Logger.Debug("Updated Match infos.");

                    // Update only the fields but not the whole Match object to do not flush the rules
                    Match.PeriodCurrent = newMatchInfo.PeriodCurrent;
                    Match.RulePeriodCount = newMatchInfo.RulePeriodCount;
                    Match.MatchStatus = newMatchInfo.MatchStatus;
                    Match.TimeLeftSeconds = newMatchInfo.TimeLeftSeconds;
                    Match.Team1Score = newMatchInfo.Team1Score;
                    Match.Team2Score = newMatchInfo.Team2Score;
                    Match.Penalties = newMatchInfo.Penalties;

                    // Update LEDs
                    Display.ShowString(Match.Team1Name ?? "Team1", "team1name");
                    Display.ShowString(Match.Team2Name ?? "Team2", "team2name");
                    Display.ShowString((Match.Team1Score ?? 0).ToString(), "team1goals");
                    Display.ShowString((Match.Team2Score ?? 0).ToString(), "team2goals");
                    Display.ShowString(":", "goaldivider");

                    var pent1 = "";
                    var pent2 = "";
                    // Get Penalties
                    foreach (var aPen in Match.Penalties)
                    {
                        if (aPen.PenaltyTime > 0)
                        {
                            var timeleft = Match.TimeLeftSeconds - (Match.RulePeriodLength - aPen.PenaltyTimeStart - aPen.PenaltyTime) - ((Match.RulePeriodLength ?? 0) * (Match.PeriodCurrent - 1));

                            if (timeleft >= -10) // To show the 0:00 some seconds after the penalty is over.
                            {
                                if (timeleft < 0)
                                    timeleft = 0;

                                if (aPen.TeamId == 0)
                                    pent1 += (timeleft / 60).ToString() + ":" + (timeleft.Value % 60).ToString().PadLeft(2, '0') + "#" + aPen.PlayerNumber + "(" + @aPen.PenaltyName + ")\n";
                                else
                                    pent2 += (timeleft / 60).ToString() + ":" + (timeleft.Value % 60).ToString().PadLeft(2, '0') + "#" + aPen.PlayerNumber + "(" + @aPen.PenaltyName + ")\n";
                            }
                        }
                    }

                    if (pent1 != "")
                        Display.ShowString(pent1, "team1penalties");
                    if (pent2 != "")
                        Display.ShowString(pent2, "team2penalties");

                    LastKnownMatchHash = matchCore.PropertyHash;
                }
            }

            // If local timer says, time is over check verify before stopping
            if (Match.TimeLeftSeconds == 0)
            {
                // Set the current time left to the server data
                Match.TimeLeftSeconds = matchCore.TimeLeftSeconds;

                // Stop Timer only if the Server based timer is done.
                if (Match.TimeLeftSeconds == 0)
                {
                    Logger.Info("Time of match {0} is over.", Match.Id);
                    //tmrMatchtime.Stop();

                    // If this is not the last period, show the start button to start the next period
                    if (Match.PeriodCurrent != Match.RulePeriodCount)
                    {
                        Logger.Debug("Not-the-last period is over.");
                    }
                    else
                    {
                        Logger.Debug("Last period is over.");
                    }
                }
            }
            else // update local timer
            {
                //To count the seconds more smoothly, only correct the seconds, if the diff is more than 1 second
                var serverTimeLeft = matchCore.TimeLeftSeconds;
                if (Match.TimeLeftSeconds - serverTimeLeft > 1 ||
                    Match.TimeLeftSeconds - serverTimeLeft < 1)
                {
                    Match.TimeLeftSeconds = serverTimeLeft;
                }
                else if (Match.TimeLeftSeconds > 0)
                {
                    Match.TimeLeftSeconds--;
                }

                var timeString = string.Format("{0}:{1}", (Match.TimeLeftSeconds / 60), ((Match.TimeLeftSeconds ?? 0) % 60).ToString().PadLeft(2, '0'));
                Display.ShowString(timeString, "time");
            }
            Display.Render();
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
                Logger.Warn("Display not initialized. Call initialize function first.");
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
        public void Initialize(Layout layout, bool force = false)
        {
            if ((Display.LayoutConfig == null || force) && layout != null)
            {
                Display.Initialize(layout);
            }
        }
    }
}
