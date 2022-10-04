using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeDi.Shared.DtoModel;
using Newtonsoft.Json;
using LeDi.Shared.Enum;

namespace LeDi.Shared
{
    public class Api
    {
        public string ServerBaseUrl { get; set; } = "https://localhost:7077/api/";

        public Api()
        {
            Console.WriteLine("Using Serverbase URL " + ServerBaseUrl);
        }
        public Api(string serverUrl)
        {
            ServerBaseUrl = serverUrl; 
            Console.WriteLine("Using Serverbase URL " + ServerBaseUrl);
        }


        #region Device

        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoDevice>?> GetDeviceAsync()
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Device");
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<List<DtoDevice>?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        public async Task<DtoDevice?> GetDeviceAsync(string deviceId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Device/" + deviceId);
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<DtoDevice?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Register a new device at the server. Returns a Device Objects containing the DeviceID
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<string?> RegisterDevice(string json)
        {
            Console.WriteLine("Registering Device...");

            var responseBody = await Helper.ApiRequestPost(ServerBaseUrl + "Device", json);

            return responseBody;
        }

        #endregion
        #region DeviceCommand

        /// <summary>
        /// Sends a command request to the database to tell a device to run a command
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task SetDeviceCommand(string deviceId, string command, string parameter)
        {
            Console.WriteLine("Sending Device Command {0} to device {1} with parameter {2}...", command, deviceId, parameter);
            var obj = new DtoDeviceCommand()
            {
                DeviceId = deviceId,
                Command = command,
                Parameter = parameter
            };
            var json = Helper.SerializeObject(obj);
            await Helper.ApiRequestPost(ServerBaseUrl + "DeviceCommand", json);

        }

        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoDeviceCommand>> GetDeviceCommandAsync(string deviceId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "DeviceCommand/" + deviceId);
            if (json == null)
                return new List<DtoDeviceCommand>();

            var command = JsonConvert.DeserializeObject<List<DtoDeviceCommand>?>(json, Helper.GetJsonSerializer());
            return command ?? new List<DtoDeviceCommand>();
        }

        /// <summary>
        /// Removes a command requested
        /// </summary>
        /// <returns></returns>
        public async Task RemoveDeviceCommand(DtoDeviceCommand command)
        {
            Console.WriteLine("Sending Remove DeviceCommand for command {0} to device {1}...", command.Id, command.DeviceId);
            var json = Helper.SerializeObject(command);
            await Helper.ApiRequestDelete(ServerBaseUrl + "DeviceCommand", json);
        }

        #endregion
        #region DeviceSetting

