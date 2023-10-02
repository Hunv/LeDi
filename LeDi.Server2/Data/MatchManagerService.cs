using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Components;
using LeDi.Shared2.DatabaseModel;
using System.Linq;
using LeDi.Shared2.Enum;
using System.Reflection;

namespace LeDi.Server2.Data
{
    public class MatchManagerService
    {
        /// <summary>
        /// Logger instance for logging
        /// </summary>
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The timer, that is counting the time of the ongoing matches down.
        /// </summary>
        private System.Timers.Timer tmrMatchtime = new System.Timers.Timer();


        /// <summary>
        /// The timer, that checks if all connected displays showing the match they should show.
        /// </summary>
        private System.Timers.Timer tmrDisplayCheck = new System.Timers.Timer();

        /// <summary>
        /// Contains the matches that are currently loaded because any client accessed it or it is currently running
        /// </summary>
        public List<TblMatch> LoadedMatches { get; private set; } = new List<TblMatch>();


        public event EventHandler? OnPeriodTimeOver;
        public event EventHandler? OnMatchTimeUpdated;
        public event EventHandler? OnMatchUpdated;

        public MatchManagerService() 
        { 
            tmrMatchtime.Interval = 1000;
            tmrMatchtime.Elapsed += TmrMatchtime_Elapsed;
            tmrMatchtime.Start();

            tmrDisplayCheck.Interval = 10000;
            tmrDisplayCheck.Elapsed += TmrDisplayCheck_Elapsed;
            tmrDisplayCheck.Start();

            LoadMatches();
        }

        /// <summary>
        /// Timer checks if the displays connected are showing the matches they should show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TmrDisplayCheck_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await InitializeDisplay();
        }

        /// <summary>
        /// Loads all currently ongoing matches to the LoadedMatches variable
        /// </summary>
        private void LoadMatches()
        {
            //Get all matches, that are currently ongoing (or paused/stopped)
            var activeMatches = DataHandler.GetMatchList().Where(x => x.MatchStatus == (int)MatchStatusEnum.Running || x.MatchStatus == (int)MatchStatusEnum.Stopped).ToList();
            foreach(var aMatch in activeMatches)
            {
                Logger.Debug("Adding match {0} with status {1} to loaded matches.", aMatch.Id, aMatch.MatchStatus);
                LoadedMatches.Add(aMatch);
            }
        }

        /// <summary>
        /// Elapses every second to count down the match time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TmrMatchtime_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Debug("Matchtime timer elapsed.");

