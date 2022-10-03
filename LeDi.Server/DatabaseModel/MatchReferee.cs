using LeDi.Shared.DtoModel;

namespace LeDi.Server.DatabaseModel
{
    public class MatchReferee
    {
        /// <summary>
        /// Internal ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The first and lastname of the referee
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The Clubname of the Club of the referee
        /// </summary>
        public string? Clubname { get; set; }

        /// <summary>
        /// The Role of the referee in that match
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// The match this referee belongs to
        /// </summary>
        public Match? Match { get; set; }



        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoMatchReferee ToDto()
        {
            var dto = new DtoMatchReferee()
            {
                Name = Name ?? "",
                Clubname = Clubname ?? "",
                Role = Role ?? ""
            };

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoMatchReferee dto)
        {
            Name = dto.Name;
            Clubname = dto.Clubname;
            Role = dto.Role;
        }
    }
}
