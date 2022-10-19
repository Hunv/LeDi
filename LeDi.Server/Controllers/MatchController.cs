using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeDi.Shared.DtoModel;

namespace LeDi.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMatchList()
        {
            _logger.Trace("{0}: Get Matchlist", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiMatch.GetMatchList();
            var result = new OkObjectResult(json);

            _logger.Trace("{0}: Got Matchlist JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}")]
        public IActionResult GetMatch(int matchId)
        {
            _logger.Trace("{0}: Get Match {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var json = Api.ApiMatch.GetMatch(matchId);
            var result = new OkObjectResult(json);

            _logger.Trace("{0}: Got Match ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, json);
            return result;
        }

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}/full")]
        public IActionResult GetMatchFull(int matchId)
        {
            _logger.Trace("{0}: Get MatchFull {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var json = Api.ApiMatch.GetMatchFull(matchId);
            var result = new OkObjectResult(json);

            _logger.Trace("{0}: Got MatchFull ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, json);
            return result;
        }

        /// <summary>
        /// Gets the remaining seconds only
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}/time")]
        public IActionResult GetMatchTime(int matchId)
        {
            _logger.Trace("{0}: Get MatchTime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var json = Api.ApiMatch.GetMatchTime(matchId);
            var result = new OkObjectResult(json);

            _logger.Trace("{0}: Got MatchTime ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, json);
            return result;
        }

        /// <summary>
        /// Gets the remaining seconds and a hash of all other values. If they changed, the hash will change and the client will know to request a full match dataset
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}/core")]
        public async Task<IActionResult> GetMatchHash(int matchId)
        {
            _logger.Trace("{0}: Get Match Core for ID {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var json = await Api.ApiMatch.GetMatchCore(matchId);
            var result = new OkObjectResult(json);

            _logger.Trace("{0}: Got Match Core for ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, json);
            return result;
        }

        /// <summary>
        /// Modify an existing Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}")]
        public async Task<IActionResult> SetMatch(
            [FromBody] DtoMatch match,
            int matchId
            )
        {
            _logger.Trace("{0}: Set Match {1}", Request.HttpContext.Connection.RemoteIpAddress, match.Id);

            await Api.ApiMatch.SetMatch(match, matchId);
            
            _logger.Trace("{0}: Set Match ID {1}", Request.HttpContext.Connection.RemoteIpAddress, match.Id);
            return new OkResult();
        }

        /// <summary>
        /// Start the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/time/start")]
        public async Task<IActionResult> SetMatchtimeStart(int matchId)
        {
            _logger.Trace("{0}: Start Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            await Api.ApiMatch.StartMatchtime(matchId);

            _logger.Trace("{0}: Started Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkResult();
        }


        /// <summary>
        /// Stops the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/time/stop")]
        public async Task<IActionResult> SetMatchtimeStop(int matchId)
        {
            _logger.Trace("{0}: Stop Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            await Api.ApiMatch.PauseMatchtime(matchId);

            _logger.Trace("{0}: Stopped Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkResult();
        }

        /// <summary>
        /// Load the next period after another period ended
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/next")]
        public async Task<IActionResult> SetMatchPeriod(int matchId)
        {
            _logger.Trace("{0}: Set next Match period {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            await Api.ApiMatch.NextPeriod(matchId);

            _logger.Trace("{0}: Set next Match period {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkResult();
        }


        /// <summary>
        /// Stops the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchid}/goal/{teamid}/{amount}")]
        public async Task<IActionResult> SetMatchGoal(int matchid, int teamid, int amount)
        {
            _logger.Trace("{0}: Setting Goal in Match {1} for team {2} by {3}", Request.HttpContext.Connection.RemoteIpAddress, matchid, teamid, amount);

            await Api.ApiMatch.SetMatchGoal(matchid, teamid, amount);

            _logger.Trace("{0}: Set Goal in Match {1} for team {2} by {3}", Request.HttpContext.Connection.RemoteIpAddress, matchid, teamid, amount);
            return new OkResult();
        }



        /// <summary>
        /// Add a new Match
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> NewMatch(
            [FromBody] DtoMatch match
            )
        {
            _logger.Trace("{0}: New Match", Request.HttpContext.Connection.RemoteIpAddress);

            var newMatchDto = await Api.ApiMatch.NewMatch(match);

            _logger.Trace("{0}: New Match", Request.HttpContext.Connection.RemoteIpAddress);
            return new OkObjectResult(newMatchDto);
        }

        /// <summary>
        /// Set the remaining seconds
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/time")]
        public async Task<IActionResult> SetMatchTime(
            int matchId, 
            [FromQuery]int timeleft
            )
        {
            _logger.Trace("{0}: Set MatchTime {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, timeleft);

            await Api.ApiMatch.SetMatchTime(matchId, timeleft);
            
            _logger.Trace("{0}: Set MatchTime ID {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, timeleft);
            return new OkResult(); 
        }

        /// <summary>
        /// Set the remaining seconds
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/status")]
        public async Task<IActionResult> SetMatchStatus(
            int matchId,
            [FromBody] int status
            )
        {
            _logger.Trace("{0}: Set MatchStatus {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, status);

            await Api.ApiMatch.SetMatchStatus(matchId, status);

            _logger.Trace("{0}: Set MatchStatus ID {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, status);
            return new OkResult();
        }

        /// <summary>
        /// Gets all live Matches
        /// </summary>
        /// <returns></returns>
        [HttpGet("live")]
        public IActionResult GetLiveMatchList()
        {
            _logger.Trace("{0}: Get Livematchlist", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiMatch.GetLiveMatchList();
            var result = new OkObjectResult(json);

            _logger.Trace("{0}: Got Livematchlist JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Get match events
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}/events")]
        public IActionResult GetMatchEvents(
            int matchId
            )
        {
            _logger.Trace("{0}: Get MatchEvents for {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var dto = Api.ApiMatch.GetMatchEvents(matchId);

            _logger.Trace("{0}: Got MatchEvents for {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkObjectResult(dto);
        }


        /// <summary>
        /// Get match penalties
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}/penalty")]
        public IActionResult GetMatchPenalty(
            int matchId
            )
        {
            _logger.Trace("{0}: Get MatchPenalties for {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var dto = Api.ApiMatch.GetMatchPenalties(matchId);

            _logger.Trace("{0}: Got MatchPenalties for {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkObjectResult(dto);
        }


        /// <summary>
        /// Create a new matchPealty
        /// </summary>
        /// <returns></returns>
        [HttpPost("{matchId}/penalty")]
        public async Task<IActionResult> CreateMatchPenalty(
            int matchId,
            [FromBody] DtoMatchPenalty penalty
            )
        {
            _logger.Trace("{0}: Set MatchPenalty to {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var dto = await Api.ApiMatch.NewMatchPenalty(matchId, penalty);

            _logger.Trace("{0}: Set MatchPenalty to ID {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkObjectResult(dto);
        }

        /// <summary>
        /// Revokes a matchPenalty
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/penalty/{penaltyId}")]
        public async Task<IActionResult> RevokeMatchPenalty(
            int matchId,
            int penaltyId,
            [FromBody] string revokeNote
            )
        {
            _logger.Trace("{0}: Revoke MatchPenalty {1} from match {2}", Request.HttpContext.Connection.RemoteIpAddress, penaltyId, matchId);

            var dto = await Api.ApiMatch.RevokeMatchPenalty(matchId, penaltyId, revokeNote);

            _logger.Trace("{0}: Revoke MatchPenalty ID {1} from match {2}", Request.HttpContext.Connection.RemoteIpAddress, penaltyId, matchId);
            return new OkObjectResult(dto);
        }
    }
}
