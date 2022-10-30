namespace LeDi.Server.DatabaseModel
{
    public class Device2Match
    {
        public int DeviceId { get; set; }
        public Device? Device { get; set; }

        public int MatchId { get; set; }
        public Match? Match { get; set; }
    }
}
