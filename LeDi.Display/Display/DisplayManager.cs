﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
using LeDi.Display.Effects;
using LeDi.Shared;

namespace LeDi.Display.Display
{
    public class DisplayManager
    {
        /// <summary>
        /// Is the DisplayManager initalized?
        /// </summary>
        public bool IsInitialized { get { return Display.LayoutConfig == null ? false : true; } }

        /// <summary>
        /// Timer to query commands from server
        /// </summary>
        private readonly System.Timers.Timer _TmrMisc = new(2000);
        private Api Api = new Api();
        private Connector _Connector;
        private bool _MiscRunning = false;

        public DisplayManager(Layout layout, Connector connector)
        {
            Initialize(layout);
            _Connector = connector;
            _TmrMisc.Elapsed += _TmrMisc_Elapsed;
            _TmrMisc.Start();
        }

        private async void _TmrMisc_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_MiscRunning)
                return;

            _MiscRunning = true;

            var commands = await _Connector.GetDeviceCommands();
            if (commands == null || commands.Count == 0)
            {
                _MiscRunning = false;
                return;
            }

            foreach(var aCmd in commands)
            {
                IEffect effect = new NoEffect();
                switch (aCmd.Command)
                {
                    case "showareas":
                        effect = new TestArea();
                        break;
                    case "showtestpattern":
                        effect = new TestPattern();
                        break;
                    case "showcolortest":
                        effect = new TestColorWipe();
                        break;
                    case "showfullcolortest":
                        effect = new TestFullColor();
                        break;
                    case "showclock":
                        Display.SetAll(Color.Black);
                        Display.ShowString(DateTime.Now.ToString("HH:mm"), null, null, false, 5, 3);
                        Display.Render();
                        break;
                    case "idlebar":
                        effect = new IdleBar();
                        break;
                    case "calibratefps":
                        Display.SetAll(Color.Black);
                        Display.Calibrate();
                        Display.ShowString(Display.FPS.ToString("F0"), null, null, false, 5, 3);
                        Display.Render();
                        break;
                    case "calibratebrightness":
                        Display.SetBrightness(255);
                        effect = new TestBrightness();
                        break;
                    case "reload":
                        Console.WriteLine("Reloading Display settings...");
                        await _Connector.LoadLocalDeviceConfigAsync();
                        await _Connector.RegisterDevice();
                        var layout = await _Connector.GetDeviceSettings();
                        if (layout != null)
                            Initialize(layout, true);
                        Console.WriteLine("Display settings reloaded...");
                        break;
                    case "restartsoft":
                        Display.SetAll(Color.Black);
                        Display.SetLed(1, Color.Red);
                        Display.Render();

                        //Remove the command already here. Otherwise it will not being removed because the system/service stops too fast.
                        await Api.RemoveDeviceCommand(aCmd);

                        try
                        {
                            System.Diagnostics.Process procSoft = new System.Diagnostics.Process();
                            procSoft.StartInfo.FileName = "/bin/bash";
                            procSoft.StartInfo.Arguments = "-c \"/usr/bin/systemctl restart ledi.display\"";
                            procSoft.StartInfo.UseShellExecute = false;
                            procSoft.StartInfo.RedirectStandardOutput = true;
                            procSoft.Start();
                        }
                        catch(Exception ea)
                        {
                            Console.WriteLine("Failed to run command. Error: " + ea.ToString());
                        }

                        break;
                    case "restarthard":
                        Display.SetAll(Color.Black);
                        Display.SetLed(1, Color.Red);
                        Display.SetLed(2, Color.Red);
                        Display.Render();

                        //Remove the command already here. Otherwise it will not being removed because the system/service stops too fast.
                        await Api.RemoveDeviceCommand(aCmd);

                        try
                        {
                            System.Diagnostics.Process procHard = new System.Diagnostics.Process();
                            procHard.StartInfo.FileName = "/bin/bash";
                            procHard.StartInfo.Arguments = "-c \"/usr/sbin/shutdown -r now\"";
                            procHard.StartInfo.UseShellExecute = false;
                            procHard.StartInfo.RedirectStandardOutput = true;
                            procHard.Start();
                        }
                        catch (Exception ea)
                        {
                            Console.WriteLine("Failed to run command. Error: " + ea.ToString());
                        }

                break;
                    case "shutdown":
                        Display.SetAll(Color.Black);
                        Display.SetLed(1, Color.Red);
                        Display.SetLed(2, Color.Red);
                        Display.SetLed(3, Color.Red);
                        Display.Render();

                        //Remove the command already here. Otherwise it will not being removed because the system/service stops too fast.
                        await Api.RemoveDeviceCommand(aCmd);

                        try
                        { 
                            System.Diagnostics.Process procDown = new System.Diagnostics.Process();
                            procDown.StartInfo.FileName = "/bin/bash";
                            procDown.StartInfo.Arguments = "-c \"/usr/sbin/shutdown -h now\"";
                            procDown.StartInfo.UseShellExecute = false;
                            procDown.StartInfo.RedirectStandardOutput = true;
                            procDown.Start();
                        }
                        catch (Exception ea)
                        {
                            Console.WriteLine("Failed to run command. Error: " + ea.ToString());
                        }

                        break;
                    default:
                        Console.WriteLine("Unknown command {0}", aCmd.Command);
                        break;
                }
                if (effect != null)
                {
                    Console.WriteLine("Running Effect {0}...", aCmd.Command);
                    try
                    {
                        effect.Execute();
                    }
                    catch(Exception ea)
                    {
                        Console.WriteLine("Stopped effect {0} because of Error: {1}", aCmd.Command, ea.ToString());
                    }
                    Display.SetBrightness(Display.Brightness); //Setting Brightness back to configured value in case it was changed for calibration tests
                    Console.WriteLine("Effect {0} done...", aCmd.Command);
                    await Api.RemoveDeviceCommand(aCmd);
                }
            }
            
            _MiscRunning = false;
            
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
        public void Initialize(Layout layout, bool force = false)
        {
            if ((Display.LayoutConfig == null || force) && layout != null)
            {
                Display.Initialize(layout);
            }
        }

    }
}