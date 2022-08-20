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

        public Connector()
        {   
        }

        public async Task LoadConfig()
        {
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
            
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator};

            //Create HttpClient to query the information
            HttpClient client = new HttpClient(handler);
            var requestMessage = new HttpRequestMessage()
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(ServerUrl + "Device"),
                Content = new StringContent(json)
            };

            if (requestMessage.Content == null)
            {
                Console.WriteLine("No content set. Not setting content type...");
            }
            else
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            Console.WriteLine("Query data from: {0}", requestMessage.RequestUri);

            var response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Device registered.");
                var responseBody = await response.Content.ReadAsStringAsync();

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
                Console.WriteLine("Failed to register device. Responsecode: {0}. Response: {1}", response.StatusCode, response.Content.ToString());
            }
        }
    }
}