            foreach(var aMatch in LoadedMatches)
            {
                if (aMatch.MatchStatus == (int) MatchStatusEnum.Running)
                {
                    // if there is time left for the match or the match has overtime
                    if (aMatch.CurrentTimeLeft > 0 || aMatch.RulePeriodOvertime)
                    {
                        aMatch.CurrentTimeLeft--;
                        if (OnMatchTimeUpdated != null)
                        {
                            OnMatchTimeUpdated(aMatch.Id, e);
                        }

                        // Update the new time left in the database
                        await DataHandler.SetMatchTimeAsync(aMatch.Id, aMatch.CurrentTimeLeft);                     

                    }
                    else // if the time of a match is over and the match has no overtime
                    {
                        var dbMatch = await DataHandler.GetMatchAsync(aMatch.Id);
                        
                        //stop the timer by setting the new status. Set to PeriodEnded if there are more periods left. Set to Ended if all periods are over.
                        if (aMatch.CurrentPeriod < aMatch.RulePeriodCount)
                        {
                            aMatch.MatchStatus = (int)MatchStatusEnum.PeriodEnded;
                            var matchEvent = new TblMatchEvent() { Event = MatchEventEnum.MatchCancel, Matchtime = aMatch.GetMatchTime(), Timestamp = DateTime.UtcNow, Source = "MatchManager", Text = "Status set to PeriodEnded" };
                            aMatch.MatchEvents.Add(matchEvent);
                        }
                        else
                        {
                            aMatch.MatchStatus = (int)MatchStatusEnum.Ended;
                            var matchEvent = new TblMatchEvent() { Event = MatchEventEnum.MatchCancel, Matchtime = aMatch.GetMatchTime(), Timestamp = DateTime.UtcNow, Source = "MatchManager", Text = "Status set to MatchEnded" };
                            aMatch.MatchEvents.Add(matchEvent);
                        }

                        //Save changes to database
                        dbMatch.MatchStatus = aMatch.MatchStatus;
                        dbMatch.MatchEvents = aMatch.MatchEvents;
                        await DataHandler.SetMatchAsync(dbMatch);

                        if (OnPeriodTimeOver != null)
                        {
                            OnPeriodTimeOver(aMatch.Id, e);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Sets a new Match Status
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task SetMatchStatus(int matchId, MatchStatusEnum newStatus)
        {
            if (await LoadMatch(matchId) == false)
            {
                Logger.Warn("Cannot set match {0}.", matchId);
                return;
            }

            var match = LoadedMatches.Single(x => x.Id == matchId);

            // Check if a match is being started. If yes, also count the period
            if (match.MatchStatus == (int)MatchStatusEnum.Planned && newStatus == MatchStatusEnum.Running){
                if (match.CurrentPeriod == 0)
                {
                    match.CurrentPeriod++;
                }
            }

            // Set the new matchstatus
            match.MatchStatus = (int)newStatus;

            // Create an event for logging
            var matchEvent = new TblMatchEvent() { 
                Event = MatchEventEnum.MatchCancel, 
                Matchtime = match.GetMatchTime(), 
                Timestamp = DateTime.UtcNow, 
                Source = "MatchManager", 
                Text = "Status set to " + newStatus.ToString() 
            };
            match.MatchEvents.Add(matchEvent);

            // Save changes to db.
            var dbMatch = await DataHandler.GetMatchAsync(matchId);
            dbMatch.MatchStatus = match.MatchStatus;
            dbMatch.CurrentPeriod = match.CurrentPeriod;
            dbMatch.MatchEvents.Add(matchEvent);
            await DataHandler.SetMatchAsync(dbMatch);

            if (OnMatchUpdated != null)
            {
                OnMatchUpdated(matchId, new EventArgs());
            }
        }

        /// <summary>
        /// Adds a new penalty
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="playerId"></param>
        /// <param name="teamId"></param>
        /// <param name="penaltyText"></param>
        /// <param name="penaltySeconds"></param>
        /// <param name="penaltyTimeStart"></param>
        /// <returns></returns>
        public async Task AddMatchPenalty(TblMatchPenalty penalty)
        {

            if (await LoadMatch(penalty.MatchId) == false)
            {
                Logger.Warn("Cannot change match {0}.", penalty.MatchId);
                return;
            }

            var match = LoadedMatches.Single(x => x.Id == penalty.MatchId);

            match.MatchPenalties.Add(penalty);

            var matchEvent = new TblMatchEvent() { 
                Event = (penalty.TeamId == 0 ? MatchEventEnum.PenaltyTeam1 : MatchEventEnum.PenaltyTeam2), 
                Matchtime = match.GetMatchTime(), 
                Timestamp = DateTime.UtcNow, 
                Source = "MatchManager", 
                Text = "Penalty for team " + penalty.TeamId + " player #" + penalty.PlayerNumber + " because of " + penalty.PenaltyName 
            };
            match.MatchEvents.Add(matchEvent);

            // Save changes to db.
            var dbMatch = await DataHandler.GetMatchAsync(penalty.MatchId);
            dbMatch.MatchStatus = match.MatchStatus;
            dbMatch.MatchEvents.Add(matchEvent);
            dbMatch.MatchPenalties.Add(penalty);
            await DataHandler.SetMatchAsync(dbMatch);

            if (OnMatchUpdated != null)
            {
                OnMatchUpdated(penalty.MatchId, new EventArgs());
            }

        }

        /// <summary>
        /// Loads the next period of a match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task LoadNextPeriod(int matchId)
        {
            if (await LoadMatch(matchId) == false)
            {
                Logger.Warn("Cannot Perpare match {0}.", matchId);
                return;
            }

            var match = LoadedMatches.Single(x => x.Id == matchId);
            if (match.CurrentPeriod >= match.RulePeriodCount)
            {
                Logger.Warn("Cannot load next period for match {0} because current period is {1} of {2}.", matchId, match.CurrentPeriod, match.RulePeriodCount);
                return;
            }

            match.CurrentPeriod++;
            match.CurrentTimeLeft = match.RulePeriodLength;

            if (OnMatchUpdated != null)
            {
                OnMatchUpdated(matchId, new EventArgs());
            }
        }

        /// <summary>
        /// Gets a match and handles errors in case it is not possible.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TblMatch GetMatch(int matchId)
        {
            try
            {
                return LoadedMatches.Single(x => x.Id == matchId);
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Unable to get match with ID {0}.", matchId);
            }
            throw new Exception("Unable to get match");
        }

        /// <summary>
        /// Checks if a match is loaded to LoadedMatches and if not, it will be loaded, if yes, it will be updated/reloaded from database
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<bool> LoadMatch(int matchId)
        {
            // if the matchId is not loaded, load it
            if (!LoadedMatches.Any(x => x.Id == matchId))
            {
                var match = await DataHandler.GetMatchAsync(matchId);
                if (match == null)
                {
                    Logger.Warn("Unable to load matchId {0} because is seems not to exist.", matchId);
                    return false;
                }
                LoadedMatches.Add(match);
            }
            else
            {
                var match = LoadedMatches.Single(x => x.Id == matchId);
                var dbMatch = await DataHandler.GetMatchAsync(matchId);
                match.CurrentPeriod = dbMatch.CurrentPeriod;
                match.CurrentTimeLeft = dbMatch.CurrentTimeLeft;
                match.GameName = dbMatch.GameName;
                match.MatchEvents = dbMatch.MatchEvents;
                match.MatchPenalties = dbMatch.MatchPenalties;
                match.MatchReferees = dbMatch.MatchReferees;
                match.MatchStatus = dbMatch.MatchStatus;
                match.RuleMatchExtensionOnDraw = dbMatch.RuleMatchExtensionOnDraw;                
                match.RulePeriodCount = dbMatch.RulePeriodCount;
                match.RulePeriodLastPauseTimeOnEvent = dbMatch.RulePeriodLastPauseTimeOnEvent;
                match.RulePeriodLastPauseTimeOnEventSeconds= dbMatch.RulePeriodLastPauseTimeOnEventSeconds;
                match.RulePeriodLength = dbMatch.RulePeriodLength;
                match.RulePeriodOvertime = dbMatch.RulePeriodOvertime;
                match.ScheduledTime = dbMatch.ScheduledTime;
                match.Team1Name = dbMatch.Team1Name;
                match.Team1Players= dbMatch.Team1Players;
                match.Team1Score = dbMatch.Team1Score;
                match.Team2Name = dbMatch.Team2Name;
                match.Team2Players = dbMatch.Team2Players;
                match.Team2Score = dbMatch.Team2Score;
                match.Tournament = dbMatch.Tournament;
            }
            return true;
        }


        /// <summary>
        /// Checks if the current match should be shown on a display and sets the setting
        /// </summary>
        private async Task InitializeDisplay()
        {
            foreach (var match in LoadedMatches)
            {
                //Check if the match has display(s) to show the match on
                if (match.Devices.Count > 0)
                {
                    foreach (var aDev in match.Devices)
                    {
                        Logger.Debug("Setting display {0} to show match id {1} because it is listed as the display device.", aDev, match.Id);

                        var set = await DataHandler.GetDeviceSettingAsync(aDev.Device.DeviceId, "matchid");
                        if (set != null)
                        {
                            await DataHandler.SetDeviceSettingAsync(aDev.Device.DeviceId, "matchid", match.Id.ToString());
                            await DataHandler.AddDeviceCommandAsync(aDev.Device.DeviceId, "loadmatch", "");
                        }
                    }
                }
            }
        }
    }
}
