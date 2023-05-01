using System.ComponentModel.DataAnnotations;

namespace LeDi.Server2.DatabaseModel
{
    public class TblUser
    {
        /// <summary>
        /// The ID of the User
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Login Name of the user
        /// </summary>
        [Required]
        public string? LoginName { get; set; }

        /// <summary>
        /// The Displayname of the user
        /// </summary>
        [Required]
        public string? DisplayName { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        [Required]
        public string? Password { get; set; }

        /// <summary>
        /// Is the user enabled?
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The Role of the user
        /// </summary>
        public TblUserRole? Role { get; set; }
    }
}
