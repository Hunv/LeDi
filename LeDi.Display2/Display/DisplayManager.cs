using LeDi.Display2.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeDi.Shared2.Display;
using LeDi.Display2.Effects;
using BlazorBootstrap;
using System.Drawing;
using LeDi.Shared2.EffectParameters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.JSInterop;

namespace LeDi.Display2.Display
{
    internal static class DisplayManager
    {
        /// <summary>
        /// The current Mode to show content
        /// </summary>
        private static string Mode { get; set; } = "none";

        /// <summary>
        /// The path the the config file.
        /// </summary>
        private static readonly string ConfigFilename = "./config.json";

        /// <summary>
        /// Is the display initialized?
        /// </summary>
        private static bool IsInitialized = false;

        /// <summary>
        /// The configurable settings of this device
        /// </summary>
        private static DeviceConfig? Config { get; set; } = null;

        /// <summary>
        /// Logger instance
        /// </summary>
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Connector that handles the server connection
        /// </summary>
        private static Connector? Connector { get; set; } = null;

        /// <summary>
        /// The currently running effect
        /// </summary>
        private static IEffect EffectActive { get; set; } = new NoEffect();

        /// <summary>
        /// The Task running the current EffectActive
        /// </summary>
        private static Task? EffectTask { get; set; } = null;

        /// <summary>
        /// Cancelation Token source to cancel running effect
        /// </summary>
        private static CancellationTokenSource EffectCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// The cancellation token to cancel running effect
        /// </summary>
        private static CancellationToken EffectCancellationToken = EffectCancellationTokenSource.Token;

        /// <summary>
        /// Must be performed before the display works
        /// </summary>
        public async static void Initialize()
        {
            //Load the config
            Config = await LoadConfig();

            if (Config == null)
            {
                Logger.Fatal("Cannot load config file.");
                return;
            }

            //Initialize the Connector
            Connector = new Connector(Config.ServerURL);

            //Connect the device to the server
            await Connector.Connect();

            //Register events of Connector
            Connector.DataUpdateReceived += Connector_DataUpdateReceived;

            //Ensure registered or register if not
            RegisterDevice();

            //Initialize Display Layout
            InitializeDisplay();

            //Get current data to show
            await Connector.RequestUpdate(Config.DeviceId);

            // Set initialized variable to true
            IsInitialized = true;
        }

        #region Connector Events
        private static void Connector_DataUpdateReceived(object? sender, DataUpdateEventArgs e)
        {
            if (e.Mode != null)
                Mode = e.Mode;
        }

        #endregion

        /// <summary>
        /// Initialize the Display Controller
        /// </summary>
        /// <returns></returns>
        private static void InitializeDisplay()
        {
            Logger.Trace("GetDeviceSettings...");

            if (Config.DeviceId == null)
            {
                Logger.Error("Cannot load Device Settings. DeviceId not set.");
                return;
            }

            var layout = new Layout
            {
                Width = Config.DeviceWidth,
                Height = Config.DeviceHeight,
                Brightness = Config.Brightness,
                Frequency = Config.Frequency,
                DmaChannel = Config.DMAChannel,
                CharacterSet = Config.CharacterSet,
                HardAreaBorders = Config.HardAreaBorders
            };

            Display.IsBottomToTop = Config.LedTopToBottom;
            Display.HasAlternatingRows = Config.LedAlternatingRows;
            Display.IsLeftToRight = Config.LedFirstLedLeft;

            //Set the layout name in case it was not set before manually
            if (layout.Name == null)
                layout.Name = layout.Width + "x" + layout.Height;

            Logger.Info("Layout to use: {0}", layout.Name);

            // currently unhandled settings
            //    layout.CharacterList
            //    layout.AreaList

            Display.Initialize(layout);
        }

        /// <summary>
        /// Register device at the server if not already done.
        /// </summary>        
        private static async void RegisterDevice()
        {
            if (string.IsNullOrWhiteSpace(Config.DeviceId))
            {
                Config.DeviceId = Guid.NewGuid().ToString();
                Logger.Info("New DeviceId is {0}", Config.DeviceId);
                await SaveConfig();
            }

            await Connector.RegisterDevice(Config.DeviceId, Config.DeviceName, Config.DeviceType, Config.DeviceModel);
        }

        /// <summary>
        /// Loads the config from the config.json file
        /// </summary>
        private static async Task<DeviceConfig?> LoadConfig()
        {
            try
            {
                Logger.Info("Loading config {0}. Current Directory is {1}", ConfigFilename, Environment.CurrentDirectory);
                StreamReader sR = new StreamReader(ConfigFilename);
                var json = await sR.ReadToEndAsync();
                sR.Close();
                Logger.Trace("Loaded json content is {0}", json);
                var config = JsonConvert.DeserializeObject<DeviceConfig>(json, GetJsonSerializer());

                if (config != null)
                {
                    Logger.Debug("Server Setting value: {0}", config.ServerURL);
                    Logger.Info("Successfully loaded config.");
                    return config;
                }

                Logger.Error("Loading the config file returns a null value.");
                return null;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Unable to load config. Does file {0} exists?", ConfigFilename);
                return null;
            }
        }

