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
        [HttpGet("{id}")]
        public IActionResult GetMatch(int id)
        {
            _logger.LogDebug("{0}: Get Match {1}", Request.HttpContext.Connection.RemoteIpAddress, id);

            var json = Api.ApiMatch.GetMatch(id);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Match ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, id, json);
            return result;
        }

        /// <summary>
        /// Gets the remaining seconds only
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/time")]
        public IActionResult GetMatchTime(int id)
        {
            _logger.LogDebug("{0}: Get MatchTime {1}", Request.HttpContext.Connection.RemoteIpAddress, id);

            var json = Api.ApiMatch.GetMatchTime(id);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got MatchTime ID {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, id, json);
            return result;
        }

        /// <summary>
        /// Modify an existing Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> SetMatch(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoMatch match
            )
        {
            _logger.LogDebug("{0}: Set Match {1}", Request.HttpContext.Connection.RemoteIpAddress, match.Id);

            await Api.ApiMatch.SetMatch(match);
            
            _logger.LogDebug("{0}: Set Match ID {1}", Request.HttpContext.Connection.RemoteIpAddress, match.Id);
            return new OkResult(); ;
        }

        /// <summary>
        /// Start the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}/time/start")]
        public IActionResult SetMatchtimeStart(int id)
        {
            _logger.LogDebug("{0}: Start Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, id);

            Api.ApiMatch.StartMatchtime(id);

            _logger.LogDebug("{0}: Started Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, id);
            return new OkResult(); ;
        }


        /// <summary>
        /// Stops the counter of a Match
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}/time/stop")]
        public IActionResult SetMatchtimeStop(int id)
        {
            _logger.LogDebug("{0}: Stop Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, id);

            Api.ApiMatch.PauseMatchtime(id);

            _logger.LogDebug("{0}: Stopped Matchtime {1}", Request.HttpContext.Connection.RemoteIpAddress, id);
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
        [HttpPut("{id}/time")]
        public async Task<IActionResult> SetMatchTime(
            int id, 
            [FromQuery]int timeleft
            )
        {
            _logger.LogDebug("{0}: Set MatchTime {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, id, timeleft);

            await Api.ApiMatch.SetMatchTime(id, timeleft);
            
            _logger.LogDebug("{0}: Set MatchTime ID {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, id, timeleft);
            return new OkResult(); 
        }

    }
}
