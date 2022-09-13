using Tiwaz.Server.DatabaseModel;
using Tiwaz.Shared.DtoModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tiwaz.Shared;
using Tiwaz.Shared.Enum;

namespace Tiwaz.Server.Api
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
        /// Sets parameters of a match
        /// </summary>
        /// <param name="match"></param>
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
                        RunMatchStatusChangeActions(dto, match.MatchStatus);
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
        /// <param name="match"></param>
        /// <returns></returns>
        public async static Task NewMatch(DtoMatch match)
        {
            using var dbContext = new TwDbContext();

            var newMatch = new Match()
            {
                Team1Score = match.Team1Score ?? 0,
                Team2Score = match.Team2Score ?? 0,
                Team1Name = match.Team1Name,
                Team2Name = match.Team2Name,
                CurrentTimeLeft = match.TimeLeftSeconds,
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
                return;

            dbContext.Matches.Add(newMatch);

            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the time left of a specific match
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetMatchTime(int id)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == id);
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
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<string> GetMatchCore(int matchId)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var match = await dbContext.Matches.SingleOrDefaultAsync(x => x.Id == matchId);
                if (match != null)
                {
                    // Create a hash of match values that might change
                    var propHash = (match.CurrentHalftime + "@@@" + string.Join(',', match.MatchEventIds ?? new List<int> { 0 }) + "@@@" + match.MatchStatus + "@@@" + match.Team1Score + "@@@" + match.Team2Score).GetHashCode();

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
        /// Set the time left in seconds of a specific match
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeleft"></param>
        /// <returns></returns>
        public static async Task SetMatchTime(int id, int timeleft)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == id);
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
        /// <param name="matchId"></param>
        public static async Task StartMatchtime(int matchId)
        {
            //If match not already exist, create a new one
            if (!MatchEngine.OngoingMatches.Any(x => x.MatchId == matchId))
            {
                MatchEngine.AddOngoingMatch(new MatchHandler(matchId));
            }

            //Start the match
            await MatchEngine.OngoingMatches.Single(x => x.MatchId == matchId).Start();
        }

        /// <summary>
        /// Pause time for a specific match
        /// </summary>
        /// <param name="id"></param>
        public static void PauseMatchtime(int matchId)
        {
            var match = MatchEngine.OngoingMatches.SingleOrDefault(x => x.MatchId == matchId);
            if (match != null)
                match.Stop();
        }

        /// <summary>
        /// Ends a match
        /// </summary>
        /// <param name="matchId"></param>
        public static void EndMatch(int matchId)
        {
            var match = MatchEngine.OngoingMatches.SingleOrDefault(x => x.MatchId == matchId);
            if (match != null)
                MatchEngine.OngoingMatches.Remove(match);
        }

        /// <summary>
        /// Set a new value for a team score
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="teamId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async Task SetMatchGoal(int matchId, int teamId, int amount)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (dto != null)
                {
                    if (teamId == 0)
                        dto.Team1Score += amount;
                    else if (teamId == 1)
                        dto.Team2Score += amount;

                    await dbContext.SaveChangesAsync();
                }
            }

        }

        /// <summary>
        /// Gets a list of all ongoing matches
        /// </summary>
        /// <returns></returns>
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
                //Stop the match time
                MatchEngine.OngoingMatches.Single(x => x.MatchId == dto.Id).Stop();
            }
            else if (startTimeStatus.Contains(newMatchStatus))
            {
                //Start the match time
                await MatchEngine.OngoingMatches.Single(x => x.MatchId == dto.Id).Start();
            }
        }
    }
}
