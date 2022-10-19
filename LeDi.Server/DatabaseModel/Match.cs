using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LeDi.Shared.DtoModel;
using LeDi.Server.Classes;

namespace LeDi.Server.DatabaseModel
{
    public class Match : MatchRuleSet
    {

        /// <summary>
        /// The ID of the match
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The List of the Players for Team 1
        /// </summary>
        public List<Player>? Team1Players { get; set; }

        /// <summary>
        /// The Ids of the Players for Team 1
        /// </summary>
        [NotMapped]
        public List<int> Team1PlayerIds { get { return Team1Players != null ? Team1Players.Select(x => x.Id).ToList() : new List<int>(); } }

        /// <summary>
        /// The List of the Players for Team 2
        /// </summary>
        public List<Player>? Team2Players { get; set; }

        /// <summary>
        /// The Ids of the Players for Team 2
        /// </summary>
        [NotMapped]
        public List<int> Team2PlayerIds { get { return Team2Players != null ? Team2Players.Select(x => x.Id).ToList() : new List<int>(); } }

        /// <summary>
        /// Score of Team1
        /// </summary>
        [Required]
        public int Team1Score { get; set; }

        /// <summary>
        /// Score of Team2
        /// </summary>
        [Required]
        public int Team2Score { get; set; }

        /// <summary>
        /// Name of Team1
        /// </summary>
        [Required]
        public string? Team1Name { get; set; }

        /// <summary>
        /// Name of Team2
        /// </summary>
        [Required]
        public string? Team2Name { get; set; }

        /// <summary>
        /// Time left in seconds
        /// </summary>
        [Required]
        public int CurrentTimeLeft { get; set; }

        /// <summary>
        /// The current period; 0 = match not started
        /// </summary>
        [Required]
        public int CurrentPeriod { get; set; }

        /// <summary>
        /// The Status of the Match
        /// </summary>
        public int MatchStatus { get; set; }

        /// <summary>
        /// The Scheduled time when the match should start
        /// </summary>
        public DateTime? ScheduledTime { get; set; }

        /// <summary>
        /// The timestamps of matchevents
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<MatchEvent> MatchEvents { get; set; } = new List<MatchEvent>();

        /// <summary>
        /// The referees of the event
        /// </summary>
        public List<MatchReferee> MatchReferees { get; set; } = new List<MatchReferee>();

