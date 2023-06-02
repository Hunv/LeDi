using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared2.DatabaseModel
{
    public class TblGameRulePenalty
    {
        /// <summary>
        /// Creates a new instance of Setting
        /// </summary>
        public TblGameRulePenalty()
        {
        }

        /// <summary>
        /// Unique ID of the penalty
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// Name of the penalty
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Duration of the penalty (if applies)
        /// </summary>
        public int PenaltySeconds { get; set; }

        /// <summary>
        /// Is this penalty a total dismissal and a player or team is disqualified because of this penalty?
        /// </summary>
        public bool TotalDismissal { get; set; }

        /// <summary>
        /// List of names of the penalties in different language (if provided)
        /// </summary>
        public List<TblGameRulePenaltyDisplay> Display { get; set; } = new List<TblGameRulePenaltyDisplay>();

    }
}
