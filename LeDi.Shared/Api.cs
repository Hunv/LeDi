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
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string ServerBaseUrl { get; set; } = "https://localhost:7077/api/";

        public Api()
        {
            Logger.Info("Using Serverbase URL " + ServerBaseUrl);
        }
        public Api(string serverUrl)
        {
            ServerBaseUrl = serverUrl; 
            Logger.Info("Using Serverbase URL " + ServerBaseUrl);
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
            {
                Logger.Error("Failed to get devicelist.");
                return null;
            }

            Logger.Trace("Got devicelist: {0}", json);
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
            {
                Logger.Error("Failed to get device {0}", deviceId);
                return null;
            }

            Logger.Trace("Got device {0}: {1}", deviceId, json);
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
            Logger.Info("Registering Device using {0}...", json);

            var responseBody = await Helper.ApiRequestPost(ServerBaseUrl + "Device", json);

            Logger.Trace("Got register device response {0}", responseBody);

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
            Logger.Debug("Sending Device Command {0} to device {1} with parameter {2}...", command, deviceId, parameter);
            var obj = new DtoDeviceCommand()
            {
                DeviceId = deviceId,
                Command = command,
                Parameter = parameter
            };
            var json = Helper.SerializeObject(obj);
            Logger.Trace("Got set device command response for device {0} with command {1} and parameter {2}: {3}", deviceId, command, parameter, json);
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
            {
                Logger.Error("Failed to get device command {0}", deviceId);
                return new List<DtoDeviceCommand>();
            }

            Logger.Trace("Got device command(s) for device {0}: {1}", deviceId, json);
            var command = JsonConvert.DeserializeObject<List<DtoDeviceCommand>?>(json, Helper.GetJsonSerializer());
            return command ?? new List<DtoDeviceCommand>();
        }

        /// <summary>
        /// Removes a command requested
        /// </summary>
        /// <returns></returns>
        public async Task RemoveDeviceCommand(DtoDeviceCommand command)
        {
            Logger.Debug("Sending Remove DeviceCommand for command {0} to device {1}...", command.Id, command.DeviceId);
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
            {
                Logger.Error("Failed to get device settings for device {0}", deviceId);
                return null;
            }

            Logger.Trace("Got device settings for device {0}: {1}", deviceId, json);
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
            {
                Logger.Error("Failed to get device setting {0} for device {1}.", settingName, deviceId);
                return null;
            }

            Logger.Trace("Got device setting {0} for device {1}: {2}", settingName, deviceId, json);
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
                Logger.Error("Failed to set device setting {0} for deviceId {1}", setting.Name, deviceId);
            }
            else
            {
                Logger.Trace("Set device setting {0} for device {1}.", setting.Name, deviceId);
            }
        }

        /// <summary>
        /// Deletes a Device Setting to a new value
        /// </summary>
        /// <param name="setting"></param>
        public async Task DeleteDeviceSettingAsync(string deviceId, string settingName)
        {            
            await Helper.ApiRequestDelete(ServerBaseUrl + "Device/" + deviceId + "/" + settingName);
            Logger.Trace("Deleted device setting {0} for deviceId {1}.", settingName, deviceId);
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
            Logger.Trace("Deleted device {0}", deviceId);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoSetting>?> GetSettingAsync()
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Setting");
            if (json == null)
            {
                Logger.Error("Failed to get settings.");
                return null;
            }

            Logger.Trace("Got system settings: {0}", json);
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
            {
                Logger.Error("Failed to get setting {0}.", settingName);
                return null;
            }

            Logger.Trace("Got setting {0}: {1}", settingName, json);
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
                Logger.Error("Failed to set setting {0}", setting.Name);
            }
            else
            {
                Logger.Trace("Set setting {0}.", setting.Name);
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
            {
                Logger.Error("Failed to get match {0}.", matchId);
                return null;
            }

            Logger.Trace("Got match {0}: {1}", matchId, json);
            var setting = JsonConvert.DeserializeObject<DtoMatch?>(json, Helper.GetJsonSerializer());
            return setting;
        }

        /// <summary>
        /// Gets a Match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<DtoMatch?> GetMatchFullAsync(int matchId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Match/" + matchId + "/full");
            if (json == null)
            {
                Logger.Error("Failed to get full match details for match {0}.", matchId);
                return null;
            }

            Logger.Trace("Got full match {0}: {1}", matchId, json);
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
            {
                Logger.Error("Failed to get match time for match {0}.", matchId);
                return -1;
            }

            Logger.Trace("Got match time for {0}: {1}", matchId, json);
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
            {
                Logger.Error("Failed to get match core for {0}.", matchId);
                return null;
            }

            Logger.Trace("Got match core for {0}: {1}", matchId, json);
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
            {
                Logger.Error("Failed to get match list.");
                return new List<DtoMatch>();
            }

            Logger.Trace("Got match list: {0}", json);
            var setting = JsonConvert.DeserializeObject<List<DtoMatch>>(json, Helper.GetJsonSerializer());
            return setting ?? new List<DtoMatch>();
        }


        /// <summary>
        /// Add a new Tournament
        /// </summary>
        /// <param name="match"></param>
        public async Task<DtoMatch?> NewMatchAsync(DtoMatch match)
        {
            var json = await Helper.ApiRequestPost(ServerBaseUrl + "Match", JsonConvert.SerializeObject(match, Helper.GetJsonSerializer()));

            if (json == null)
            {
                Logger.Error("Failed to create new match {0}.", match.Team1Name + " vs. " + match.Team2Name);
                return new DtoMatch();
            }

            Logger.Trace("Got new match: {0}", json);
            var newMatch = JsonConvert.DeserializeObject<DtoMatch>(json, Helper.GetJsonSerializer());
            return newMatch ?? new DtoMatch();
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
                Logger.Error("Failed to set match {0}", match.Id);
            }
            else
            {
                Logger.Trace("Set match for match {0}.", match.Id);
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
                Logger.Error("Failed to set match {0}", matchId);
            }
            else
            {
                Logger.Trace("Sent control command for match {0}: {1}", matchId, command);
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
                Logger.Error("Failed to set match goal for {0} and team {1} amount {2}", matchId, teamId, amount);
            }
            else
            {
                Logger.Trace("Sent match amout for match {0}, team {1}: {2}", matchId, teamId, amount);
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
                Logger.Error("Failed to set match status for {0} and status {1}", matchId, newMatchStatus);
            }
            else
            {
                Logger.Trace("Sent new match status for {0}: {1}", matchId, newMatchStatus);
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

        /// <summary>
        /// Gets all Match Events for a match
        /// </summary>
        /// <returns>List of match events</returns>
        public async Task<List<DtoMatchEvent>> GetMatchEvents(int matchId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Match/" + matchId + "/events");
            if (json == null)
            {
                Logger.Error("Failed to get match events for {0}", matchId);
                return new List<DtoMatchEvent>();
            }

            Logger.Trace("Got match events for {0}: {1}", matchId, json);
            var setting = JsonConvert.DeserializeObject<List<DtoMatchEvent>>(json, Helper.GetJsonSerializer());
            return setting ?? new List<DtoMatchEvent>();
        }

        /// <summary>
        /// Sets the next match halftime
        /// </summary>
        /// <param name="matchId">The match ID for the match</param>
        /// <returns></returns>
        public async Task SetMatchHalftimeNext(int matchId)
        {
            var response = await Helper.ApiRequestPut(ServerBaseUrl + "Match/" + matchId + "/next", "");

            if (response == null)
            {
                Logger.Error("Failed to set next halftime for matchId {0}", matchId);
            }
            else
            {
                Logger.Trace("Set next halftime for match {0}", matchId);
            }
        }

        /// <summary>
        /// Gets all Match Penalties for a match
        /// </summary>
        /// <returns>List of match events</returns>
        public async Task<List<DtoMatchPenalty>> GetMatchPenalties(int matchId)
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Match/" + matchId + "/penalty");
            if (json == null)
            {
                Logger.Error("Failed to get match penalties for match {0}", matchId);
                return new List<DtoMatchPenalty>();
            }

            Logger.Trace("Got match penalties for match {0}: {1}", matchId, json);
            var setting = JsonConvert.DeserializeObject<List<DtoMatchPenalty>>(json, Helper.GetJsonSerializer());
            return setting ?? new List<DtoMatchPenalty>();
        }

        /// <summary>
        /// Adds a new Match Penalty for a match
        /// </summary>
        /// <returns>The new match penalty</returns>
        public async Task<DtoMatchPenalty> AddMatchPenalty(int matchId, DtoMatchPenalty penalty)
        {
            var jsonIn = JsonConvert.SerializeObject(penalty, Helper.GetJsonSerializer());

            var json = await Helper.ApiRequestPost(ServerBaseUrl + "Match/" + matchId + "/penalty", jsonIn);
            if (json == null)
            {
                Logger.Error("Failed to add match penalty using {0} for {1}.", jsonIn, matchId);
                return new DtoMatchPenalty();
            }

            Logger.Trace("Got match penalty for match {0}: {1}", matchId, json);
            var setting = JsonConvert.DeserializeObject<DtoMatchPenalty>(json, Helper.GetJsonSerializer());
            return setting ?? new DtoMatchPenalty();
        }

        /// <summary>
        /// Revokes a penalty
        /// </summary>
        /// <returns>the revoked match penalty</returns>
        public async Task<DtoMatchPenalty> RevokeMatchPenalty(int matchId, int penaltyId, string revokeNote)
        {            
            var json = await Helper.ApiRequestPut(ServerBaseUrl + "Match/" + matchId + "/penalty/" + penaltyId, System.Text.Json.JsonEncodedText.Encode(revokeNote).ToString());
            if (json == null)
            {
                Logger.Error("Failed to revoke match penalty {0} for match {1} with note {2}.",penaltyId, matchId, revokeNote);
                return new DtoMatchPenalty();
            }

            Logger.Trace("Revoked match penalty {0} for match {1}: {2}", penaltyId, matchId, json);
            var setting = JsonConvert.DeserializeObject<DtoMatchPenalty>(json, Helper.GetJsonSerializer());
            return setting ?? new DtoMatchPenalty();
        }

        #endregion

        #region Rules
        /// <summary>
        /// Gets all Rules
        /// </summary>
        /// <returns></returns>
        public async Task<DtoRuleBody?> GetRulesAsync()
        {
            var json = await Helper.ApiRequestGet(ServerBaseUrl + "Rule/rules");
            if (json == null)
            {
                Logger.Error("Failed to get rule list.");
                return null;
            }

            Logger.Trace("Got rules: {0}", json);
            var rules = JsonConvert.DeserializeObject<DtoRuleBody?>(json, Helper.GetJsonSerializer());
            return rules;
        }
        #endregion
    }
}
