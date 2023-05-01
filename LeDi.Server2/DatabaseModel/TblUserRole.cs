using System.ComponentModel.DataAnnotations;

namespace LeDi.Server2.DatabaseModel
{
    public class TblUserRole
    {
        /// <summary>
        /// The ID of the Role
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of the Role
        /// </summary>
        public string RoleName { get; set; } = "Guest";

        /// <summary>
        /// Is the role an admin role?
        /// </summary>
        public bool IsAdmin { get; set; }


        /// <summary>
        /// Can add a new match (non-tournament)
        /// </summary>
        public bool CanMatchAdd { get; set; }

        /// <summary>
        /// Can delete a match (non-tournament)
        /// </summary>
        public bool CanMatchDelete { get; set; }

        /// <summary>
        /// Can edit a match
        /// </summary>
        public bool CanMatchEdit { get; set; }

        /// <summary>
        /// Can start the time of a match
        /// </summary>
        public bool CanMatchStart { get; set; }

        /// <summary>
        /// Can stop the time of a match
        /// </summary>
        public bool CanMatchStop { get; set; }

        /// <summary>
        /// Can Finish or Cancel a Match
        /// </summary>
        public bool CanMatchEnd { get; set; }

        /// <summary>
        /// Can give penalties in a match
        /// </summary>
        public bool CanMatchPenalty { get; set; }

        /// <summary>
        /// Has access to the advanced controls of a match
        /// </summary>
        public bool CanMatchAdvancedControls { get; set; }


        /// <summary>
        /// Can add a new tournament
        /// </summary>
        public bool CanTournamentAdd { get; set; }

        /// <summary>
        /// Can edit tournaments
        /// </summary>
        public bool CanTournamentEdit { get; set; }

        /// <summary>
        /// can add matches to a tournament
        /// </summary>
        public bool CanTournamentMatchAdd { get; set; }


        /// <summary>
        /// can delete matches to a tournament
        /// </summary>
        public bool CanTournamentMatchDelete{ get; set; }

        /// <summary>
        /// can edit a match if a tournament
        /// </summary>
        public bool CanTournamentMatchEdit { get;set; }


        /// <summary>
        /// Can the role add new users
        /// </summary>
        public bool CanUserAdd { get; set; }

        /// <summary>
        /// can the role edit users
        /// </summary>
        public bool CanUserEdit { get; set; }

        /// <summary>
        /// Can the role delete users
        /// </summary>
        public bool CanUserDelete { get; set;}

        /// <summary>
        /// Can the role reset passwords
        /// </summary>
        public bool CanUserPasswordEdit{ get; set; }


        /// <summary>
        /// Can the role add new players
        /// </summary>
        public bool CanPlayerAdd { get; set; }

        /// <summary>
        /// can the role edit players
        /// </summary>
        public bool CanPlayerEdit { get; set; }

        /// <summary>
        /// Can the role delete players
        /// </summary>
        public bool CanPlayerDelete { get; set; }


        /// <summary>
        /// Can the role add new teams
        /// </summary>
        public bool CanTeamAdd { get; set; }

        /// <summary>
        /// can the role edit teams
        /// </summary>
        public bool CanTeamEdit { get; set; }

        /// <summary>
        /// Can the role delete teams
        /// </summary>
        public bool CanTeamDelete { get; set; }


        /// <summary>
        /// Can add, edit or delete the game rules templates
        /// </summary>
        public bool CanTemplateManage { get; set; }


        /// <summary>
        /// Can add, edit or delete settings
        /// </summary>
        public bool CanSettingManage { get; set; }


        /// <summary>
        /// Can add, edit or delete device settings
        /// </summary>
        public bool CanDeviceManage { get; set; }

        /// <summary>
        /// Can send device commands
        /// </summary>
        public bool CanDeviceCommands { get; set; }
    }
}
