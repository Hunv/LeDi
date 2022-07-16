using Tiwaz.Server.DatabaseModel;
using Tiwaz.Server.Api.DtoModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            using(var dbContext = new TwDbContext())
            {
                List<DtoMatch>? dto = dbContext.Matches.Select(aMatch => aMatch.ToDto()).ToList();
                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                return json;
            }
        }

        /// <summary>
        /// Gets a specific match as JSON
        /// </summary>
        /// <param name="id">ID of the Match</param>
        /// <returns>Specific match details as JSON</returns>
        public static string GetMatch(int id)
        {
            using (var dbContext = new TwDbContext())
            {
                Match? dto = dbContext.Matches.SingleOrDefault(x => x.Id == id);
                if (dto != null)
                    return JsonConvert.SerializeObject(dto.ToDto(), Helper.GetJsonSerializer());                
                else
                    return "";
            }
        }

        /// <summary>
        /// Sets parameters of a match
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public async static Task SetMatch(DtoMatch match)
        {
            using (var dbContext = new TwDbContext())
            {
                Match? dto = dbContext.Matches.SingleOrDefault(x => x.Id == match.Id);
                if (dto != null)
                {
                    if (match.Team1Score.HasValue)
                        dto.Team1Score = match.Team1Score.Value;

                    if (match.Team2Score.HasValue)
                        dto.Team2Score = match.Team2Score.Value;

                    if (!string.IsNullOrEmpty(match.Team1Name))
                        dto.Team1Name = match.Team1Name;

                    if (!string.IsNullOrEmpty(match.Team2Name))
                        dto.Team2Name = match.Team2Name;

                    if (match.TimeLeftSeconds.HasValue)
                        dto.CurrentTimeLeft = match.TimeLeftSeconds.Value;

                    if (match.MatchStatus != 0)
                        dto.MatchStatus = match.MatchStatus;

                    if (match.ScheduledTime.HasValue)
                        dto.ScheduledTime = match.ScheduledTime;
                    
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
            using (var dbContext = new TwDbContext())
            {
                var newMatch = new Match()
                {
                    Team1Score = match.Team1Score ?? 0,
                    Team2Score = match.Team2Score ?? 0,
                    Team1Name = match.Team1Name,
                    Team2Name = match.Team2Name,
                    CurrentTimeLeft = match.TimeLeftSeconds ?? 0,
                    ScheduledTime = match.ScheduledTime,
                    GameName = match.GameName,
                    Team1PlayerIds = match.Team1PlayerIds,
                    Team2PlayerIds = match.Team2PlayerIds,
                    MatchStatus = match.MatchStatus == 0 ? 1 : match.MatchStatus //set to draft status if no Status is set
                };

                dbContext.Matches.Add(newMatch);

                await dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the time left of a specific match
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetMatchTime(int id)
        {
            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == id);
                if (dto != null)
                    return JsonConvert.SerializeObject(dto.CurrentTimeLeft, Helper.GetJsonSerializer());
                else
                    return "";
            }
        }

        /// <summary>
        /// Set the time left in seconds of a specific match
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeleft"></param>
        /// <returns></returns>
        public static async Task SetMatchTime(int id, int timeleft)
        {
            using (var dbContext = new TwDbContext())
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
        public static void StartMatchtime(int matchId)
        {
            if (MatchEngine.CurrentMatch.Id != matchId)
            {
                Console.WriteLine("MatchId {0} is not loaded currently.", matchId);
            }
            else
            {
                MatchEngine.StartMatch();
            }
        }

        /// <summary>
        /// Pause time for a specific match
        /// </summary>
        /// <param name="id"></param>
        public static void PauseMatchtime(int matchId)
        {
            if (MatchEngine.CurrentMatch.Id != matchId)
            {
                Console.WriteLine("MatchId {0} is not loaded currently.", matchId);
            }
            else
            {
                MatchEngine.PauseMatch();
            }
        }

        /// <summary>
        /// Ends a match
        /// </summary>
        /// <param name="matchId"></param>
        public static void EndMatch(int matchId)
        {

            if (MatchEngine.CurrentMatch.Id != matchId)
            {
                Console.WriteLine("MatchId {0} is not loaded currently.", matchId);
            }
            else
            {
                MatchEngine.EndMatch();
            }
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
            using (var dbContext = new TwDbContext())
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
    }
}