        /// <summary>
        /// Gets all Device Settings
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<List<DtoDeviceSetting>?> GetDeviceSettingsAsync(string deviceId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Device/" + deviceId);
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<List<DtoDeviceSetting>?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Gets a Device Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<DtoDeviceSetting?> GetDeviceSettingAsync(string deviceId, string settingName)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Device/" + deviceId + "/" + settingName);
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<DtoDeviceSetting?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Sets a Device Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task SetDeviceSettingAsync(string deviceId, DtoDeviceSetting setting)
        {
            var json = JsonConvert.SerializeObject(setting, Helper.GetJsonSerializer());

            var response = await Helper.ApiRequestPut(ServerBaseUrl + "Device", json);

            if (response == null)
            {
                Console.WriteLine("Failed to set device setting {0} for deviceId {1}", setting.Name, deviceId);
            }
        }

        /// <summary>
        /// Deletes a Device Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task DeleteDeviceSettingAsync(string deviceId, string settingName)
        {
            await Helper.ApiRequestDelete(ServerBaseUrl + "Device/" + deviceId + "/" + settingName);
        }

        #endregion
        #region Setting

        /// <summary>
        /// Deletes a Device  to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task DeleteDeviceAsync(string deviceId)
        {
            await Helper.ApiRequestDelete(ServerBaseUrl + "Device/" + deviceId);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoSetting>?> GetSettingAsync()
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Setting");
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<List<DtoSetting>?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Gets a Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public async Task<DtoSetting?> GetSettingAsync(string settingName)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Setting/" + settingName);
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<DtoSetting?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Sets a Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task SetSettingAsync(DtoSetting setting)
        {
            var json = JsonConvert.SerializeObject(setting, Helper.GetJsonSerializer());

            var response = await Helper.ApiRequestPut(ServerBaseUrl + "Setting", json);

            if (response == null)
            {
                Console.WriteLine("Failed to set setting {0}", setting.Name);
            }
        }

        #endregion
        #region Match

        /// <summary>
        /// Gets a Match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<DtoMatch?> GetMatchAsync(int matchId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Match/" + matchId);
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<DtoMatch?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Gets the time left of a match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<int> GetMatchTimeAsync(int matchId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Match/" + matchId + "/time");
            if (json == null)
                return -1;

            var setting = JsonConvert.DeserializeObject<int?>(json, Helper.GetJsonSerializer());
            return setting ?? -1;
        }

        /// <summary>
        /// Gets the time left of a match and a hash of all other values that might change
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<DtoMatchCore?> GetMatchCoreAsync(int matchId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Match/" + matchId + "/core");
            if (json == null)
                return null;

            var setting = JsonConvert.DeserializeObject<DtoMatchCore?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoMatch>> GetMatchListAsync()
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Match");
            if (json == null)
                return new List<DtoMatch>();

            var setting = JsonConvert.DeserializeObject<List<DtoMatch>>(json, Helper.GetJsonSerializer());
            return setting ?? new List<DtoMatch>();
        }


        /// <summary>
        /// Add a new Tournament
        /// </summary>
        /// <param name="match"></param>
        public async Task<DtoMatch?> AddMatchAsync(DtoMatch match)
        {
            var json = JsonConvert.SerializeObject(match, Helper.GetJsonSerializer());

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("POST", ServerBaseUrl + "Match", json);
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
                return null;
            }
            else //It is successful and contains the newMatch DTO Object
            {
                var newMatch = await response.Content.ReadAsStringAsync();
                if (newMatch == null)
                    return null;

                return JsonConvert.DeserializeObject<DtoMatch>(newMatch, Helper.GetJsonSerializer());
            }
        }

        /// <summary>
        /// Updates and existing Match
        /// </summary>
        /// <param name="match"></param>
        public async Task SetMatchAsync(DtoMatch match)
        {
            var json = JsonConvert.SerializeObject(match, Helper.GetJsonSerializer());
            var response = await Helper.ApiRequestPut(ServerBaseUrl + "Match/" + match.Id, json);

            if (response == null)
            {
                Console.WriteLine("Failed to set match {0}", match.Id);
            }
        }

        /// <summary>
        /// Send Control Commands of a Match
        /// </summary>
        public async Task ControlMatchtimeAsync(int matchId, string command)
        {
            var response = await Helper.ApiRequestPut(ServerBaseUrl + "Match/" + matchId + "/time/" + command, "");

            if (response == null)
            {
                Console.WriteLine("Failed to set match {0}", matchId);
            }
        }

        /// <summary>
        /// Sets match score for a team
        /// </summary>
        /// <param name="matchId">The match ID for the match</param>
        /// <param name="teamId">The Team ID for the team that gets the points (zero based)</param>
        /// <param name="amount">The amount of points the team gets</param>
        /// <returns></returns>
        public async Task SetMatchGoalAsync(int matchId, int teamId, int amount)
        {
            var response = await Helper.ApiRequestPut(ServerBaseUrl + "Match/" + matchId + "/goal/" + teamId + "/" + amount, "");

            if (response == null)
            {
                Console.WriteLine("Failed to set match goal for {0} and team {1} amount {2}", matchId, teamId, amount);
            }
        }

        /// <summary>
        /// Sets match score for a team
        /// </summary>
        /// <param name="matchId">The match ID for the match</param>
        /// <param name="newMatchStatus">The new Matchstatus ID</param>
        /// <returns></returns>
        public async Task SetMatchStatusAsync(int matchId, int newMatchStatus)
        {
            var response = await Helper.ApiRequestPut(ServerBaseUrl + "Match/" + matchId + "/status", newMatchStatus.ToString());

            if (response == null)
            {
                Console.WriteLine("Failed to set match status for {0} and status {1}", matchId, newMatchStatus);
            }
        }


        /// <summary>
        /// Sets match score for a team
        /// </summary>
        /// <param name="matchId">The match ID for the match</param>
        /// <param name="newMatchStatus">The new Matchstatus</param>
        /// <returns></returns>
        public async Task SetMatchStatusAsync(int matchId, MatchStatusEnum newMatchStatus)
        {
            await SetMatchStatusAsync(matchId, (int)newMatchStatus);
        }
#endregion
        }
}
