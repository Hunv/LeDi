using LeDi.Server.DatabaseModel;
using LeDi.Shared.DtoModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeDi.Shared;
using LeDi.Shared.Enum;
using LeDi.Server.Classes;

namespace LeDi.Server.Api
{
    public static class ApiMatch
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a List of all Matches as JSON
        /// </summary>
        /// <returns>List of all Matches as JSON</returns>
        public static string GetMatchList()
        {
            Logger.Trace("Executing GetMatchList...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                List<DtoMatch>? dto = dbContext.Matches.Select(aMatch => aMatch.ToDto()).ToList();
                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Debug("GetMatchList returns the following result: {0}", json);
                return json;
            }

            Logger.Debug("GetMatchList returns an empty response.");
            return "";
        }

        /// <summary>
        /// Gets a specific match as JSON
        /// </summary>
        /// <param name="matchId">ID of the Match</param>
        /// <returns>Specific match details as JSON</returns>
        public static string GetMatch(int matchId)
        {
            Logger.Trace("Executing GetMatch...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                Match? dto = dbContext.Matches
                    .Include("MatchPenalties")
                    .SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                {
                    var json = JsonConvert.SerializeObject(dto.ToDto(), Helper.GetJsonSerializer());
                    Logger.Debug("GetMatch for match {0} returns the follwing result: {1}", matchId, json);
                    return json;
                }
                else
                {
                    Logger.Debug("GetMatch for match {0} returns an empty response.", matchId);
                    return "";
                }
            }

            Logger.Debug("GetMatch for match {0} returns an empty response.", matchId);
            return "";
        }

        /// <summary>
        /// Gets a specific match as JSON including ALL properties (incl. subtrees)
        /// </summary>
        /// <param name="matchId">ID of the Match</param>
        /// <returns>Specific match details as JSON</returns>
        public static string GetMatchFull(int matchId)
        {
            Logger.Trace("Executing GetMatchFull...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                Match? dto = dbContext.Matches
                    .Include("MatchPenalties")
                    .Include("MatchEvents")
                    .Include("MatchReferees")
                    .Include("RulePenaltyList")
                    .Include("RulePenaltyList.Display")
                    .Include("Devices")
                    .SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                {
                    var json = JsonConvert.SerializeObject(dto.ToDto(), Helper.GetJsonSerializer());
                    Logger.Debug("GetMatchFull for match {0} returns the following result: {1}", matchId, json);
                    return json;
                }
                else
                {
                    Logger.Debug("GetMatchFull for match {0} returns an empty response.", matchId);
                    return "";
                }
            }

            Logger.Debug("GetMatchFull for match {0} returns an empty response.", matchId);
            return "";
        }

