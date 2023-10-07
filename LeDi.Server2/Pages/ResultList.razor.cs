using LeDi.Shared2.DatabaseModel;

namespace LeDi.Server2.Pages
{
    public partial class ResultList
    {
        public List<TblMatch>? MatchList;
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected override async Task OnInitializedAsync()
        {
            var eventTournamentId = "";
            var evId = await DataHandler.GetSettingAsync("eventtournamentid");
            if (evId != null)
            {
                eventTournamentId = evId.SettingValue;
            }

            // Get single matches
            if (string.IsNullOrWhiteSpace(eventTournamentId))
            {
                var matches = DataHandler.GetMatchList();
                if (matches != null)
                {
                    MatchList = matches.Where(x => x.ScheduledTime >= DateTime.UtcNow.AddDays(-5)).OrderByDescending(x => x.ScheduledTime).ToList();
                }
            }
            else // Get matches of a tournament
            {
                var matches = DataHandler.GetMatchList();
                if (matches != null)
                {
                    MatchList = matches.Where(x => x.Tournament != null && x.Tournament.Id.ToString() == eventTournamentId).ToList();
                }
            }
        }
    }
}
