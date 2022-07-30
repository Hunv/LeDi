using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tiwaz.Server.DatabaseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Tiwaz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly ILogger<MatchController> _logger;

        public MatchController(ILogger<MatchController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMatchList()
        {
            _logger.LogDebug("{0}: Get Matchlist", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiMatch.GetMatchList();
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Matchlist JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}")]
        public IActionResult GetMatch(int matchId)
        {
            _logger.LogDebug("{0}: Get Match {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var json = Api.ApiMatch.GetMatch(matchId);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Match ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, json);
            return result;
        }

        /// <summary>
        /// Gets the remaining seconds only
        /// </summary>
        /// <returns></returns>
        [HttpGet("{matchId}/time")]
        public IActionResult GetMatchTime(int matchId)
        {
            _logger.LogDebug("{0}: Get MatchTime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            var json = Api.ApiMatch.GetMatchTime(matchId);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got MatchTime ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, json);
            return result;
        }

        /// <summary>
        /// Modify an existing Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}")]
        public async Task<IActionResult> SetMatch(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoMatch match,
            int matchId
            )
        {
            _logger.LogDebug("{0}: Set Match {1}", Request.HttpContext.Connection.RemoteIpAddress, match.Id);

            await Api.ApiMatch.SetMatch(match, matchId);
            
            _logger.LogDebug("{0}: Set Match ID {1}", Request.HttpContext.Connection.RemoteIpAddress, match.Id);
            return new OkResult(); ;
        }

        /// <summary>
        /// Start the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/time/start")]
        public IActionResult SetMatchtimeStart(int matchId)
        {
            _logger.LogDebug("{0}: Start Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            Api.ApiMatch.StartMatchtime(matchId);

            _logger.LogDebug("{0}: Started Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkResult(); ;
        }


        /// <summary>
        /// Stops the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchId}/time/stop")]
        public IActionResult SetMatchtimeStop(int matchId)
        {
            _logger.LogDebug("{0}: Stop Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            Api.ApiMatch.PauseMatchtime(matchId);

            _logger.LogDebug("{0}: Stopped Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);
            return new OkResult(); ;
        }

        /// <summary>
        /// Stops the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{matchid}/goal/{teamid}/{amount}")]
        public async Task<IActionResult> SetMatchGoal(int matchid, int teamid, int amount)
        {
            _logger.LogDebug("{0}: Setting Goal in Match {1} for team {2} by {3}", Request.HttpContext.Connection.RemoteIpAddress, matchid, teamid, amount);

            await Api.ApiMatch.SetMatchGoal(matchid, teamid, amount);

            _logger.LogDebug("{0}: Set Goal in Match {1} for team {2} by {3}", Request.HttpContext.Connection.RemoteIpAddress, matchid, teamid, amount);
            return new OkResult(); ;
        }



        /// <summary>
        /// Add a new Match
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> NewMatch(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoMatch match
            )
        {
            _logger.LogDebug("{0}: New Match", Request.HttpContext.Connection.RemoteIpAddress);

            await Api.ApiMatch.NewMatch(match);

            _logger.LogDebug("{0}: New Match", Request.HttpContext.Connection.RemoteIpAddress);
            return new OkResult(); ;
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
            _logger.LogDebug("{0}: Set MatchTime {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, timeleft);

            await Api.ApiMatch.SetMatchTime(matchId, timeleft);
            
            _logger.LogDebug("{0}: Set MatchTime ID {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, matchId, timeleft);
            return new OkResult(); 
        }


        /// <summary>
        /// Gets all live Matches
        /// </summary>
        /// <returns></returns>
        [HttpGet("live")]
        public IActionResult GetLiveMatchList()
        {
            _logger.LogDebug("{0}: Get Livematchlist", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiMatch.GetLiveMatchList();
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Livematchlist JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }
    }
}
