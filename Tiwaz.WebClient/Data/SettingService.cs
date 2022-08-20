using Newtonsoft.Json;
using System.Text;
using Tiwaz.Shared.DtoModel;
using Tiwaz.WebClient.Data.Classes;

namespace Tiwaz.WebClient.Data
{
    public class SettingService
    {
        private static readonly string _ServerBaseUrl = "https://localhost:7077/api/";

        public SettingService()
        {
            Console.WriteLine("Using Serverbase URL " + _ServerBaseUrl);
        }


        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoSetting>?> GetSettingAsync()
        {
            var setting = new List<DtoSetting>();

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Setting"))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                setting = JsonConvert.DeserializeObject<List<DtoSetting>>(json, Helper.GetJsonSerializer());
            }

            return setting;
        }

        /// <summary>
        /// Gets a Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<DtoSetting?> GetSettingAsync(string settingName)
        {
            var setting = new DtoSetting();
            
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Setting/" + settingName))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                setting = JsonConvert.DeserializeObject<DtoSetting>(json, Helper.GetJsonSerializer());
            }

            return setting;
        }

        /// <summary>
        /// Sets a Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task SetSettingAsync(DtoSetting setting)
        {
            var json = JsonConvert.SerializeObject(setting, Helper.GetJsonSerializer());

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("PUT", _ServerBaseUrl + "Setting", json);

            if (requestMessage.Content == null)
            {
                Console.WriteLine("No content set. Not setting content type...");
            }
            else
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }
            
            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
