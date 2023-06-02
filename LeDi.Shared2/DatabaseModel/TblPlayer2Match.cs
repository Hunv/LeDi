namespace LeDi.Shared2.DatabaseModel
{
    public class TblPlayer2Match
    {
        public int PlayerId { get; set; }
        public TblPlayer? Player { get; set; }

        public int MatchId { get; set; }
        public TblMatch? Match { get; set; }
    }
}
