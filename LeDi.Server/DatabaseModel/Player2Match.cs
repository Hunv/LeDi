namespace LeDi.Server.DatabaseModel
{
    public class Player2Match
    {
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public int MatchId { get; set; }
        public Match? Match { get; set; }
    }
}
