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
using LeDi.Server2.Data;
using System.Security.Cryptography.X509Certificates;
using LeDi.Server2.Pages;

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
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblMatches == null)
                    return null;

                return await dbContext.TblMatches
                    .Include("MatchEvents")
                    .Include("MatchPenalties")
                    .Include("MatchReferees")
                    .Include("Tournament")
                    .Include("Tournament.Template")
                    .Include("Tournament.Template.PenaltyList")
                    .Include("Tournament.Template.PenaltyList.Display")
                    .SingleOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static List<TblMatch> GetMatchList()
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblMatches == null)
                    return new List<TblMatch>();

                return dbContext.TblMatches.Include("MatchEvents").Include("MatchPenalties").Include("MatchReferees").ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblMatch>();
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
            try
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

                        // Send the new data to the connected displays
                        await SendMatch(matchId, match);

                        return match.Team1Score;
                    }
                    else if (teamId == 1)
                    {
                        match.Team2Score += scoreDiff;
                        await dbContext.SaveChangesAsync();
                        OnMatchUpdated?.Invoke(matchId);
                        OnMatchScoreChanged?.Invoke(matchId);

                        // Send the new data to the connected displays
                        await SendMatch(matchId, match);

                        return match.Team2Score;
                    }
                    return int.MinValue;
                }
                else
                {
                    return int.MinValue;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return int.MinValue;
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
                {
                    var dbTournament = await dbContext.TblTournaments.Include("Template").SingleAsync(x => x.Id == match.Tournament.Id);
                    match.Tournament = dbTournament;
                }

                dbContext.TblMatches.Add(match);
                await dbContext.SaveChangesAsync();
                OnMatchAdded?.Invoke(match.Id);
                return match;
            }
            catch (Exception ex)
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
            try
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
                    if (match.Tournament != null)
                        dbMatch.Tournament = await dbContext.TblTournaments.SingleOrDefaultAsync(x => x.Id == match.Tournament.Id);

                    dbMatch.MatchEvents = match.MatchEvents;
                    dbMatch.MatchPenalties = match.MatchPenalties;
                    dbMatch.MatchReferees = match.MatchReferees;
                    dbMatch.Team1Players = match.Team1Players;
                    dbMatch.Team2Players = match.Team2Players;

                    await dbContext.SaveChangesAsync();
                    OnMatchUpdated?.Invoke(dbMatch.Id);                    

                    // Send the new data to the connected displays
                    await SendMatch(match.Id, match);

                    return dbMatch;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblMatches == null)
                    return;

                var match = await dbContext.TblMatches.SingleOrDefaultAsync(x => x.Id == matchId);

                if (match == null)
                    return;

                match.CurrentTimeLeft = timeLeft;
                await dbContext.SaveChangesAsync();

                // Send the new data to the connected displays
                await SendMatch(matchId, match);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The match. Null if match Id not exists.</returns>
        public static List<TblMatchEvent> GetMatchEvents(int matchId)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblMatchEvents == null)
                    return new List<TblMatchEvent>();

                return dbContext.TblMatchEvents.Where(x => x.MatchId == matchId).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblMatchEvent>();
        }


        ///// <summary>
        ///// Get the rules for the game name given in the parameter
        ///// </summary>
        ///// <param name="gamename"></param>
        ///// <returns></returns>
        //public static TblGameRule? GetGameRule(string gamename)
        //{
        //    using var dbContext = new LeDiDbContext();

        //    if (dbContext.TblGameRules == null)
        //        return null;

        //    return dbContext.TblGameRules.SingleOrDefault(x => x.Sport == gamename);
        //}



        /// <summary>
        /// Get all devices currently available.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblDevice>> GetDeviceListAsync(bool includeDisabled = false)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblDevice>();
        }


        /// <summary>
        /// Get a device setting
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static async Task<TblDeviceSetting?> GetDeviceSettingAsync(string deviceId, string settingName)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblDeviceSettings == null)
                    return null;

                return await dbContext.TblDeviceSettings.SingleOrDefaultAsync(x => x.DeviceId == deviceId && x.SettingName == settingName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Get all device settings
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static async Task<List<TblDeviceSetting>> GetDeviceSettingListAsync(string deviceId)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblDeviceSettings == null)
                    return new List<TblDeviceSetting>();

                return await dbContext.TblDeviceSettings.Where(x => x.DeviceId == deviceId).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblDeviceSetting>();
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
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Removes a device setting.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static async Task<bool> RemoveDeviceSettingAsync(string deviceId, string settingName)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
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
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        /// <summary>
        /// Gets device commands for a device Id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task<List<TblDeviceCommand>> GetDeviceCommandsAsync(string deviceId)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext == null || dbContext.TblDeviceCommands == null)
                    return new List<TblDeviceCommand>();

                return await dbContext.TblDeviceCommands.Where(x => x.DeviceId == deviceId).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblDeviceCommand>();
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

            try
            {
                using var dbContext = new LeDiDbContext();

                if (dbContext == null || dbContext.TblDeviceCommands == null)
                    return;

                dbContext.TblDeviceCommands.Remove(command);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Gets the device with device ID deviceId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task<TblDevice?> GetDeviceAsync(string deviceId)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
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
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Removes a device.
        /// </summary>
        public async static Task<bool> RemoveDeviceAsync(string deviceId)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
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
            try
            {
                using var dbContext = new LeDiDbContext();

                return await dbContext.TblSettings.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblSetting>();
        }

        /// <summary>
        /// Get setting
        /// </summary>
        /// <returns></returns>
        public static async Task<TblSetting?> GetSettingAsync(string settingName)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                return await dbContext.TblSettings.SingleOrDefaultAsync(x => x.SettingName == settingName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
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
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        /// <summary>
        /// Gets all tournaments
        /// </summary>
        /// <returns></returns>
        public static async Task<List<TblTournament>> GetTournamentListAsync()
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                return await dbContext.TblTournaments.Include("Matches").Include("Template").ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblTournament>();
        }

        /// <summary>
        /// Gets a tournament
        /// </summary>
        /// <returns></returns>
        public static async Task<TblTournament?> GetTournamentAsync(int tournamentId)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                return await dbContext.TblTournaments
                    .Include("Matches")
                    .Include("Template")
                    .Include("Template.PenaltyList")
                    .SingleOrDefaultAsync(x => x.Id == tournamentId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
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

                //Get the "live"-Database object of the template database entry instead of the previously cached template, which was downloaded in another session
                if (tournament.Template != null)
                    tournament.Template = await dbContext.TblTemplates
                        .SingleOrDefaultAsync(x => x.Id == tournament.Template.Id);

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
        public static async Task<TblTournament?> SetTournamentAsync(TblTournament tournament)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                var tour = await dbContext.TblTournaments.SingleOrDefaultAsync(x => x.Id == tournament.Id);
                if (tour == null)
                    return null;

                tour.DefaultTeam1Name = tournament.DefaultTeam1Name;
                tour.DefaultTeam2Name = tournament.DefaultTeam2Name;
                tour.Devices = tournament.Devices;
                tour.EndDate = tournament.EndDate;
                tour.Matches = tournament.Matches;
                tour.Name = tournament.Name;
                tour.Sport = tournament.Sport;
                tour.StartDate = tournament.StartDate;
                tour.Template = tournament.Template == null ? null : await GetTemplate(tournament.Template.Id);

                await dbContext.SaveChangesAsync();
                OnTournamentChanged?.Invoke(tour.Id);

                return tour;
            }
            catch (Exception ea)
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
            try
            {
                using var dbContext = new LeDiDbContext();

                return await dbContext.TblUserRoles.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblUserRole>();
        }

        /// <summary>
        /// Adds or edits a new userrole to the database
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task<TblUserRole?> SetUserRoleAsync(TblUserRole role)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets a userrole from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<TblUserRole?> GetUserRoleAsync(int roleId)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                return await dbContext.TblUserRoles.SingleOrDefaultAsync(x => x.Id == roleId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        /// <summary>
        /// Gets a userrole from the database
        /// </summary>
        /// <returns></returns>
        public static async Task<TblUserRole?> GetUserRoleAsync(string roleName)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                return await dbContext.TblUserRoles.SingleOrDefaultAsync(x => x.RoleName == roleName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }


        /// <summary>
        /// Gets a template by ID
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task<TblTemplate?> GetTemplate(int templateId)
        {
            try
            {
                using var dbContext = new LeDiDbContext();
                return (await dbContext.TblTemplates
                    .Include("PenaltyList")
                    .Include("PenaltyList.Display")
                    .SingleOrDefaultAsync(x => x.Id == templateId)
                    );
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Returns all templates
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task<List<TblTemplate>> GetTemplateList()
        {
            try
            {
                using var dbContext = new LeDiDbContext();
                return (await dbContext.TblTemplates.Include("PenaltyList").ToListAsync());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return new List<TblTemplate>();
        }

        /// <summary>
        /// Adds a template to database
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task AddTemplate(TblTemplate template)
        {
            try
            {
                using var dbContext = new LeDiDbContext();
                dbContext.TblTemplates.Add(template);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        /// <summary>
        /// Adds a template penalty to database
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task AddTemplatePenalty(int templateId, TblTemplatePenaltyItem penalty)
        {
            try
            {
                using var dbContext = new LeDiDbContext();
                if (dbContext.TblTemplates.Single(x => x.Id == templateId).PenaltyList == null)
                {
                    dbContext.TblTemplates.Single(x => x.Id == templateId).PenaltyList = new List<TblTemplatePenaltyItem>();
                }

                dbContext.TblTemplates.Single(x => x.Id == templateId).PenaltyList.Add(penalty);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Updates a template penalty to database
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task SetTemplatePenalty(int templateId, TblTemplatePenaltyItem penalty)
        {
            try
            {
                using var dbContext = new LeDiDbContext();

                var templatePenalty = dbContext.TblTemplates
                    .Include("PenaltyList")
                    .Include("PenaltyList.Display")
                    .Single(x => x.Id == templateId)
                    .PenaltyList.Single(x => x.Id == penalty.Id);
                templatePenalty.TotalDismissal = penalty.TotalDismissal;
                templatePenalty.PenaltySeconds = penalty.PenaltySeconds;
                templatePenalty.Display = penalty.Display;
                templatePenalty.Name = penalty.Name;
                templatePenalty.RuleNumber = penalty.RuleNumber;
                templatePenalty.Note = penalty.Note;

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Remove a template from database
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task DeleteTemplate(TblTemplate template)
        {
            try
            {
                using var dbContext = new LeDiDbContext();
                var toDelete = dbContext.TblTemplates.SingleOrDefault(x => x.Id == template.Id);
                if (toDelete != null)
                {
                    dbContext.TblTemplates.Remove(toDelete);

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        /// <summary>
        /// Remove  a template from database
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task DeleteTemplate(int templateId)
        {
            try
            {
                using var dbContext = new LeDiDbContext();
                var toDelete = dbContext.TblTemplates.SingleOrDefault(x => x.Id == templateId);
                if (toDelete != null)
                {
                    dbContext.TblTemplates.Remove(toDelete);

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// Remove a templatepenalty for a template from database
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task DeleteTemplatePenalty(TblTemplatePenaltyItem templatePenalty)
        {
            try
            {
                using var dbContext = new LeDiDbContext();
                var toDeleteParent = dbContext.TblTemplates.Include("PenaltyList").SingleOrDefault(x => x.Id == templatePenalty.Template.Id);
                if (toDeleteParent == null)
                    return;

                var toDelete = toDeleteParent.PenaltyList.FirstOrDefault(x => x.Id == templatePenalty.Id);

                if (toDelete == null)
                    return;

                toDeleteParent.PenaltyList.Remove(toDelete);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }







        /// <summary>
        /// Sends a match update to connected Clients, that are showing this match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public static async Task SendMatch(int matchId, TblMatch? match = null)
        {
            if (hubContext == null)
                return;

            if (match == null)
            {
                match = await GetMatchAsync(matchId);
            }

            // Remove the Match Events. They are not relevant and cause Dependency Cycles.
            match.MatchEvents = new List<TblMatchEvent>();

            try
            {
                var group = hubContext.Clients.Group("Match-" + matchId);
                if (group != null)
                {
                    await group.SendAsync("ReceiveMatch", match);
                }
            }
            catch (Exception ex)
            {
                Logger.Trace(ex, "Cannot send match to group {0} as the group does not exists.", "Match-" + matchId);
            }
        }
    }
}
