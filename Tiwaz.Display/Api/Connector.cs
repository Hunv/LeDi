using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiwaz.Shared.DtoModel;
using Tiwaz.Shared;

namespace Tiwaz.Display.Api
{
    public class Connector
    {
        private string? ServerUrl;
        public string? Layout { get; set; }

        public Shared.Api SrvApi = new();

        public Connector()
        {   
        }

        /// <summary>
        /// Gets the config of the current device
        /// </summary>
        /// <returns></returns>
        public async Task LoadLocalDeviceConfigAsync()
        {
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("unable to load config.json.");
                return;
            }

            var sR = new StreamReader("config.json");
            var rulesJson = await sR.ReadToEndAsync();

            var jsonSerializer = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };

            var config = JsonConvert.DeserializeObject<Config>(rulesJson, jsonSerializer);

            if (config == null)
            {
                Console.WriteLine("Unable to read config values from config.json");
                return;
            }

            ServerUrl = config.ServerUrl;
            Layout = config.Layout;

            Console.WriteLine("Config loaded.");
        }

        public async Task RegisterDevice()
        {
            Console.WriteLine("Registering Device...");
            var json = "{\"DeviceType\":\"Display\",\"DeviceModel\":\"LED Screen\",\"DeviceId\":\"\"}";

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

                Console.WriteLine("Object ID: {0}", deviceObj.DeviceId);
                //Todo: Save the received deviceId
            }
            else
            {
                Console.WriteLine("Failed to register device.");
            }
        }
    }
}