        /// <summary>
        /// The Penalties that were raised in this match
        /// </summary>
        public List<MatchPenalty> MatchPenalties { get; set; } = new List<MatchPenalty>();

        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoMatch ToDto()
        {
            var dto = new DtoMatch()
            {
                Id = Id,
                Team1Score = Team1Score,
                Team2Score = Team2Score,
                Team1Name = Team1Name,
                Team2Name = Team2Name,
                TimeLeftSeconds = CurrentTimeLeft,
                PeriodCurrent = CurrentPeriod,
                MatchStatus = MatchStatus,
                ScheduledTime = ScheduledTime,
                Team1PlayerIds = Team1PlayerIds,
                Team2PlayerIds = Team2PlayerIds,

                RulePeriodCount = RulePeriodCount,
                RulePeriodLength = RulePeriodLength,
                RulePeriodLastPauseTimeOnEvent = RulePeriodLastPauseTimeOnEvent,
                RulePeriodLastPauseTimeOnEventSeconds = RulePeriodLastPauseTimeOnEventSeconds,
                RulePeriodOvertime = RulePeriodOvertime,
                RuleMatchExtensionOnDraw = RuleMatchExtensionOnDraw
                
            };

            if (MatchReferees != null)
            {
                dto.Referees = new List<DtoMatchReferee>();
                foreach (var aRef in MatchReferees)
                {
                    dto.Referees.Add(aRef.ToDto());
                }
            }

            // Set Rule Penalties
            if (RulePenaltyList != null && RulePenaltyList.Count > 0)
            {
                dto.RulePenaltyList = new List<DtoRulePenalty>();
                foreach (var aPenalty in RulePenaltyList)
                {
                    var dtoRs = new DtoRulePenalty()
                    {                        
                        Name = aPenalty.Name,
                        PenaltySeconds = aPenalty.PenaltySeconds,
                        TotalDismissal = aPenalty.TotalDismissal
                    };
                    
                    if (aPenalty.Display != null && aPenalty.Display.Count > 0)
                    {
                        dtoRs.Display = new List<DtoDisplayText>();
                        foreach(var aDisp in aPenalty.Display)
                        {
                            var dtoDisp = new DtoDisplayText()
                            {
                                Language = aDisp.Language,
                                Text = aDisp.Text
                            };
                            
                            dtoRs.Display.Add(dtoDisp);
                        }
                    }

                    dto.RulePenaltyList.Add(dtoRs);
                }
            }


            // Set Penalties
            if (MatchPenalties != null && MatchPenalties.Count > 0)
            {
                dto.Penalties = new List<DtoMatchPenalty>();
                foreach(var aMatchPenalty in MatchPenalties)
                {
                    var newDtoMp = aMatchPenalty.ToDto();
                    dto.Penalties.Add(newDtoMp);
                }
            }

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoMatch dto)
        {
            Id = dto.Id;

            if (dto.Team1Score.HasValue)
                Team1Score = dto.Team1Score.Value;

            if (dto.Team2Score.HasValue)
                Team2Score = dto.Team2Score.Value;

            Team1Name = dto.Team1Name;
            
            Team2Name = dto.Team2Name;
            
            if (dto.TimeLeftSeconds.HasValue)
                CurrentTimeLeft = dto.TimeLeftSeconds.Value;
            
            if (dto.PeriodCurrent.HasValue)
                CurrentPeriod = dto.PeriodCurrent.Value;
            
            if (dto.MatchStatus.HasValue)
                MatchStatus = dto.MatchStatus.Value;
            
            ScheduledTime = dto.ScheduledTime;
            
            GameName = dto.Gamename;
            
            if (dto.RulePeriodLength.HasValue) 
                RulePeriodLength = dto.RulePeriodLength.Value;            
            
            if (dto.RulePeriodCount.HasValue) 
                RulePeriodCount = dto.RulePeriodCount.Value;
            
            RulePeriodLastPauseTimeOnEvent = dto.RulePeriodLastPauseTimeOnEvent;
            
            if (dto.RulePeriodLastPauseTimeOnEventSeconds.HasValue)
                RulePeriodLastPauseTimeOnEventSeconds = dto.RulePeriodLastPauseTimeOnEventSeconds.Value;
            
            RulePeriodOvertime = dto.RulePeriodOvertime;
            
            RuleMatchExtensionOnDraw = dto.RuleMatchExtensionOnDraw;


            // Get team 1 players from database by ID
            if (dto.Team1PlayerIds != null && dto.Team1PlayerIds.Count > 0)
            {
                using var dbContext = new TwDbContext();

                foreach (var aPlayerId in dto.Team1PlayerIds)
                {
                    if (dbContext.Players != null)
                    {
                        var player = dbContext.Players.SingleOrDefault(x => x.Id == aPlayerId);
                        if (player != null)
                        {
                            if (Team1Players == null)
                                Team1Players = new List<Player>();

                            Team1Players.Add(player);
                        }
                    }
                }
            }

            // Get Team 2 players from database by ID
            if (dto.Team2PlayerIds != null && dto.Team2PlayerIds.Count > 0)
            {
                using var dbContext = new TwDbContext();

                foreach (var aPlayerId in dto.Team2PlayerIds)
                {
                    if (dbContext.Players != null)
                    {
                        var player = dbContext.Players.SingleOrDefault(x => x.Id == aPlayerId);
                        if (player != null)
                        {
                            if (Team2Players == null)
                                Team2Players = new List<Player>();

                            Team2Players.Add(player);
                        }
                    }
                }
            }

            // Get referees
            if (dto.Referees != null && dto.Referees.Count > 0)
            {
                foreach (var aRef in dto.Referees)
                {
                    if (MatchReferees == null)
                        MatchReferees = new List<MatchReferee>();

                    MatchReferees.Add(new MatchReferee
                    {
                        Name = aRef.Name,
                        Clubname = aRef.Clubname,
                        Role = aRef.Role
                    });
                }
            }

            // Get Rule Penalties
            if (dto.RulePenaltyList != null && dto.RulePenaltyList.Count > 0)
            {
                RulePenaltyList = new List<MatchRuleSetPenalty>();
                foreach(var aPenalty in dto.RulePenaltyList)
                {
                    var dbRs = new MatchRuleSetPenalty();
                    dbRs.Name = aPenalty.Name;
                    dbRs.TotalDismissal = aPenalty.TotalDismissal;
                    dbRs.PenaltySeconds = aPenalty.PenaltySeconds;
                    
                    dbRs.Display = new List<MatchRuleSetDisplayText>();
                    foreach (var aDisplay in aPenalty.Display)
                    {
                        var dbDisp = new MatchRuleSetDisplayText();
                        dbDisp.Text = aDisplay.Text;
                        dbDisp.Language = aDisplay.Language;
                        dbRs.Display.Add(dbDisp);
                    }

                    RulePenaltyList.Add(dbRs);
                }
            }


            // Get Penalties
            if (dto.Penalties != null && dto.Penalties.Count > 0)
            {
                MatchPenalties = new List<MatchPenalty>();
                foreach (var aPenalty in dto.Penalties)
                {
                    var dbPen = new MatchPenalty();
                    dbPen.Id = aPenalty.Id;
                    dbPen.PlayerId = aPenalty.PlayerId;
                    dbPen.PlayerName = aPenalty.PlayerName;
                    dbPen.PlayerNumber = aPenalty.PlayerNumber;
                    dbPen.Note = aPenalty.Note;
                    dbPen.Source = aPenalty.Source;
                    dbPen.PenaltyTime = aPenalty.PenaltyTime;
                    dbPen.PenaltyTimeStart = aPenalty.PenaltyTimeStart;
                    dbPen.Timestamp = aPenalty.Timestamp;
                    dbPen.TeamId = aPenalty.TeamId;
                    dbPen.PenaltyName = aPenalty.PenaltyName;
                                        
                    MatchPenalties.Add(dbPen);
                }
            }
        }
    }
}
