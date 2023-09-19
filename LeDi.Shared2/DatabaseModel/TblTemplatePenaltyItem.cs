﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LeDi.Shared2.DatabaseModel
{
    public class TblTemplatePenaltyItem
    {
        /// <summary>
        /// Creates a new instance of Setting
        /// </summary>
        public TblTemplatePenaltyItem()
        {
        }

        /// <summary>
        /// Unique ID of the penalty
        /// </summary>
        [Key]
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }


        /// <summary>
        /// Name of the penalty
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Official Rule number of this penalty
        /// </summary>
        public string RuleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Any kind of note for that rule. Not visible to any users, just for administrative user internal notes
        /// </summary>
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// Duration of the penalty (if applies). Just the default value if this penalty is selected. Can be changed when used.
        /// </summary>
        public int PenaltySeconds { get; set; }

        /// <summary>
        /// Is this penalty a total dismissal and a player or team is disqualified because of this penalty?
        /// </summary>
        public bool TotalDismissal { get; set; }

        /// <summary>
        /// List of names of the penalties in different language (if provided)
        /// </summary>
        public List<TblTemplatePenaltyText> Display { get; set; } = new List<TblTemplatePenaltyText>();

        /// <summary>
        /// The template this penalty template item belongs to
        /// </summary>
        public TblTemplate Template { get; set; }

    }
}