        /// <summary>
        /// Save the config to config.json
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> SaveConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Config, GetJsonSerializer());
                StreamWriter sW = new StreamWriter(ConfigFilename);
                await sW.WriteAsync(json);
                sW.Close();

                Logger.Debug("Saved config file.");
                return true;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Unable to write config to {0}", ConfigFilename);
                return false;
            }
        }

        /// <summary>
        /// Helper method to get the Json Serializer settings
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerSettings GetJsonSerializer()
        {
            var js = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };

            return js;
        }

        /// <summary>
        /// Execute a command at the display
        /// </summary>
        /// <param name="command"></param>
        public static async Task ExecuteCommand(string command, string? parameter = null)
        {
            IEffect? effect = null;
            Logger.Info("Setting effect \"" + command + "\"...");
            switch (command)
            {
                #region main commands
                case "clock": //Todo: Make an effect of this and always show the current time.
                    Console.WriteLine("Running Clock - needs to be done as effect");
                    Display.ShowString(DateTime.Now.ToString("HH:mm"));
                    Display.Render();
                    effect = null;
                    break;

                case "text":
                    break;

                case "countdown":
                    if (parameter == null)
                    {
                        Logger.Warn("Effect countdown requires a parameter but no parameter given.");
                        return;
                    }
                    effect = new Countdown();

                    // Get the parameter object from the Json Element of the parsed parameter
                    var para = (CountdownParameters)JsonConvert.DeserializeObject(parameter,typeof(CountdownParameters));

                    Logger.Debug("Countdown Parameters: Seconds ({0}), Text ({1})", para.Seconds, para.Text);                    
                    ((Countdown)effect).Seconds = para.Seconds;
                    ((Countdown)effect).Text = para.Text;
                    break;

                case "match":
                    break;

                case "tournament":
                    break;

                #endregion

                #region testing commands
                case "black":
                    effect = new TestBlack();
                    break;

                case "areas":
                    effect = new TestArea();
                    break;

                case "testpattern":
                    effect = new TestPattern();
                    break;

                case "colortest":
                    effect = new TestColorWipe();
                    break;

                case "fullcolortest":
                    effect = new TestFullColor();
                    break;

                case "pixelwipe":
                    effect = new TestPixelWipe();
                    break;
                #endregion

                #region screensaver commands
                case "idlebar":
                    effect = new IdleBar();
                    break;

                case "ledilogo":
                    effect = new LeDiLogo();
                    break;
                #endregion

                #region calibration
                case "calibratefps":
                    Display.SetAll(Color.Black);
                    Display.Calibrate();
                    Display.ShowString(Display.FPS.ToString("F0"), null, null, false, 5, 3);
                    Display.Render();
                    effect = null;
                    break;

                case "calibratebrightness":
                    effect = new TestBrightness();
                    break;
                #endregion

                #region System commands
                case "reload":
                    Config = await LoadConfig();
                    await Connector.Connect();
                    await Connector.RegisterDevice(Config.DeviceId, Config.DeviceName, Config.DeviceType, Config.DeviceModel);
                    InitializeDisplay();                    
                    Logger.Debug("Display settings reloaded...");
                    break;

                case "restartsoft":
                    Logger.Info("Running command \"restartsoft\"...");
                    Display.SetAll(Color.Black);
                    Display.SetLed(1, Color.Red);
                    Display.Render();

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
                    catch (Exception ea)
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

                    try
                    {
                        Logger.Info("Executing restart of hardware device. See you soon...");

                        Display.ShowString("See you...");
                        Display.SetLed(1, Color.Red);
                        Display.SetLed(2, Color.Red);
                        Display.Render();

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

                    try
                    {
                        Logger.Info("Executing shutdown of hardware device. Goodbye...");

                        Display.SetLed(1, Color.Red);
                        Display.SetLed(2, Color.Red);
                        Display.SetLed(3, Color.Red);
                        Display.ShowString("Bye.");
                        Display.Render();

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
                    #endregion
            }


            // Check if it was an effect (can also be a systemcommand)
            if (effect != null)
            {
                Logger.Debug("Running effect {0}...", command);
                try
                {
                    // Cancel current effect and wait for finish
                    EffectCancellationTokenSource.Cancel();
                    if (EffectTask != null)
                    {
                        try
                        {
                            await EffectTask.WaitAsync(EffectCancellationToken);
                        }
                        catch(TaskCanceledException) { }                        
                        //await Task.Factory.ContinueWhenAll(new Task[] { EffectTask }, completed => { });
                    }

                    // Create new cancellation token and run the new effect
                    EffectCancellationTokenSource = new CancellationTokenSource();
                    EffectCancellationToken = EffectCancellationTokenSource.Token;
                    EffectActive = effect;
                    EffectTask = Task.Factory.StartNew(() => { EffectActive.Execute(EffectCancellationToken); }, EffectCancellationToken); // Execute and "forget". Don't care about result.                    
                }
                catch (Exception ea)
                {
                    Logger.Error("Stopped effect {0} because of Error: {1}", command, ea.ToString());
                }
                Display.SetBrightness(Display.Brightness); //Setting Brightness back to configured value in case it was changed for calibration tests
                Logger.Debug("Effect {0} done...", command);
            }
        }

   
    }
}
