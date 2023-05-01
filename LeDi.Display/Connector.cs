using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeDi.Shared.DtoModel;
using LeDi.Shared;
using LeDi.Display.Display;

namespace LeDi.Display
{
    public class Connector
    {
        private string? ServerUrl;
        private string ConfigFilename = "config.conf";
        private Api SrvApi = new();

        public string? Layout { get; set; }
        public string? DeviceId { get; set; }
        public string DeviceModel = "LeDi Display";
        public string DeviceType = "LED Screen";
        public string DeviceName = "LeDi";
        public int DeviceWidth = 1;
        public int DeviceHeight = 1;
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public Connector()
        {
        }

        /// <summary>
        /// Gets the local config of the current device
        /// </summary>
        /// <returns></returns>
        public async Task LoadLocalDeviceConfigAsync()
        {
            Logger.Trace("Loading local device config...");
            
            if (!File.Exists(ConfigFilename))
            {
                Logger.Fatal("Unable to load " + ConfigFilename);
                return;
            }

            // Read the config file
            var sR = new StreamReader(ConfigFilename);
            while (sR.Peek() >= 0) {
                var line = await sR.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || !line.Contains(':'))
                    continue;

                var lineSplit = line.Split(new char[] { ':' }, 2);
                switch (lineSplit[0].ToLower())
                {
                    case "serverurl":
                        ServerUrl = lineSplit[1];
                        break;
                    case "deviceid":
                        DeviceId = lineSplit[1];
                        break;
                    case "devicemodel":
                        DeviceModel = lineSplit[1];
                        break;
                    case "devicetype":
                        DeviceType = lineSplit[1];
                        break;
                    default:
                        Logger.Warn("Unknown Setting {0} with value {1}", lineSplit[0], lineSplit[1]);
                        break;
                }
                
            }
            sR.Close();

            //If serverUrl not set, the config may not being accessible
            if (ServerUrl == null)
            {
                Logger.Fatal("Unable to read config values from {0} or the ServerUrl value is missing.", ConfigFilename);
                return;
            }

            //Initialize the Api Interface using the ServerUrl:
            SrvApi = new Api(ServerUrl);

            Logger.Info("Config {0} loaded.", ConfigFilename);
        }

        public async Task RegisterDevice()
        {
            try
            {
                Logger.Trace("RegisterDevice executed.");

                if (DeviceId == null)
                { 
                    //Create DeviceId
                    var sR = new StreamReader(ConfigFilename);
                    var config = (await sR.ReadToEndAsync()).Split('\n');
                    sR.Close();

                    DeviceId = Guid.NewGuid().ToString();
                    Logger.Info("New DeviceID: {0}", DeviceId);

                    //Write new config file
                    var sW = new StreamWriter(ConfigFilename, true);
                    await sW.WriteLineAsync("\nDeviceId:" + DeviceId);
                    sW.Close();
                    Logger.Debug("Updated deviceid in config file");
                }

                var json = "{\"DeviceType\":\"" + DeviceType + "\",\"DeviceModel\":\"" + DeviceModel + "\",\"DeviceId\":\"" + DeviceId + "\", \"DeviceName\":\"" + DeviceName + "\"}";

                var responseBody = await SrvApi.RegisterDevice(json);

                if (responseBody != null)
                {
                    try
                    {
                        Logger.Info("Device connected.");

                        if (responseBody == null)
                        {
                            Logger.Debug("No response body received.");
                            return;
                        }

                        //Convert response to object
                        var deviceObj = (DtoDevice?)JsonConvert.DeserializeObject(responseBody, typeof(DtoDevice), Helper.GetJsonSerializer());

                        if (deviceObj == null)
                        {
                            Logger.Warn("Device Object answer is null.");
                            return;
                        }

                        Logger.Info("Device ID: {0}", deviceObj.DeviceId);
                        if (deviceObj.DeviceId == DeviceId)
                        {
                            Logger.Debug("Device ID confirmed.");
                        }
                        else
                        {
                            Logger.Error("Device ID received from Server does not match. Canceling...");
                            return;
                        }
                    }
                    catch (Exception ea)
                    {
                        Logger.Error(ea, "An error occured on register device.");
                    }
                }
                else
                {
                    Logger.Error("Failed to register device.");
                }
            }
            catch(Exception ea)
            {
                Logger.Error("Cannot register because of an Error: ", ea.ToString());
            }
        }

        /// <summary>
        /// Gets Device settings from the Server
        /// </summary>
        /// <returns></returns>
        public async Task<Layout?> GetDeviceSettings()
        {
            Logger.Trace("GetDeviceSettings...");

            if (DeviceId == null)
            {
                Logger.Error("Cannot load Device Settings. DeviceId not set.");
                return null;
            }

            var layout = new Layout();
            var settings = await SrvApi.GetDeviceSettingsAsync(DeviceId);

            if (settings == null)
            {
                Logger.Error("Failed to load settings from Server.");
                return null;
            }

            foreach(var aSetting in settings)
            {
                switch (aSetting.Name.ToLower())
                {
                    case "width":
                        layout.Width = Convert.ToInt32(aSetting.Value);
                        Logger.Info("Loaded width of {0}", layout.Width);
                        break;
                    case "height":
                        layout.Height = Convert.ToInt32(aSetting.Value);
                        Logger.Info("Loaded height of {0}", layout.Height);
                        break;
                    case "brightness":
                        layout.Brightness = Convert.ToByte(aSetting.Value);
                        break;
                    case "frequency":
                        layout.Frequency = Convert.ToUInt32(aSetting.Value);
                        break;
                    case "dmachannel":
                        layout.DmaChannel = Convert.ToInt32(aSetting.Value);
                        break;
                    case "characterset":
                        layout.CharacterSet = aSetting.Value;
                        break;
                    case "layoutname":
                        layout.Name = aSetting.Value;
                        break;
                    case "hardborders":
                        layout.HardAreaBorders = Convert.ToBoolean(aSetting.Value);
                        break;
                    case "led_toptobottom":
                        Display.Display.IsBottomToTop = Convert.ToBoolean(aSetting.Value);
                        break;
                    case "led_alternatingrows":
                        Display.Display.HasAlternatingRows = Convert.ToBoolean(aSetting.Value);
                        break;
                    case "led_firstledleft":
                        Display.Display.IsLeftToRight = Convert.ToBoolean(aSetting.Value);
                        break;
                }
            }

            //Set the layout name in case it was not set before manually
            if (layout.Name == null)
                layout.Name = layout.Width + "x" + layout.Height;

            Logger.Info("Layout to use: {0}", layout.Name);

            // currently unhandled settings
            //    layout.CharacterList
            //    layout.AreaList
                
            return layout;
        }

        /// <summary>
        /// Gets the DeviceCommands from the Server
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoDeviceCommand>?> GetDeviceCommands()
        {
            Logger.Trace("GetDeviceCommands...");

            if (DeviceId == null)
            {
                Logger.Error("Cannot get DeviceCommands. DeviceId is not set.");
                return null;
            }
            return await SrvApi.GetDeviceCommandAsync(DeviceId);
        }
    }
}
