using LeDi.Display2.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeDi.Shared2.Display;
using LeDi.Display2.Effects;

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
        public static void ExecuteCommand(string command)
        {
            IEffect effect = new NoEffect();
            switch (command)
            {
                case "showareas":
                    Logger.Info("Running command \"showareas\"...");
                    effect = new TestArea();
                    break;
            }

            if (effect != null)
            {
                Logger.Debug("Running effect {0}...", command);
                try
                {
                    effect.Execute();
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
