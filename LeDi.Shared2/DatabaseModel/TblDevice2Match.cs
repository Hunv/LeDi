namespace LeDi.Shared2.DatabaseModel
{
    public class TblDevice2Match
    {
        public int DeviceId { get; set; }
        public TblDevice? Device { get; set; }

        public int MatchId { get; set; }
        public TblMatch? Match { get; set; }
    }
}
