using LeDi.Shared2.DatabaseModel;
using LeDi.Server2.DatabaseModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog.Web.LayoutRenderers;
using Microsoft.AspNetCore.SignalR;
using LeDi.Server2.Display;

namespace LeDi.Server2
{
    public static class DataHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static IHubContext<DisplayHub> hubContext { get; set; }


        /// <summary>
        /// Get the match with the given ID.
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static async Task<TblMatch?> GetMatchAsync(int id)
        {
            using var dbContext = new LeDiDbContext();
            
            if (dbContext.TblMatches == null)
                return null;

            return await dbContext.TblMatches.Include("MatchEvents").Include("MatchPenalties").Include("MatchReferees").SingleOrDefaultAsync(x => x.Id == id);

        }


        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static List<TblMatch> GetMatchList()
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblMatches == null)
                return new List<TblMatch>();

            return dbContext.TblMatches.Include("MatchEvents").Include("MatchPenalties").Include("MatchReferees").ToList();
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
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblMatches == null)
                return int.MinValue;

            var match = await dbContext.TblMatches.SingleOrDefaultAsync(x => x.Id == matchId);
            if (match != null)
            {
                if (teamId == 0)
                {
                    match.Team1Score += scoreDiff;
                    await dbContext.SaveChangesAsync();
                    OnMatchUpdated?.Invoke(matchId);
                    OnMatchScoreChanged?.Invoke(matchId);
                    return match.Team1Score;
                }
                else if (teamId == 1)
                {
                    match.Team2Score += scoreDiff;
                    await dbContext.SaveChangesAsync();
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
        public static async Task<TblMatch?> AddMatch(TblMatch match)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblMatches == null)
                    return null;

                // fixing the problem, that tournament property is from a different context. This is dirty.
                // Maybe this will be a long term fix: https://stackoverflow.com/questions/52718652/ef-core-sqlite-sqlite-error-19-unique-constraint-failed
                if (match.Tournament != null)
                    match.Tournament = await dbContext.TblTournaments.SingleAsync(x => x.Id == match.Tournament.Id);

                dbContext.TblMatches.Add(match);
                await dbContext.SaveChangesAsync();
                OnMatchAdded?.Invoke(match.Id);
                return match;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Failed to add a new match.");
                return null;
            }
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
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblMatches == null)
                return null;

            if (await dbContext.TblMatches.AnyAsync(x => x.Id == match.Id))
            {
                var dbMatch = await dbContext.TblMatches.SingleAsync(x => x.Id == match.Id);
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

                await dbContext.SaveChangesAsync();
                OnMatchUpdated?.Invoke(dbMatch.Id);
                return dbMatch;
            }
            return null;
        }

        /// <summary>
        /// Updates the time left for a match
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="timeLeft"></param>
        /// <returns></returns>
        public static async Task SetMatchTimeAsync(int matchId, int timeLeft)
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblMatches == null)
                return;

            var match = await dbContext.TblMatches.SingleOrDefaultAsync(x => x.Id == matchId);

            if (match == null)
                return;

            match.CurrentTimeLeft = timeLeft;
            await dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static List<TblMatchEvent> GetMatchEvents(int matchId)
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblMatchEvents == null)
                return new List<TblMatchEvent>();

            return dbContext.TblMatchEvents.Where(x => x.MatchId == matchId).ToList();
        }


        /// <summary>
        /// Get the rules for the game name given in the parameter
        /// </summary>
        /// <param name="gamename"></param>
        /// <returns></returns>
        public static TblGameRule? GetGameRule(string gamename)
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblGameRules == null)
                return null;

            return dbContext.TblGameRules.SingleOrDefault(x => x.Sport == gamename);
        }



        /// <summary>
        /// Get all devices currently available.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblDevice>> GetDeviceListAsync(bool includeDisabled = false)
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDevice == null)
                return new List<TblDevice>();

            if (includeDisabled)
            {
                return await dbContext.TblDevice.ToListAsync();
            }
            else
            {
                return await dbContext.TblDevice.Where(x => x.Enabled == true).ToListAsync();
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
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDeviceSettings == null)
                return null;

            return await dbContext.TblDeviceSettings.SingleOrDefaultAsync(x => x.DeviceId == deviceId && x.SettingName == settingName);
        }

        /// <summary>
        /// Get all device settings
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static async Task<List<TblDeviceSetting>> GetDeviceSettingListAsync(string deviceId)
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDeviceSettings == null)
                return new List<TblDeviceSetting>();

            return await dbContext.TblDeviceSettings.Where(x => x.DeviceId == deviceId).ToListAsync();
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
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDeviceSettings == null)
                return null;

            var value = await dbContext.TblDeviceSettings.SingleOrDefaultAsync(x => x.DeviceId == deviceId && x.SettingName == settingName);
            if (value == null && createIfNotExist)
            {
                value = new TblDeviceSetting(deviceId, settingName, settingValue);
                await dbContext.TblDeviceSettings.AddAsync(value);
            }
            else if (value != null)
            {
                value.SettingValue = settingValue;
            }
            await dbContext.SaveChangesAsync();
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
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDeviceSettings == null)
                return false;

            var value = await dbContext.TblDeviceSettings.SingleOrDefaultAsync(x => x.DeviceId == deviceId && x.SettingName == settingName);
            if (value == null)
            {
                return false;
            }

            dbContext.TblDeviceSettings.Remove(value);
            await dbContext.SaveChangesAsync();
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
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDeviceCommands == null)
                return null;

            var value = new TblDeviceCommand(deviceId, command, parameter);
            await dbContext.TblDeviceCommands.AddAsync(value);
            await dbContext.SaveChangesAsync();
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
            using var dbContext = new LeDiDbContext();

            if (dbContext == null || dbContext.TblDeviceCommands == null)
                return new List<TblDeviceCommand>();

            return await dbContext.TblDeviceCommands.Where(x => x.DeviceId == deviceId).ToListAsync();
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

            using var dbContext = new LeDiDbContext();

            if (dbContext == null || dbContext.TblDeviceCommands == null)
                return;

            dbContext.TblDeviceCommands.Remove(command);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the device with device ID deviceId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task<TblDevice?> GetDeviceAsync(string deviceId)
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDevice == null)
                return null;

            var devices = dbContext.TblDevice.Where(x => x.DeviceId == deviceId);

            if (devices == null || devices.Count() == 0)
                return null;

            if (await devices.CountAsync() > 1)
                Logger.Warn("More than 1 device ({0}) found for device ID '{1}'. Returning only first device.", await devices.CountAsync(), deviceId);

            return devices.First();
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
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDevice == null)
                return null;

            if (await dbContext.TblDevice.AnyAsync(x => x.DeviceId == device.DeviceId))
            {
                var dev = await dbContext.TblDevice.SingleAsync(x => x.DeviceId == device.DeviceId);

                dev.DeviceName = device.DeviceName;
                dev.DeviceModel = device.DeviceModel;
                dev.DeviceType = device.DeviceType;
                dev.Enabled = device.Enabled;
                dev.Default = device.Default;
                await dbContext.SaveChangesAsync();
                OnDeviceUpdated?.Invoke(dev.DeviceId);
                return dev;
            }
            else
            {
                await dbContext.TblDevice.AddAsync(device);
                await dbContext.SaveChangesAsync();
                OnDeviceUpdated?.Invoke(device.DeviceId);
                return device;
            }
        }

        /// <summary>
        /// Removes a device.
        /// </summary>
        public async static Task<bool> RemoveDeviceAsync(string deviceId)
        {
            using var dbContext = new LeDiDbContext();

            if (dbContext.TblDevice == null)
                return false;

            if (await dbContext.TblDevice.AnyAsync(x => x.DeviceId == deviceId))
            {
                var dev = await dbContext.TblDevice.SingleAsync(x => x.DeviceId == deviceId);
                dbContext.Remove(dev);
                await dbContext.SaveChangesAsync();
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

                using var dbContext = new LeDiDbContext();

                // To satisfy the compiler
                if (dbContext == null || dbContext.TblDevice == null)
                    return;

                //Check if device already exists
                if (dbContext.TblDevice.Any(x => x.DeviceId == deviceId))
                {
                    Logger.Info("Device {0} is already registered.", deviceId);
                    return;
                }

                // Create new device entry
                var newDevice = new TblDevice(deviceId, deviceModel, deviceType, deviceName);
                newDevice.Enabled = true;

                // Add device to database
                await dbContext.TblDevice.AddAsync(newDevice);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ea)
            {
                Logger.Error(ea, "Cannot register because of an Error");
            }
            
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblSetting>> GetSettingListAsync()
        {
            using var dbContext = new LeDiDbContext();

            return await dbContext.TblSettings.ToListAsync();
        }

        /// <summary>
        /// Get setting
        /// </summary>
        /// <returns></returns>
        public static async Task<TblSetting?> GetSettingAsync(string settingName)
        {
            using var dbContext = new LeDiDbContext();

            return await dbContext.TblSettings.SingleOrDefaultAsync(x => x.SettingName == settingName);
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
            using var dbContext = new LeDiDbContext();

            var value = await dbContext.TblSettings.SingleOrDefaultAsync(x => x.SettingName == settingName);
            if (value == null && createIfNotExist)
            {
                value = new TblSetting(settingName, settingValue);
                await dbContext.TblSettings.AddAsync(value);
            }
            else if (value != null)
            {
                value.SettingValue = settingValue;
            }
            await dbContext.SaveChangesAsync();
            OnSettingUpdated?.Invoke();
            return value;
        }


        /// <summary>
        /// Gets all tournaments
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblTournament>> GetTournamentListAsync()
        {
            using var dbContext = new LeDiDbContext();

            return await dbContext.TblTournaments.Include("Matches").ToListAsync();
        }

        /// <summary>
        /// Gets a tournament
        /// </summary>
        /// <returns></returns>
        public static async Task<TblTournament?> GetTournamentAsync(int tournamentId)
        {
            using var dbContext = new LeDiDbContext();

            return await dbContext.TblTournaments.Include("Matches").SingleOrDefaultAsync(x => x.Id == tournamentId);
        }

        /// <summary>
        /// Add a new tournament to the database
        /// </summary>
        public static Action<int>? OnTournamentChanged { get; set; }

        /// <summary>
        /// Add a tournament to the database and inform relevant clients
        /// </summary>
        public static async Task<TblTournament?> AddTournamentAsync(TblTournament tournament)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                dbContext.TblTournaments?.Add(tournament);
                await dbContext.SaveChangesAsync();
                OnTournamentChanged?.Invoke(tournament.Id);
                return tournament;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to add a new tournament.");
            }
            return null;
        }

        /// <summary>
        /// Set/Edit a tournament
        /// </summary>
        /// <param name="tournament"></param>
        /// <returns></returns>
        public static async Task<TblTournament?> SetTournamentAsync (TblTournament tournament)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                var tour = await dbContext.TblTournaments.SingleOrDefaultAsync(x => x.Id == tournament.Id);
                if (tour == null)
                    return null;

                tour.DefaultPeriodCount = tournament.DefaultPeriodCount;
                tour.DefaultRuleMatchExtensionOnDraw = tournament.DefaultRuleMatchExtensionOnDraw;
                tour.DefaultRulePenaltyList = tournament.DefaultRulePenaltyList;
                tour.DefaultRulePeriodLastPauseTimeOnEvent = tournament.DefaultRulePeriodLastPauseTimeOnEvent;
                tour.DefaultRulePeriodLastPauseTimeOnEventSeconds = tournament.DefaultRulePeriodLastPauseTimeOnEventSeconds;
                tour.DefaultRulePeriodLength = tournament.DefaultRulePeriodLength;
                tour.DefaultRulePeriodOvertime = tournament.DefaultRulePeriodOvertime;
                tour.DefaultTeam1Name = tournament.DefaultTeam1Name;
                tour.DefaultTeam2Name = tournament.DefaultTeam2Name;
                tour.Devices = tournament.Devices;
                tour.EndDate = tournament.EndDate;
                tour.Matches = tournament.Matches;
                tour.Name = tournament.Name;
                tour.Sport = tournament.Sport;
                tour.StartDate = tournament.StartDate;

                await dbContext.SaveChangesAsync();
                OnTournamentChanged?.Invoke(tour.Id);

                return tour;
            }
            catch(Exception ea)
            {
                Logger.Error(ea, "Failed to edit tournament.");
            }
            return null;
        }

        /// <summary>
        /// Gets all user roles from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblUserRole>> GetUserRoleListAsync()
        {
            using var dbContext = new LeDiDbContext();

            return await dbContext.TblUserRoles.ToListAsync();
        }

        /// <summary>
        /// Adds or edits a new userrole to the database
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<TblUserRole?> SetUserRoleAsync(TblUserRole role)
        {
            var dbRole = await GetUserRoleAsync(role.Id);

            using var dbContext = new LeDiDbContext();

            if (dbRole == null)
            {
                await dbContext.TblUserRoles.AddAsync(role);
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
            await dbContext.SaveChangesAsync();
            return role;
        }

        /// <summary>
        /// Gets a userrole from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<TblUserRole?> GetUserRoleAsync(int roleId)
        {
            using var dbContext = new LeDiDbContext();

            return await dbContext.TblUserRoles.SingleOrDefaultAsync(x => x.Id == roleId);
        }


        /// <summary>
        /// Gets a userrole from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<TblUserRole?> GetUserRoleAsync(string roleName)
        {
            using var dbContext = new LeDiDbContext();

            return await dbContext.TblUserRoles.SingleOrDefaultAsync(x => x.RoleName == roleName);
        }
    }
}