        /// <summary>
        /// Sets parameters of a match. Null or 0 values will be ignored and not updated.
        /// </summary>
        /// <param name="match">The match object with the new properties.</param>
        /// <returns></returns>
        public async static Task SetMatch(DtoMatch match, int matchId)
        {
            Logger.Trace("Executing SetMatch...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                Match? dbMatch = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dbMatch != null)
                {
                    // Perform actions that need to be done on Status change
                    if (match.MatchStatus.HasValue && match.MatchStatus != dbMatch.MatchStatus)
                    {
                        await RunMatchStatusChangeActions(dbMatch, match.MatchStatus.Value);
                    }

                    if (match.Team1Score.HasValue)
                    {
                        dbMatch.Team1Score = match.Team1Score.Value;
                        await LogEvent(matchId, MatchEventEnum.ScoreTeam1, string.Format("{0} score was set to {1}", dbMatch.Team1Name, match.Team1Score.Value));
                        Logger.Debug("Team1Score was set to {0} for {1}", dbMatch.Team1Score, matchId);
                    }

                    if (match.Team2Score.HasValue)
                    {
                        dbMatch.Team2Score = match.Team2Score.Value;
                        await LogEvent(matchId, MatchEventEnum.ScoreTeam1, string.Format("{0} score was set to {1}", dbMatch.Team2Name, match.Team2Score.Value));
                        Logger.Debug("Team2Score was set to {0} for {1}", dbMatch.Team2Score, matchId);
                    }

                    if (!string.IsNullOrEmpty(match.Team1Name))
                    {
                        dbMatch.Team1Name = match.Team1Name;
                        Logger.Debug("Team1Name was set to {0} for {1}", dbMatch.Team1Name, matchId);
                    }

                    if (!string.IsNullOrEmpty(match.Team2Name))
                    {
                        dbMatch.Team2Name = match.Team2Name;
                        Logger.Debug("Team2Name was set to {0} for {1}", dbMatch.Team2Name, matchId);
                    }

                    if (match.TimeLeftSeconds.HasValue)
                    {
                        dbMatch.CurrentTimeLeft = match.TimeLeftSeconds.Value;
                        await LogEvent(matchId, MatchEventEnum.Undefined, string.Format("Time left was set to {0}", match.TimeLeftSeconds.Value));
                        Logger.Debug("CurrentTimeLeft was set to {0} for {1}", dbMatch.CurrentTimeLeft, matchId);
                    }

                    if (match.MatchStatus.HasValue)
                    {
                        //Check if a restart of the match was performed. The current status must be something that is "running" or later and the new status must be "ready to start".
                        if ((dbMatch.MatchStatus != (int)MatchStatusEnum.Planned && dbMatch.MatchStatus != (int)MatchStatusEnum.ReadyToStart && dbMatch.MatchStatus != (int)MatchStatusEnum.Undefined) &&
                            match.MatchStatus == (int)MatchStatusEnum.ReadyToStart)
                        {
                            await LogEvent(matchId, MatchEventEnum.MatchRestarted, "Match restarted.");
                        }
                        dbMatch.MatchStatus = match.MatchStatus.Value;
                        Logger.Debug("MatchStatus was set to {0} for {1}", dbMatch.MatchStatus, matchId);
                    }

                    if (match.ScheduledTime.HasValue)
                    {
                        dbMatch.ScheduledTime = match.ScheduledTime;
                        Logger.Debug("ScheduledTime was set to {0} for {1}", dbMatch.ScheduledTime, matchId);
                    }

                    if (match.RulePeriodCount.HasValue)
                    {
                        dbMatch.RulePeriodCount = match.RulePeriodCount.Value;
                        Logger.Debug("RulePeriodCount was set to {0} for {1}", dbMatch.RulePeriodCount, matchId);
                    }

                    if (match.PeriodCurrent.HasValue)
                    {
                        dbMatch.CurrentPeriod = match.PeriodCurrent.Value;
                        Logger.Debug("CurrentPeriod was set to {0} for {1}", dbMatch.CurrentPeriod, matchId);
                    }

                    Logger.Debug("SetMatch for match {0} executed.", matchId);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Create a new Match
        /// </summary>
        /// <param name="match">The DtoMatch object to create a new match from.</param>
        /// <returns></returns>
        public async static Task<DtoMatch?> NewMatch(DtoMatch match)
        {
            Logger.Trace("Executing NewMatch...");

            using var dbContext = new TwDbContext();

            var newMatch = new Match();
            newMatch.FromDto(match);

            if (dbContext.Matches == null)
                return null;

            dbContext.Matches.Add(newMatch);

            //This adds the ID to the newMatch variable
            await dbContext.SaveChangesAsync();

            var json = newMatch.ToDto();
            Logger.Debug("NewMatch returns the following result: {0}", json);
            return json;
        }

        /// <summary>
        /// Gets the time left of a specific match
        /// </summary>
        /// <param name="matchId">ID of the match</param>
        /// <returns>The time left in seconds as a string</returns>
        public static string GetMatchTime(int matchId)
        {
            Logger.Trace("Executing GetMatchTime...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                {
                    var json = JsonConvert.SerializeObject(dto.CurrentTimeLeft, Helper.GetJsonSerializer());
                    Logger.Debug("GetMatchTime for match {0} returns the following result: {1}", matchId, json);
                    return json;
                }
                else
                {
                    Logger.Debug("GetMatchTime for match {0} returns an empty response.", matchId);
                    return "";
                }
            }

            Logger.Debug("GetMatchTime for match {0} returns an empty response.", matchId);
            return "";
        }

        /// <summary>
        /// Gets the Match time and a hash of all other values.
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The JSON with the current time left and a hash over all other properties relevant for a live match.</returns>
        public static async Task<string> GetMatchCore(int matchId)
        {
            Logger.Trace("Executing GetMatchCore...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var match = await dbContext.Matches.SingleOrDefaultAsync(x => x.Id == matchId);
                if (match != null)
                {
                    // Create a hash of match values that might change
                    var propHash = (match.CurrentPeriod + "@@@" + string.Join(',', match.MatchEvents.Select(x => x.Id)) + "@@@" + match.MatchStatus + "@@@" + match.Team1Score + "@@@" + match.Team2Score + "@@@" + match.ScheduledTime).GetHashCode();

                    // Create the DtoMatchCore object
                    var dto = new DtoMatchCore() { TimeLeftSeconds = match.CurrentTimeLeft, PropertyHash = propHash };

                    var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                    Logger.Trace("GetMatchCore for match {0} returns the following result: {1}", matchId, json);
                    return json;
                }
                else
                {
                    Logger.Debug("GetMatchCore for match {0} returns an empty response.", matchId);
                    return "";
                }
            }

            Logger.Debug("GetMatchCore for match {0} returns an empty response.", matchId);
            return "";
        }

        /// <summary>
        /// Sets the matchstatus only by a matchId
        /// </summary>
        /// <param name="matchId">The ID of the match to set the status for</param>
        /// <param name="newStatus">The new Status ID of the match</param>
        /// <returns></returns>
        public static async Task SetMatchStatus(int matchId, int newStatus)
        {
            Logger.Trace("Executing SetMatchStatus...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                Match? dbMatch = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dbMatch != null)
                {   
                    dbMatch.MatchStatus = newStatus;

                    await dbContext.SaveChangesAsync();
                    await RunMatchStatusChangeActions(dbMatch, newStatus);

                    Logger.Debug("Set match status for {0} to {1}.", matchId, newStatus);

                    if (newStatus == (int)MatchStatusEnum.Canceled)
                    {
                        await LogEvent(matchId, MatchEventEnum.MatchCancel, "Match canceled.");
                    }
                }
            }

        }

        /// <summary>
        /// Set the time left in seconds of a specific match
        /// </summary>
        /// <param name="matchId">The ID of the match to set the time for</param>
        /// <param name="timeleft">The time left in seconds for the match</param>
        /// <returns></returns>
        public static async Task SetMatchTime(int matchId, int timeleft)
        {
            Logger.Trace("Executing SetMatchTime...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                {
                    dto.CurrentTimeLeft = timeleft;
                    await dbContext.SaveChangesAsync();

                    Logger.Debug("Set match time for {0} to {1}.", matchId, timeleft);
                }
            }
        }


        /// <summary> 
        /// Start time for a specific match
        /// </summary>
        /// <param name="matchId">The match ID of the match to start the time for</param>
        public static async Task StartMatchtime(int matchId)
        {
            Logger.Trace("Executing StartMatchtime...");

            //If match not already exist, create a new one
            if (!MatchEngine.OngoingMatches.Any(x => x.MatchId == matchId))
            {
                MatchEngine.AddOngoingMatch(new MatchHandler(matchId));
            }

            // Get the match object
            var match = MatchEngine.OngoingMatches.Single(x => x.MatchId == matchId);

            // Create the match event
            using var dbContext = new TwDbContext();

            // Add the mathc event
            if (dbContext.Matches != null)
            {
                var matchDb = dbContext.Matches.Include("MatchEvents").SingleOrDefault(x => x.Id == matchId);
                if (matchDb != null)
                {
                    //Check if the match was started before
                    if (matchDb.MatchEvents.Any(x => x.Event == MatchEventEnum.MatchStart))
                    {
                        await LogEvent(matchId, MatchEventEnum.MatchResumed, "Match resumed.");
                    }
                    else
                    {
                        await LogEvent(matchId, MatchEventEnum.MatchStart, "Match started.");
                    }
                }
            }

            //Start the match
            await match.Start();
        }

        /// <summary>
        /// Pause time for a specific match
        /// </summary>
        /// <param name="id">The match ID of the match to pause the time for</param>
        public static async Task PauseMatchtime(int matchId)
        {
            Logger.Trace("Executing PauseMatchtime...");

            var matchHandler = MatchEngine.OngoingMatches.SingleOrDefault(x => x.MatchId == matchId);
            if (matchHandler != null)
            {
                matchHandler.Stop();

                // Create the match event
                await LogEvent(matchId, MatchEventEnum.MatchPaused, "Match paused.");
            }
        }

        /// <summary>
        /// Ends a match
        /// </summary>
        /// <param name="matchId">The match ID of the match to end</param>
        public static async Task EndMatch(int matchId)
        {
            Logger.Trace("Executing EndMatch...");

            var matchHandler = MatchEngine.OngoingMatches.SingleOrDefault(x => x.MatchId == matchId);
            if (matchHandler != null)
            {
                MatchEngine.OngoingMatches.Remove(matchHandler);

                // Create the match event
                await LogEvent(matchId, MatchEventEnum.MatchEnd, "Match ended.");
            }
        }

        /// <summary>
        /// Loads the next match period
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public static async Task NextPeriod(int matchId)
        {
            Logger.Trace("Executing NextPeriod for matchId {0}...", matchId);

            var matchHandler = MatchEngine.OngoingMatches.SingleOrDefault(x => x.MatchId == matchId);
            if (matchHandler != null)
            {
                await matchHandler.NextPeriod();

                // Create the match event
                //await LogEvent(matchId, MatchEventEnum., "Match period loaded.");
            }
        }

        /// <summary>
        /// Logs an event match
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="matchEvent"></param>
        /// <param name="matchText"></param>
        /// <returns></returns>
        private static async Task LogEvent(int matchId, MatchEventEnum matchEvent, string matchText)
        {
            Logger.Trace("Logging new matchevent {0} for match {1} with text {2}...", (int)matchEvent, matchId, matchText);

            using var dbContext = new TwDbContext();
            if (dbContext.Matches != null)
            {
                var match = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (match != null)
                {
                    if (match.MatchEvents == null)
                        match.MatchEvents = new List<MatchEvent>();

                    if (match.MatchEvents != null)
                    {
                        var timeSinceStart = match.RulePeriodLength - match.CurrentTimeLeft + (match.CurrentPeriod -1) * match.RulePeriodLength;
                        if (timeSinceStart < 0)
                            timeSinceStart = 0;

                        match.MatchEvents.Add(new MatchEvent
                        {
                            Event = matchEvent,
                            Text = matchText,
                            Timestamp = DateTime.Now,
                            Matchtime = timeSinceStart
                        });
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Set a new value for a team score
        /// </summary>
        /// <param name="matchId">The ID of the match to count a goal for</param>
        /// <param name="teamId">The team ID of the team that gains a goal (zero based)</param>
        /// <param name="amount">The amount of goals/points to add for the team. Can also be negative to revoke points/goals.</param>
        /// <returns></returns>
        public static async Task SetMatchGoal(int matchId, int teamId, int amount)
        {
            Logger.Trace("Executing SetMatchGoal...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var match = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (match != null)
                {
                    if (teamId == 0)
                    {
                        match.Team1Score += amount;

                        // Create the match event
                        await LogEvent(matchId, MatchEventEnum.ScoreTeam1, match.Team1Name + " scored (" + (amount > 0 ? "+" : "") + amount + ").");
                    }
                    else if (teamId == 1)
                    {
                        match.Team2Score += amount;

                        // Create the match event
                        await LogEvent(matchId, MatchEventEnum.ScoreTeam2, match.Team2Name + " scored (" + (amount > 0 ? "+" : "") + amount + ").");
                    }

                    await dbContext.SaveChangesAsync();
                    Logger.Debug("SetMatchGoal added {0} scores for team {1} to match {2}", amount, teamId, matchId);
                }
            }

        }

        /// <summary>
        /// Gets a list of all ongoing matches
        /// </summary>
        /// <returns>JSON string with the list of all mathces that are currently loaded on the cache, so they are prepared, running or recently finished.</returns>
        public static string GetLiveMatchList()
        {
            Logger.Trace("Executing GetLiveMatchList...");

            using var dbContext = new TwDbContext();

            var ongoingMatchIds = MatchEngine.OngoingMatches.Select(x => x.MatchId);
            var dto = new List<DtoMatch>();

            if (dbContext.Matches != null)
            {
                foreach (var aMatch in dbContext.Matches.Where(x => ongoingMatchIds.Contains(x.Id)))
                    dto.Add(aMatch.ToDto());

                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Debug("GetLiveMatchList returns the following result: {0}", json);
                return json;
            }
            else
            {
                Logger.Debug("GetLiveMatchList returns an empty result. No match seem to be running.");
                return "{}";
            }
        }

        /// <summary>
        /// Gets a list of match events
        /// </summary>
        /// <returns>JSON string with the list of all match events</returns>
        public static string GetMatchEvents(int matchId)
        {
            Logger.Trace("Executing GetMatchEvents...");

            using var dbContext = new TwDbContext();

            var dto = new List<DtoMatchEvent>();

            if (dbContext.Matches != null && dbContext.MatchEvents != null)
            {
                var match = dbContext.Matches.Include("MatchEvents").SingleOrDefault(x => x.Id == matchId);
                if (match == null)
                    return "{}";

                var events = match.MatchEvents;
                if (events == null)
                    return "{}";

                foreach(var aEvent in events)
                    dto.Add(aEvent.ToDto());

                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Debug("GetMatchEvents for match {0} returns the following result: {1}", matchId, json);
                return json;
            }
            else
            {
                Logger.Debug("GetLiveMatchList returns an empty result. No match event for {0} available.", matchId);
                return "{}";
            }
        }

        /// <summary>
        /// Performs the actions required on changes of the Match Status (i.e. Stop time).
        /// </summary>
        /// <param name="dto">The current (old) Match object</param>
        /// <param name="newMatchStatus">the new match status</param>
        private async static Task RunMatchStatusChangeActions(Match dto, int newMatchStatus)
        {
            Logger.Trace("Executing RunMatchStatusChangeActions...");

            int[] stopTimeStatus = new int[] { (int)MatchStatusEnum.Canceled, (int)MatchStatusEnum.Closed, (int)MatchStatusEnum.Ended, (int)MatchStatusEnum.Planned, (int)MatchStatusEnum.ReadyToStart, (int)MatchStatusEnum.Stopped };
            int[] startTimeStatus = new int[] { (int)MatchStatusEnum.Running };

            // If the new Match status is a stop status
            if (stopTimeStatus.Contains(newMatchStatus))
            {
                if (MatchEngine.OngoingMatches.Any(x => x.MatchId == dto.Id))
                {
                    //Stop the match time
                    MatchEngine.OngoingMatches.Single(x => x.MatchId == dto.Id).Stop();
                }
            }
            else if (startTimeStatus.Contains(newMatchStatus))
            {

                if (MatchEngine.OngoingMatches.Any(x => x.MatchId == dto.Id))
                {
                    //Start the match time
                    await MatchEngine.OngoingMatches.Single(x => x.MatchId == dto.Id).Start();
                }
            }
        }

        /// <summary>
        /// Adds a new match penalty to database
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async static Task<string> NewMatchPenalty(int matchId, DtoMatchPenalty dto)
        {
            Logger.Trace("Executing NewMatchPenalty...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null && dbContext.MatchPenalties != null)
            {
                var match = dbContext.Matches.Include("MatchPenalties").SingleOrDefault(x => x.Id == matchId);
                if (match == null)
                    return "";

                var penalties = match.MatchPenalties;
                if (penalties == null)
                    match.MatchPenalties = new List<MatchPenalty>();

                var pen = new MatchPenalty();
                pen.FromDto(dto);
                pen.MatchId = matchId;

                dbContext.MatchPenalties.Add(pen);
                await dbContext.SaveChangesAsync();

                // Create the match event
                if (dto.PlayerNumber != 0)
                {
                    //await LogEvent(matchId, dto.TeamId == 0 ? MatchEventEnum.PenaltyTeam1 : MatchEventEnum.PenaltyTeam2, String.Format("Player {0} ({1}) got penalty \"{2}\"", dto.PlayerNumber, (dto.TeamId == 0 ? match.Team1Name : match.Team2Name), dto.PenaltyName) + (dto.PenaltyTime == 0 ? "" : " with " + dto.PenaltyTime + " seconds time penalty."));
                    await LogEvent(
                        matchId,
                        dto.TeamId == 0 ? MatchEventEnum.PenaltyTeam1 : MatchEventEnum.PenaltyTeam2,
                        string.Format(
                            "{2} for player #{0} ({1})",
                            dto.PlayerNumber,
                            (dto.TeamId == 0 ? match.Team1Name : match.Team2Name),
                            dto.PenaltyName
                        )
                    );
                }
                else
                {
                    //await LogEvent(matchId, dto.TeamId == 0 ? MatchEventEnum.PenaltyTeam1 : MatchEventEnum.PenaltyTeam2, string.Format("Team {0} got penalty \"{1}\"", (dto.TeamId == 0 ? match.Team1Name : match.Team2Name), dto.PenaltyName) + (dto.PenaltyTime == 0 ? "" : " with " + dto.PenaltyTime + " seconds time penalty."));
                    await LogEvent(
                        matchId,
                        dto.TeamId == 0 ? MatchEventEnum.PenaltyTeam1 : MatchEventEnum.PenaltyTeam2,
                        string.Format(
                            "{1} for team {0}",
                            (dto.TeamId == 0 ? match.Team1Name : match.Team2Name),
                            dto.PenaltyName
                        )
                    );
                }

                dto.Id = pen.Id;
                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Debug("NewMatchPenalty for match {0} returns the following result: {1}", matchId, json);
                return json;
            }

            Logger.Debug("NewMatchPenalty for match {0} returns an empty result.", matchId);
            return "";
        }

        /// <summary>
        /// Gets a list of penalties for a match
        /// </summary>
        /// <returns>JSON string with the list of all match penaties for a match</returns>
        public static string GetMatchPenalties(int matchId)
        {
            Logger.Trace("Executing GetMatchPenalties...");

            using var dbContext = new TwDbContext();

            var dto = new List<DtoMatchPenalty>();

            if (dbContext.Matches != null && dbContext.MatchPenalties != null)
            {
                var match = dbContext.Matches.Include("MatchPenalties").SingleOrDefault(x => x.Id == matchId);
                if (match == null)
                    return "{}";

                var penalties = match.MatchPenalties;
                if (penalties == null)
                    return "{}";

                foreach (var aPenalty in penalties)
                    dto.Add(aPenalty.ToDto());

                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Debug("GetMatchPenalties for match {0} returns the following result: {1}", matchId, json);
                return json;
            }
            else
            {
                Logger.Debug("GetMatchPenalties for match {0} returns an empty result. No match penalties available.", matchId);
                return "{}";
            }
        }

        /// <summary>
        /// Revokes a previously created match penalty from database
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="penaltyId"></param>
        /// <returns></returns>
        public async static Task<string> RevokeMatchPenalty(int matchId, int penaltyId, string revokeNote)
        {
            Logger.Trace("Executing RevokeMatchPenalty...");

            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null && dbContext.MatchPenalties != null)
            {
                var match = dbContext.Matches.Include("MatchPenalties").SingleOrDefault(x => x.Id == matchId);
                if (match == null)
                    return "";

                var penalties = match.MatchPenalties;
                if (penalties == null)
                    return "";

                var pen = penalties.SingleOrDefault(x => x.Id == penaltyId);
                if (pen == null)
                    return "";

                pen.Revoked = true;
                pen.RevokeNote = revokeNote;

                await dbContext.SaveChangesAsync();

                // Create the match event
                if (pen.PlayerNumber != 0)
                    await LogEvent(matchId, pen.TeamId == 0 ? MatchEventEnum.PenaltyTeam1Revoke : MatchEventEnum.PenaltyTeam2Revoke, string.Format("Revoked {0} for Player #{1} ({2}).", pen.PenaltyName, pen.PlayerNumber, (pen.TeamId == 0 ? match.Team1Name : match.Team2Name)));
                else
                    await LogEvent(matchId, pen.TeamId == 0 ? MatchEventEnum.PenaltyTeam1Revoke : MatchEventEnum.PenaltyTeam2Revoke, string.Format("Revoked {0} for Team ({1}).", pen.PenaltyName, (pen.TeamId == 0 ? match.Team1Name : match.Team2Name)));

                var dto = penalties.SingleOrDefault(x => x.Id == penaltyId);
                if (dto == null)
                    return "";

                var json = JsonConvert.SerializeObject(dto.ToDto(), Helper.GetJsonSerializer());
                Logger.Debug("RevokeMatchPenalty for penalty {0} for match {1} with note {2} returns the following result: {3}", penaltyId, matchId, revokeNote, json);
                return json;
            }
            return "";
        }
    }
}
