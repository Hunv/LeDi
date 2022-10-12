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

namespace LeDi.Server.Api
{
    public static class ApiMatch
    {
        /// <summary>
        /// Gets a List of all Matches as JSON
        /// </summary>
        /// <returns>List of all Matches as JSON</returns>
        public static string GetMatchList()
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                List<DtoMatch>? dto = dbContext.Matches.Select(aMatch => aMatch.ToDto()).ToList();
                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                return json;
            }
            return "";
        }

        /// <summary>
        /// Gets a specific match as JSON
        /// </summary>
        /// <param name="id">ID of the Match</param>
        /// <returns>Specific match details as JSON</returns>
        public static string GetMatch(int id)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                Match? dto = dbContext.Matches.SingleOrDefault(x => x.Id == id);
                if (dto != null)
                    return JsonConvert.SerializeObject(dto.ToDto(), Helper.GetJsonSerializer());
                else
                    return "";
            }

            return "";
        }

        /// <summary>
        /// Sets parameters of a match. Null or 0 values will be ignored and not updated.
        /// </summary>
        /// <param name="match">The match object with the new properties.</param>
        /// <returns></returns>
        public async static Task SetMatch(DtoMatch match, int matchId)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                Match? dto = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                {
                    // Perform actions that need to be done on Status change
                    if (match.MatchStatus != 0 && match.MatchStatus != dto.MatchStatus)
                    {
                        await RunMatchStatusChangeActions(dto, match.MatchStatus);
                    }

                    if (match.Team1Score.HasValue)
                        dto.Team1Score = match.Team1Score.Value;

                    if (match.Team2Score.HasValue)
                        dto.Team2Score = match.Team2Score.Value;

                    if (!string.IsNullOrEmpty(match.Team1Name))
                        dto.Team1Name = match.Team1Name;

                    if (!string.IsNullOrEmpty(match.Team2Name))
                        dto.Team2Name = match.Team2Name;

                    if (match.TimeLeftSeconds >= 0)
                        dto.CurrentTimeLeft = match.TimeLeftSeconds;

                    if (match.MatchStatus != 0)
                        dto.MatchStatus = match.MatchStatus;

                    if (match.ScheduledTime.HasValue)
                        dto.ScheduledTime = match.ScheduledTime;

                    if (match.HalfTimeCount >= 0)
                        dto.HalftimeCount = match.HalfTimeCount;

                    if (match.HalfTimeCurrent >= 0)
                        dto.CurrentHalftime = match.HalfTimeCurrent;

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
            using var dbContext = new TwDbContext();

            var newMatch = new Match()
            {
                Team1Score = match.Team1Score ?? 0,
                Team2Score = match.Team2Score ?? 0,
                Team1Name = match.Team1Name,
                Team2Name = match.Team2Name,
                CurrentTimeLeft = match.TimeLeftSeconds,
                HalftimeLength = match.TimeLeftSeconds, //TimeLeftSeconds is the Halftime Length on creation
                ScheduledTime = match.ScheduledTime,
                GameName = match.GameName,
                MatchStatus = match.MatchStatus == 0 ? 1 : match.MatchStatus, //set to draft status if no Status is set
                HalftimeCount = match.HalfTimeCount
                
            };

            // Add Team1 players
            if (match.Team1PlayerIds != null)
            {
                newMatch.Team1Players = new List<Player>();
                foreach (var aPlayerId in match.Team1PlayerIds)
                {
                    if (dbContext.Players != null)
                    {
                        var player = await dbContext.Players.SingleOrDefaultAsync(x => x.Id == aPlayerId);
                        if (player != null)
                            newMatch.Team1Players.Add(player);
                    }
                }
            }

            // Add Team2 players
            if (match.Team2PlayerIds != null)
            {
                newMatch.Team2Players = new List<Player>();
                foreach (var aPlayerId in match.Team2PlayerIds)
                {
                    if (dbContext.Players != null)
                    {
                        var player = await dbContext.Players.SingleOrDefaultAsync(x => x.Id == aPlayerId);
                        if (player != null)
                            newMatch.Team2Players.Add(player);
                    }
                }
            }

            if (dbContext.Matches == null)
                return null;

            dbContext.Matches.Add(newMatch);

            //This adds the ID to the newMatch variable
            await dbContext.SaveChangesAsync();


            return newMatch.ToDto();
        }

        /// <summary>
        /// Gets the time left of a specific match
        /// </summary>
        /// <param name="matchId">ID of the match</param>
        /// <returns>The time left in seconds as a string</returns>
        public static string GetMatchTime(int matchId)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                    return JsonConvert.SerializeObject(dto.CurrentTimeLeft, Helper.GetJsonSerializer());
                else
                    return "";
            }
            return "";
        }

        /// <summary>
        /// Gets the Match time and a hash of all other values.
        /// </summary>
        /// <param name="id">ID of the match</param>
        /// <returns>The JSON with the current time left and a hash over all other properties relevant for a live match.</returns>
        public static async Task<string> GetMatchCore(int matchId)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var match = await dbContext.Matches.SingleOrDefaultAsync(x => x.Id == matchId);
                if (match != null)
                {
                    // Create a hash of match values that might change
                    var propHash = (match.CurrentHalftime + "@@@" + string.Join(',', match.MatchEvents.Select(x => x.Id)) + "@@@" + match.MatchStatus + "@@@" + match.Team1Score + "@@@" + match.Team2Score).GetHashCode();

                    // Create the DtoMatchCore object
                    var dto = new DtoMatchCore() { TimeLeftSeconds = match.CurrentTimeLeft, PropertyHash = propHash };

                    var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                    return json;
                }
                else
                    return "";
            }
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
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                Match? dbMatch = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dbMatch != null)
                {   
                    dbMatch.MatchStatus = newStatus;

                    await dbContext.SaveChangesAsync();
                    await RunMatchStatusChangeActions(dbMatch, newStatus);
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
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                {
                    dto.CurrentTimeLeft = timeleft;
                    await dbContext.SaveChangesAsync();
                }
            }
        }


        /// <summary> 
        /// Start time for a specific match
        /// </summary>
        /// <param name="matchId">The match ID of the match to start the time for</param>
        public static async Task StartMatchtime(int matchId)
        {
            //If match not already exist, create a new one
            if (!MatchEngine.OngoingMatches.Any(x => x.MatchId == matchId))
            {
                MatchEngine.AddOngoingMatch(new MatchHandler(matchId));
            }

            
            var match = MatchEngine.OngoingMatches.Single(x => x.MatchId == matchId);

            // Create the match event
            using var dbContext = new TwDbContext();

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
            var matchHandler = MatchEngine.OngoingMatches.SingleOrDefault(x => x.MatchId == matchId);
            if (matchHandler != null)
            {
                MatchEngine.OngoingMatches.Remove(matchHandler);

                // Create the match event
                await LogEvent(matchId, MatchEventEnum.MatchEnd, "Match ended.");
            }
        }

        /// <summary>
        /// Loads the next match halfitme
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public static async Task NextHalftime(int matchId)
        {
            var matchHandler = MatchEngine.OngoingMatches.SingleOrDefault(x => x.MatchId == matchId);
            if (matchHandler != null)
            {
                await matchHandler.NextHalftime();

                // Create the match event
                //await LogEvent(matchId, MatchEventEnum., "Match halftime loaded.");
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
                        var timeSinceStart = match.HalftimeLength - match.CurrentTimeLeft + (match.CurrentHalftime -1) * match.HalftimeLength;

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
                }
            }

        }

        /// <summary>
        /// Gets a list of all ongoing matches
        /// </summary>
        /// <returns>JSON string with the list of all mathces that are currently loaded on the cache, so they are prepared, running or recently finished.</returns>
        public static string GetLiveMatchList()
        {
            using var dbContext = new TwDbContext();

            var ongoingMatchIds = MatchEngine.OngoingMatches.Select(x => x.MatchId);
            var dto = new List<DtoMatch>();

            if (dbContext.Matches != null)
            {
                foreach (var aMatch in dbContext.Matches.Where(x => ongoingMatchIds.Contains(x.Id)))
                    dto.Add(aMatch.ToDto());

                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                return json;
            }
            else
            {
                return "{}";
            }
        }

        /// <summary>
        /// Gets a list of match events
        /// </summary>
        /// <returns>JSON string with the list of all match events</returns>
        public static string GetMatchEvents(int matchId)
        {
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
                return json;
            }
            else
            {
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
        public async static Task<string> AddMatchPenalty(int matchId, DtoMatchPenalty dto)
        {
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

                await dbContext.MatchPenalties.AddAsync(pen);
                await dbContext.SaveChangesAsync();

                dto.Id = pen.Id;
                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());

                return json;
            }

            return "";
        }

        /// <summary>
        /// Gets a list of penalties for a match
        /// </summary>
        /// <returns>JSON string with the list of all match penaties for a match</returns>
        public static string GetMatchPenalties(int matchId)
        {
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
                return json;
            }
            else
            {
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

                var dto = penalties.SingleOrDefault(x => x.Id == penaltyId);
                if (dto == null)
                    return "";

                var json = JsonConvert.SerializeObject(dto.ToDto(), Helper.GetJsonSerializer());
                return json;
            }
            return "";
        }
    }
}
