using LeDi.Shared2.DatabaseModel;

namespace LeDi.Server2.Pages
{
    public partial class ResultList
    {
        public List<TblMatch>? MatchList;
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void OnInitialized()
        {
            var matches = DataHandler.GetMatchList();
            if (matches != null) 
            {
                MatchList = matches.Where(x => x.ScheduledTime >= DateTime.UtcNow.AddDays(-5)).OrderByDescending(x => x.ScheduledTime).ToList();
            }
        }
    }
}
