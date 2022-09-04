using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiwaz.Shared.DtoModel;
using Tiwaz.Shared;
using Tiwaz.Display.Display;

namespace Tiwaz.Display
{
    public class Connector
    {
        private string? ServerUrl;
        private string ConfigFilename = "config.conf";
        private Api SrvApi = new();

        public string? Layout { get; set; }
        public string? DeviceId { get; set; }
        public string DeviceModel = "TiwazDisplay";
        public string DeviceType = "LED Screen";

        
        public Connector()
        {
        }

        /// <summary>
        /// Gets the local config of the current device
        /// </summary>
        /// <returns></returns>
        public async Task LoadLocalDeviceConfigAsync()
        {
            if (!File.Exists(ConfigFilename))
            {
                Console.WriteLine("unable to load " + ConfigFilename);
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
                        Console.WriteLine("Unknown Setting {0} with value {1}", lineSplit[0], lineSplit[1]);
                        break;
                }
                
            }
            sR.Close();

            //If serverUrl not set, the config may not being accessible
            if (ServerUrl == null)
            {
                Console.WriteLine("Unable to read config values from {0} or the ServerUrl value is missing.", ConfigFilename);
                return;
            }

            //Initialize the Api Interface using the ServerUrl:
            SrvApi = new Api(ServerUrl);

            Console.WriteLine("Config loaded.");
        }

        public async Task RegisterDevice()
        {
            if (DeviceId != null)
            {
                Console.WriteLine("No registration required. Device already has Device ID {0}", DeviceId);
                return;
            }

            Console.WriteLine("Registering Device...");
            var json = "{\"DeviceType\":\"" + DeviceType + "\",\"DeviceModel\":\"" + DeviceModel + "\",\"DeviceId\":\"\"}";

            var responseBody = await SrvApi.RegisterDevice(json);

            if (responseBody != null)
            {
                Console.WriteLine("Device registered.");

                if (responseBody == null)
                {
                    Console.WriteLine("No response body received.");
                    return;
                }

                //Convert response to object
                var deviceObj = (DtoDevice?)JsonConvert.DeserializeObject(responseBody, typeof(DtoDevice), Helper.GetJsonSerializer());

                if (deviceObj == null)
                {
                    Console.WriteLine("Device Object answer is null.");
                    return;
                }

                Console.WriteLine("Device ID: {0}", deviceObj.DeviceId);
                DeviceId = deviceObj.DeviceId;
                
                //Save received deviceId to config file
                var sR = new StreamReader(ConfigFilename);
                var config = (await sR.ReadToEndAsync()).Split('\n');
                sR.Close();
                var deviceIdSet = false;
                for (int i = 0; i < config.Length; i++) 
                {
                    //Setting exists but was not properly set (otherwise this function should not be called...)
                    if (config[i].ToLower().StartsWith("deviceid"))
                    {
                        config[i] = deviceObj.DeviceId;
                        deviceIdSet = true;

                        //Write new config file
                        var sW = new StreamWriter(ConfigFilename, false);
                        foreach (var aLine in config)
                        {
                            await sW.WriteLineAsync(aLine);
                        }
                        sW.Close();

                        break;
                    }
                }

                // Setting didn't exist in the config file. Add it to the config file
                if (!deviceIdSet)
                {
                    var sW = new StreamWriter(ConfigFilename,true);
                    await sW.WriteLineAsync("\nDeviceId:" + deviceObj.DeviceId);
                    sW.Close();
                }
            }
            else
            {
                Console.WriteLine("Failed to register device.");
            }
        }

        /// <summary>
        /// Gets Device settings from the Server
        /// </summary>
        /// <returns></returns>
        public async Task<Layout?> GetDeviceSettings()
        {
            if (DeviceId == null)
            {
                Console.WriteLine("Cannot load Device Settings. DeviceId not set.");
                return null;
            }

            var layout = new Layout();
            var settings = await SrvApi.GetDeviceSettingsAsync(DeviceId);

            if (settings == null)
            {
                Console.WriteLine("Failed to load settings from Server.");
                return null;
            }

            foreach(var aSetting in settings)
            {
                switch (aSetting.Name.ToLower())
                {
                    case "width":
                        layout.Width = Convert.ToInt32(aSetting.Value);
                        break;
                    case "height":
                        layout.Height = Convert.ToInt32(aSetting.Value);
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
            if (DeviceId == null)
            {
                Console.WriteLine("Cannot get DeviceCommands. DeviceId is not set.");
                return null;
            }
            return await SrvApi.GetDeviceCommandAsync(DeviceId);
        }
    }
}
