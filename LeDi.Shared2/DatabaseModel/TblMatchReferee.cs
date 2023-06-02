namespace LeDi.Shared2.DatabaseModel
{
    public class TblMatchReferee
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
        public TblMatch? Match { get; set; }
    }
}
