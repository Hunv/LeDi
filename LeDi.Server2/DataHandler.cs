using LeDi.Server2.DatabaseModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog.Web.LayoutRenderers;

namespace LeDi.Server2
{
    public static class DataHandler
    {
        private static LeDiDbContext? DbContext;
        private static System.Timers.Timer TmrDbSaveTime = new System.Timers.Timer(100);
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        
        static DataHandler()
        {
            InitializeDataHandler();
        }

        public static void InitializeDataHandler()
        {
            DbContext = new LeDiDbContext();
            TmrDbSaveTime.Elapsed += TmrDbSaveTime_Elapsed;
            TmrDbSaveTime.Start();
        }

        private static async void TmrDbSaveTime_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Skipped DataHandler save...");
            }
        }


        /// <summary>
        /// Get the match with the given ID.
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static async Task<TblMatch?> GetMatchAsync(int id)
        {
            return await DbContext.TblMatches?.SingleOrDefaultAsync(x => x.Id == id);
        }


        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static List<TblMatch> GetMatchList()
        {
            return DbContext.TblMatches.ToList();
        }

        /// <summary>
        /// A match score changed
        /// </summary>
        public static Action<int>? OnMatchScoreChanged { get; set; }

        /// <summary>
        /// Adds or removes score for one of the teams
        /// </summary>
        /// <param name="teamId">Team 1 (ID: 0) or Team2 (ID: 1)?</param>
        /// <param name="scoreDiff">The score difference to the current score</param>
        /// <returns></returns>
        public static async Task<int> UpdateMatchScoreAsync(int matchId, int teamId, int scoreDiff)
        {
            var match = await DbContext.TblMatches?.SingleOrDefaultAsync(x => x.Id == matchId);
            if (match != null)
            {
                if (teamId == 0)
                {
                    match.Team1Score += scoreDiff;
                    OnMatchUpdated?.Invoke(matchId);
                    OnMatchScoreChanged?.Invoke(matchId);
                    return match.Team1Score;
                }
                else if (teamId == 1)
                {
                    match.Team2Score += scoreDiff;
                    OnMatchUpdated?.Invoke(matchId);
                    OnMatchScoreChanged?.Invoke(matchId);
                    return match.Team2Score;
                }
                return int.MinValue;
            }
            else
            {
                return int.MinValue;
            }

        }

        /// <summary>
        /// Add a new match to the database
        /// </summary>
        public static Action<int>? OnMatchAdded { get; set; }

        /// <summary>
        /// Add a match to the database and inform relevant clients
        /// </summary>
        public static TblMatch? AddMatch(TblMatch match)
        {
            DbContext.TblMatches?.Add(match);
            DbContext.SaveChangesAsync();
            OnMatchAdded?.Invoke(match.Id);
            return match;
        }

        /// <summary>
        /// A match was updated in the database
        /// </summary>
        public static Action<int>? OnMatchUpdated { get; set; }

        /// <summary>
        /// Edit a match. Attention: EVERYTHING will be set as given!
        /// </summary>
        public async static Task<TblMatch?> SetMatchAsync(TblMatch match)
        {
            if (await DbContext.TblMatches?.AnyAsync(x => x.Id == match.Id))
            {
                var dbMatch = await DbContext.TblMatches.SingleAsync(x => x.Id == match.Id);
                dbMatch.CurrentPeriod = match.CurrentPeriod;
                dbMatch.CurrentTimeLeft = match.CurrentTimeLeft;
                dbMatch.Devices = match.Devices;
                dbMatch.GameName = match.GameName;
                dbMatch.MatchStatus = match.MatchStatus;
                dbMatch.RuleMatchExtensionOnDraw = match.RuleMatchExtensionOnDraw;
                dbMatch.RulePenaltyList = match.RulePenaltyList;
                dbMatch.RulePeriodCount = match.RulePeriodCount;
                dbMatch.RulePeriodLastPauseTimeOnEvent = match.RulePeriodLastPauseTimeOnEvent;
                dbMatch.RulePeriodLastPauseTimeOnEventSeconds = match.RulePeriodLastPauseTimeOnEventSeconds;
                dbMatch.RulePeriodLength = match.RulePeriodLength;
                dbMatch.RulePeriodOvertime = match.RulePeriodOvertime;
                dbMatch.ScheduledTime = match.ScheduledTime;
                dbMatch.Team1Name = match.Team1Name;                
                dbMatch.Team1Score = match.Team1Score;
                dbMatch.Team2Name = match.Team2Name;                
                dbMatch.Team2Score = match.Team2Score;

                dbMatch.MatchEvents = match.MatchEvents;
                dbMatch.MatchPenalties = match.MatchPenalties;
                dbMatch.MatchReferees = match.MatchReferees;
                dbMatch.Team1Players = match.Team1Players;
                dbMatch.Team2Players = match.Team2Players;

                await DbContext.SaveChangesAsync();
                OnMatchUpdated?.Invoke(dbMatch.Id);
                return dbMatch;
            }
            return null;
        }




        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static List<TblMatchEvent> GetMatchEvents(int matchId)
        {
            return DbContext.TblMatchEvents.Where(x => x.MatchId == matchId).ToList();
        }


        /// <summary>
        /// Get the rules for the game name given in the parameter
        /// </summary>
        /// <param name="gamename"></param>
        /// <returns></returns>
        public static TblGameRule? GetGameRule(string gamename)
        {
            return DbContext.TblGameRules?.SingleOrDefault(x => x.Sport == gamename);
        }



        /// <summary>
        /// Get all devices currently available.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblDevice>> GetDeviceListAsync(bool includeDisabled = false)
        {
            if (DbContext.TblDevice == null)
                return new List<TblDevice>();

            if (includeDisabled)
            {
                return await DbContext.TblDevice?.ToListAsync();
            }
            else
            {
                return await DbContext.TblDevice?.Where(x => x.Enabled == true).ToListAsync();
            }
        }


        /// <summary>
        /// Get a device setting
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static async Task<TblDeviceSetting?> GetDeviceSettingAsync(string deviceId, string settingName)
        {
            return await DbContext.TblDeviceSettings?.SingleOrDefaultAsync(x => x.DeviceId == deviceId && x.SettingName == settingName);
        }

        /// <summary>
        /// Get all device settings
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static async Task<List<TblDeviceSetting>> GetDeviceSettingListAsync(string deviceId)
        {
            return await DbContext.TblDeviceSettings?.Where(x => x.DeviceId == deviceId).ToListAsync();
        }



        /// <summary>
        /// A device setting was updated or added
        /// </summary>
        public static Action<string>? OnDeviceSettingUpdated { get; set; }

        /// <summary>
        /// Updates a device setting.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        /// <returns></returns>
        public static async Task<TblDeviceSetting?> SetDeviceSettingAsync(string deviceId, string settingName, string settingValue, bool createIfNotExist = true)
        {
            var value = await DbContext.TblDeviceSettings.SingleOrDefaultAsync(x => x.DeviceId == deviceId && x.SettingName == settingName);
            if (value == null && createIfNotExist)
            {
                value = new TblDeviceSetting(deviceId, settingName, settingValue);
                await DbContext.TblDeviceSettings.AddAsync(value);
            }
            else if (value != null)
            {
                value.SettingValue = settingValue;
            }
            await DbContext.SaveChangesAsync();
            OnDeviceSettingUpdated?.Invoke(deviceId);
            return value;
        }

        /// <summary>
        /// Removes a device setting.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static async Task<bool> RemoveDeviceSettingAsync(string deviceId, string settingName)
        {
            var value = await DbContext.TblDeviceSettings.SingleOrDefaultAsync(x => x.DeviceId == deviceId && x.SettingName == settingName);
            if (value == null)
            {
                return false;
            }

            DbContext.TblDeviceSettings.Remove(value);
            await DbContext.SaveChangesAsync();
            OnDeviceSettingUpdated?.Invoke(deviceId);
            return true;
        }



        /// <summary>
        /// A device command was added
        /// </summary>
        public static Action<string>? OnDeviceCommandAdded { get; set; }

        /// <summary>
        /// Adds a new Device Command for a device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static async Task<TblDeviceCommand?> AddDeviceCommandAsync(string deviceId, string command, string parameter = "")
        {
            var value = new TblDeviceCommand(deviceId, command, parameter);
            await DbContext.TblDeviceCommands.AddAsync(value);
            await DbContext.SaveChangesAsync();
            OnDeviceCommandAdded?.Invoke(deviceId);
            return value; 
        }


        /// <summary>
        /// Gets device commands for a device Id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task<List<TblDeviceCommand>> GetDeviceCommandsAsync(string deviceId)
        {
            if (DbContext == null || DbContext.TblDeviceCommands == null)
                return new List<TblDeviceCommand>();

            return await DbContext.TblDeviceCommands.Where(x => x.DeviceId == deviceId).ToListAsync();
        }

        /// <summary>
        /// A device command was removed
        /// </summary>
        public static Action<string>? OnDeviceCommandRemoved { get; set; }

        /// <summary>
        /// Removes a device command
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task RemoveDeviceCommandAsync(TblDeviceCommand command)
        {
            if (DbContext == null || DbContext.TblDeviceCommands == null)
                return;

            DbContext.TblDeviceCommands.Remove(command);
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the device with device ID deviceId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task<TblDevice> GetDeviceAsync(string deviceId)
        {
            return await DbContext.TblDevice.SingleOrDefaultAsync(x => x.DeviceId == deviceId);
        }

        /// <summary>
        /// A device was updated or added
        /// </summary>
        public static Action<string>? OnDeviceUpdated { get; set; }

        /// <summary>
        /// Edit a device. Attention: EVERYTHING will be set as given!
        /// </summary>
        public async static Task<TblDevice?> SetDeviceAsync(TblDevice device)
        {
            if (await DbContext.TblDevice?.AnyAsync(x => x.DeviceId == device.DeviceId))
            {
                var dev = await DbContext.TblDevice.SingleAsync(x => x.DeviceId == device.DeviceId);

                dev.DeviceName = device.DeviceName;
                dev.DeviceModel = device.DeviceModel;
                dev.DeviceType = device.DeviceType;
                dev.Enabled = device.Enabled;
                dev.Default = device.Default;
                await DbContext.SaveChangesAsync();
                OnDeviceUpdated?.Invoke(dev.DeviceId);
                return dev;
            }
            else
            {
                await DbContext.TblDevice.AddAsync(device);
                await DbContext.SaveChangesAsync();
                OnDeviceUpdated?.Invoke(device.DeviceId);
                return device;
            }
        }

        /// <summary>
        /// Removes a device.
        /// </summary>
        public async static Task<bool> RemoveDeviceAsync(string deviceId)
        {
            if (DbContext.TblDevice == null)
                return false;

            if (await DbContext.TblDevice?.AnyAsync(x => x.DeviceId == deviceId))
            {
                var dev = await DbContext.TblDevice.SingleAsync(x => x.DeviceId == deviceId);
                DbContext.Remove(dev);
                await DbContext.SaveChangesAsync();
                OnDeviceUpdated?.Invoke(dev.DeviceId);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Registers a new device at the server
        /// </summary>
        /// <returns></returns>
        public static async Task RegisterDevice(string deviceId, string deviceName, string deviceType, string deviceModel)
        {
            try
            {
                Logger.Trace("RegisterDevice executed.");

                // To satisfy the compiler
                if (DbContext == null || DbContext.TblDevice == null)
                    return;

                //Check if device already exists
                if (DbContext.TblDevice.Any(x => x.DeviceId == deviceId))
                {
                    Logger.Info("Device {0} is already registered.");
                    return;
                }

                // Create new device entry
                var newDevice = new TblDevice(deviceId, deviceModel, deviceType, deviceName);
                newDevice.Enabled = true;

                // Add device to database
                await DbContext.TblDevice.AddAsync(newDevice);
            }
            catch (Exception ea)
            {
                Logger.Error("Cannot register because of an Error: ", ea.ToString());
            }
            
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblSetting>> GetSettingListAsync()
        {
            return await DbContext.TblSettings.ToListAsync();
        }

        /// <summary>
        /// Get setting
        /// </summary>
        /// <returns></returns>
        public static async Task<TblSetting?> GetSettingAsync(string settingName)
        {
            return await DbContext.TblSettings.SingleOrDefaultAsync(x => x.SettingName == settingName);
        }


        /// <summary>
        /// A setting was updated or added
        /// </summary>
        public static Action? OnSettingUpdated { get; set; }

        /// <summary>
        /// Updates a setting.
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        /// <returns></returns>
        public static async Task<TblSetting?> SetSettingAsync(string settingName, string settingValue, bool createIfNotExist = true)
        {
            var value = await DbContext.TblSettings.SingleOrDefaultAsync(x => x.SettingName == settingName);
            if (value == null && createIfNotExist)
            {
                value = new TblSetting(settingName, settingValue);
                await DbContext.TblSettings.AddAsync(value);
            }
            else if (value != null)
            {
                value.SettingValue = settingValue;
            }
            await DbContext.SaveChangesAsync();
            OnSettingUpdated?.Invoke();
            return value;
        }


        /// <summary>
        /// Gets all tournaments
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblTournament>> GetTournamentListAsync()
        {
            return await DbContext.TblTournaments.Include("Matches").ToListAsync();
        }

        /// <summary>
        /// Gets a tournament
        /// </summary>
        /// <returns></returns>
        public static async Task<TblTournament?> GetTournamentAsync(int tournamentId)
        {
            return await DbContext.TblTournaments.Include("Matches").SingleOrDefaultAsync(x => x.Id == tournamentId);
        }

        /// <summary>
        /// Add a new tournament to the database
        /// </summary>
        public static Action<int>? OnTournamentAdded { get; set; }

        /// <summary>
        /// Add a tournament to the database and inform relevant clients
        /// </summary>
        public static async Task<TblTournament?> AddTournamentAsync(TblTournament tournament)
        {
            try
            {
                DbContext.TblTournaments?.Add(tournament);
                await DbContext.SaveChangesAsync();
                OnTournamentAdded?.Invoke(tournament.Id);
                return tournament;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to add a new tournament.");
            }
            return null;
        }

        /// <summary>
        /// Saves current changes in case they are edited outside of the DataHandler
        /// </summary>
        public static async Task SaveChangesAsync()
        {
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to save changes.");
            }
        }


        /// <summary>
        /// Gets all user roles from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblUserRole>> GetUserRoleListAsync()
        {
            return await DbContext.TblUserRoles.ToListAsync();
        }

        /// <summary>
        /// Adds or edits a new userrole to the database
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<TblUserRole?> SetUserRoleAsync(TblUserRole role)
        {
            var dbRole = await GetUserRoleAsync(role.Id);

            if (dbRole == null)
            {
                await DbContext.TblUserRoles.AddAsync(role);
            }
            else
            {
                dbRole.CanDeviceCommands = role.CanDeviceCommands;
                dbRole.CanDeviceManage = role.CanDeviceManage;
                dbRole.CanMatchAdd = role.CanMatchAdd;
                dbRole.CanMatchAdvancedControls = role.CanMatchAdvancedControls;
                dbRole.CanMatchDelete = role.CanMatchDelete;
                dbRole.CanMatchEdit = role.CanMatchEdit;
                dbRole.CanMatchEnd = role.CanMatchEnd;
                dbRole.CanMatchPenalty = role.CanMatchPenalty;
                dbRole.CanMatchStart = role.CanMatchStart;
                dbRole.CanMatchStop = role.CanMatchStop;
                dbRole.CanPlayerAdd = role.CanPlayerAdd;
                dbRole.CanPlayerDelete = role.CanPlayerDelete;
                dbRole.CanPlayerEdit = role.CanPlayerEdit;
                dbRole.CanSettingManage = role.CanSettingManage;
                dbRole.CanTeamAdd = role.CanTeamAdd;
                dbRole.CanTeamDelete = role.CanTeamDelete;
                dbRole.CanTeamEdit = role.CanTeamEdit;
                dbRole.CanTemplateManage = role.CanTemplateManage;
                dbRole.CanTournamentEdit = role.CanTournamentEdit;
                dbRole.CanTournamentMatchAdd = role.CanTournamentMatchAdd;
                dbRole.CanTournamentAdd = role.CanTournamentAdd;
                dbRole.CanTournamentMatchDelete = role.CanTournamentMatchDelete;
                dbRole.CanTournamentMatchEdit = role.CanTournamentMatchEdit;
                dbRole.CanUserAdd = role.CanUserAdd;
                dbRole.CanUserDelete = role.CanUserDelete;
                dbRole.CanUserEdit = role.CanUserEdit;
                dbRole.CanUserPasswordEdit = role.CanUserPasswordEdit;
            }
            await DbContext.SaveChangesAsync();
            return role;
        }

        /// <summary>
        /// Gets a userrole from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<TblUserRole?> GetUserRoleAsync(int roleId)
        {
            return await DbContext.TblUserRoles.SingleOrDefaultAsync(x => x.Id == roleId);
        }


        /// <summary>
        /// Gets a userrole from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<TblUserRole?> GetUserRoleAsync(string roleName)
        {
            return await DbContext.TblUserRoles.SingleOrDefaultAsync(x => x.RoleName == roleName);
        }
    }
}
