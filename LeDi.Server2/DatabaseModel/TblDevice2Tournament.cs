namespace LeDi.Server2.DatabaseModel
{
    public class TblDevice2Tournament
    {
        public int DeviceId { get; set; }
        public TblDevice? Device { get; set; }

        public int TournamentId { get; set; }
        public TblTournament? Match { get; set; }
    }
}
