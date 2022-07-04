using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Tiwaz.Server.Classes;
using Tiwaz.Server.DatabaseModel;

namespace Tiwaz.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly ILogger<SettingController> _logger;

        public MatchController(ILogger<SettingController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets all matches
        /// </summary>
        /// <returns>A list of all matches</returns>
        /// <response code="200">Success</response>
        [HttpGet(Name = "GetMatches")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Match> Get()
        {
            _logger.LogDebug("{0}: Get Matches", Request.HttpContext.Connection.RemoteIpAddress);

            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Matches;

                return dto.ToArray();
            }
        }

        /// <summary>
        /// Gets match
        /// </summary>
        /// <returns>A match</returns>
        /// <response code="200">Success</response>
        [HttpGet("{matchId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Match? GetMatch(int matchId)
        {
            _logger.LogDebug("{0}: Get Match ID {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);

                return dto;
            }
        }

        /// <summary>
        /// Creates a new match
        /// </summary>
        /// <param name="team1name">The name of the first team</param>
        /// <param name="team2name">The name of the second team</param>
        /// <param name="halftimeDuration">Duration of a halftime in seconds</param>
        /// <param name="halftimeCount">Number of halftimes</param>
        /// <param name="scheduledStartTime">The expected time of match start</param>
        /// <param name="gameName">The name of the game, that this match is (i.e. soccer, underwaterhockey, ... </param>
        /// <returns>The newly created match including the match ID</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Success</response>
        [HttpPut(Name = "AddMatch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Put(
            [FromQuery] string team1name,
            [FromQuery] string team2name,
            [FromQuery] int halftimeLength,
            [FromQuery] int halftimeCount,
            [FromQuery] DateTime scheduledStartTime,
            [FromQuery] string gameName
            )
        {
            _logger.LogDebug("{0}: Add Match. Team1: {1}; Team2: {2}; halftimeDuration: {3}; halftimeCount: {4}; scheduledStartTime: {5}; gameName: {6}",
                Request.HttpContext.Connection.RemoteIpAddress, team1name, team2name, halftimeLength, halftimeCount, scheduledStartTime, gameName);

            using (var dbContext = new TwDbContext())
            {
                var match = new Match(team1name, team2name);
                match.CurrentHalftime = 0;
                match.HalftimeLength = halftimeLength;
                match.HalftimeCount = halftimeCount;
                match.GameName = gameName;
                match.ScheduledTime = scheduledStartTime;
                dbContext.Matches.Add(match);
                await dbContext.SaveChangesAsync();

                _logger.LogDebug("Added match {0}", match.Id);

                return new OkObjectResult(match);
            }
        }

        /// <summary>
        /// Updates an existing match
        /// </summary>
        /// <param name="matchId">The Id of the Match</param>
        /// <param name="team1name">The name of the first team</param>
        /// <param name="team2name">The name of the second team</param>
        /// <param name="halftimeDuration">Duration of a halftime in seconds</param>
        /// <param name="halftimeCount">Number of halftimes</param>
        /// <param name="scheduledStartTime">The expected time of match start</param>
        /// <returns>The updated match</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Cannot find the stated match ID</response>
        [HttpPost(Name = "UpdateMatch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post(
            [FromQuery] int matchId,
            [FromQuery] string? team1name,
            [FromQuery] string? team2name,
            [FromQuery] int? halftimeLength,
            [FromQuery] int? halftimeCount,
            [FromQuery] DateTime? scheduledStartTime
            )
        {
            _logger.LogDebug("{0}: Update Match {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            using (var dbContext = new TwDbContext())
            {
                var matchItem = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (matchItem == null)
                {
                    _logger.LogError("The match ID {0} does not exist.", matchId);
                    return new NotFoundResult();
                }

                if (team1name != null)
                    matchItem.Team1Name = team1name;

                if (team2name != null)
                    matchItem.Team2Name = team2name;

                if (halftimeLength != null)
                    matchItem.HalftimeLength = halftimeLength.Value;

                if (halftimeCount != null)
                    matchItem.HalftimeCount = halftimeCount.Value;

                if (scheduledStartTime != null)
                    matchItem.ScheduledTime = scheduledStartTime;

                await dbContext.SaveChangesAsync();
                return new OkObjectResult(matchItem);
            }
        }


        /// <summary>
        /// Updates an existing Match
        /// </summary>
        /// <param name="matchId">The ID of the match to delete</param>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Cannot find the match ID</response>
        [HttpDelete(Name = "DeleteMatch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromQuery] int matchId
            )
        {
            _logger.LogDebug("{0}: Delete Match {1}", Request.HttpContext.Connection.RemoteIpAddress, matchId);

            using (var dbContext = new TwDbContext())
            {
                var matchItem = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (matchItem == null)
                {
                    _logger.LogError("The match ID {0} does not exist.", matchId);
                    return new NotFoundResult();
                }

                dbContext.Matches.Remove(matchItem);
                await dbContext.SaveChangesAsync();
                return new OkResult();
            }
        }


        /// <summary>
        /// Receives a new Match Event
        /// </summary>
        /// <param name="matchId">The ID of the match to assign the Event to</param>
        /// <param name="eventId">The EventName for the match</param>
        /// <param name="eventComment">A not for the Event</param>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Cannot find the match ID or Eventname</response>
        [HttpPost("{matchId}/{eventId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RunEvent(
            int matchId,
            int eventId,
            [FromQuery] string? eventComment
            )
        {

            _logger.LogDebug("{0}: Run Event {1} on Match {2}", Request.HttpContext.Connection.RemoteIpAddress, eventId, matchId);

            using (var dbContext = new TwDbContext())
            {
                var matchItem = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);


                //for testing
                MatchEngine.LoadMatch(matchItem);

                if (matchItem == null)
                {
                    _logger.LogError("The match ID {0} does not exist.", matchId);
                    return new NotFoundResult();
                }
                try
                {
                    var eventItem = (MatchEventEnum) eventId;
                }
                catch(Exception)
                {
                    _logger.LogError("Cannot assign eventId {0} to an event", eventId);
                    return new NotFoundResult();
                }

                //Add the event to the MatchEngine. Also adds it to the matchItem variable. So just a save is required after this.
                MatchEventItem evItem = new MatchEventItem();
                evItem.TimeOfEvent = DateTime.Now;
                evItem.EventName = (MatchEventEnum)eventId;
                evItem.EventComment = eventComment;
                MatchEngine.MatchEvent(evItem);

                //MatchEvent mEvent = new MatchEvent();
                //mEvent.Event = (MatchEventEnum)eventId;
                //mEvent.Text = eventComment;
                //mEvent.Timestamp = DateTime.Now;

                //matchItem.MatchEvents.Add(mEvent);

                await dbContext.SaveChangesAsync();
                return new OkResult();
            }
        }
    }
}