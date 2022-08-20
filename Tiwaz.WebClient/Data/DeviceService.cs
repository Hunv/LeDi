using Newtonsoft.Json;
using System.Text;
using Tiwaz.Shared.DtoModel;
using Tiwaz.WebClient.Data.Classes;

namespace Tiwaz.WebClient.Data
{
    public class DeviceService
    {
        private static readonly string _ServerBaseUrl = "https://localhost:7077/api/";

        public DeviceService()
        {
            Console.WriteLine("Using Serverbase URL " + _ServerBaseUrl);
        }


        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoDevice>?> GetDeviceAsync()
        {
            var setting = new List<DtoDevice>();

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Device"))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                setting = JsonConvert.DeserializeObject<List<DtoDevice>>(json, Helper.GetJsonSerializer());
            }

            return setting;
        }

        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        public async Task<DtoDevice?> GetDeviceAsync(string deviceId)
        {
            var device = new DtoDevice("","","");

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Device/" + deviceId))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                device = JsonConvert.DeserializeObject<DtoDevice>(json, Helper.GetJsonSerializer());
            }

            return device;
        }


        /// <summary>
        /// Gets all Device Settings
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<List<DtoDeviceSetting>?> GetDeviceSettingsAsync(string deviceId)
        {
            var setting = new List<DtoDeviceSetting>();

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Device/" + deviceId))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                setting = JsonConvert.DeserializeObject<List<DtoDeviceSetting>>(json, Helper.GetJsonSerializer());
            }

            return setting;
        }

        /// <summary>
        /// Gets a Device Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<DtoDeviceSetting?> GetDeviceSettingAsync(string deviceId, string settingName)
        {
            var setting = new DtoDeviceSetting("","","");
            
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Device/" + deviceId + "/" + settingName))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                setting = JsonConvert.DeserializeObject<DtoDeviceSetting>(json, Helper.GetJsonSerializer());
            }

            return setting;
        }

        /// <summary>
        /// Sets a Device Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task SetDeviceSettingAsync(string deviceId, DtoDeviceSetting setting)
        {
            var json = JsonConvert.SerializeObject(setting, Helper.GetJsonSerializer());

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("PUT", _ServerBaseUrl + "Device", json);

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

        /// <summary>
        /// Deletes a Device Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task DeleteDeviceSettingAsync(string deviceId, string settingName)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);     

            var requestMessage = Helper.GetRequestMessage("DELETE", _ServerBaseUrl + "Device/" + deviceId + "/" + settingName,"");

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to delete device setting {1}", deviceId);
            }
        }

        /// <summary>
        /// Deletes a Device  to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task DeleteDeviceAsync(string deviceId)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("DELETE", _ServerBaseUrl + "Device/" + deviceId, "");

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to delete device setting {1}", deviceId);
            }
        }
    }
}
