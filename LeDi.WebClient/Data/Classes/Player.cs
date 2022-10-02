using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeDi.WebClient.Data.Classes
{
    public class Player
    {
        public Player() { }

        /// <summary>
        /// The ID of the Player
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The First Name of the player
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// The Last Name of the player
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// The (current) number of the player
        /// </summary>
        public int PlayerNumber { get; set; }

        /// <summary>
        /// The List of match IDs that player participated at
        /// </summary>
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        public int[]? MatchIdList { get; set; }

    }
}
