﻿using System.ComponentModel.DataAnnotations;
using LeDi.Server.Classes;
using LeDi.Shared.DtoModel;
using LeDi.Shared.Enum;

namespace LeDi.Server.DatabaseModel
{
    public class MatchPenalty
    {
        /// <summary>
        /// The unique Id for the Database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The PlayerId in case the player is maintained in database
        /// </summary>
        public int? PlayerId { get; set; }

        /// <summary>
        /// The Playernumber
        /// </summary>
        public int PlayerNumber { get; set; }

        /// <summary>
        /// The name of the player
        /// </summary>
        public string? PlayerName { get; set; }

        /// <summary>
        /// The TeamID the player is playing for (0 = Team1, 1 = Team2)
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// A note for the penalty
        /// </summary>
        public string Note { get; set; } = "";

        /// <summary>
        /// The source of the penalty
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// The timestamp when the penalty occured
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The point of time the penalty was made. This are the seconds the game already runs.
        /// </summary>
        public int PenaltyTimeStart { get; set; }

        /// <summary>
        /// Number of seconds the penalty lasts. 0 if it has no ending.
        /// </summary>
        public int PenaltyTime { get; set; }

        /// <summary>
        /// The name of the penalty. Internal - always in English.
        /// </summary>
        public string PenaltyName { get; set; } = "";

        /// <summary>
        /// Is the penalty revoked?
        /// </summary>
        public bool Revoked { get; set; }

        /// <summary>
        /// Note why the penalty was revoked
        /// </summary>
        public string? RevokeNote { get; set; }

        /// <summary>
        /// The MatchId this Penalty belongs to
        /// </summary>
        public int MatchId { get; set; }


        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoMatchPenalty ToDto()
        {
            var dto = new DtoMatchPenalty()
            {
                Id = Id,
                PlayerId = PlayerId,
                PlayerNumber = PlayerNumber,
                PlayerName = PlayerName,
                Note = Note,
                Source = Source,
                Timestamp = Timestamp,
                PenaltyTimeStart = PenaltyTimeStart,
                PenaltyTime = PenaltyTime,
                TeamId = TeamId,
                PenaltyName = PenaltyName,
                Revoked = Revoked,
                RevokeNote = RevokeNote
            };

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoMatchPenalty dto)
        {
            Id = dto.Id;
            PlayerId = dto.PlayerId;
            PlayerNumber = dto.PlayerNumber;
            PlayerName = dto.PlayerName;
            Note = dto.Note;
            Source = dto.Source;
            Timestamp = dto.Timestamp;
            PenaltyTimeStart = dto.PenaltyTimeStart;
            PenaltyTime = dto.PenaltyTime;
            TeamId = dto.TeamId;
            PenaltyName = dto.PenaltyName;
            Revoked = dto.Revoked;
            RevokeNote = dto.RevokeNote;
        }
    }
}
