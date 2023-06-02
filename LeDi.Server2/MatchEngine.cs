using LeDi.Server2.DatabaseModel;
using LeDi.Shared2.Enum;

namespace LeDi.Server2
{
    public class MatchEngine
    {
        public List<MatchHandler> OngoingMatches { get; private set; } = new List<MatchHandler>();

        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public bool IsInitialized { get; private set; }

        public MatchEngine() 
        {
            // Loads the currently ongoing (not finished) matches from database
            LoadRunningMatches();
        }

        public void AddOngoingMatch(MatchHandler matchHandler)
        {
            Logger.Trace("Adding match {0} to cached ongoing matches.", matchHandler.MatchId);

            matchHandler.DisposeMatchHandler += MatchHandler_DisposeMatchHandler;
            OngoingMatches.Add(matchHandler);
        }

        private void MatchHandler_DisposeMatchHandler(object? sender, EventArgs e)
        {
            if (sender != null)
                OngoingMatches.Remove((MatchHandler)sender);
        }

        /// <summary>
        /// Loads not ended matches. Used to be executed on startup
        /// </summary>
        public void LoadRunningMatches()
        {
            Logger.Debug("Caching unfinished matches...");
            try
            {
                // Create the match event
                using var dbContext = new LeDiDbContext();

                if (dbContext.TblMatches != null)
                {
                    foreach (var aMatch in dbContext.TblMatches)
                    {
                        if (aMatch.MatchStatus == (int)MatchStatusEnum.Canceled ||
                            aMatch.MatchStatus == (int)MatchStatusEnum.Closed ||
                            aMatch.MatchStatus == (int)MatchStatusEnum.Ended)
                            continue;

                        //If match not already loaded, create a new one
                        if (!OngoingMatches.Any(x => x.MatchId == aMatch.Id))
                        {
                            AddOngoingMatch(new MatchHandler(aMatch.Id, false));
                        }
                    }
                }
                IsInitialized = true;
                Logger.Trace("Cached unfinished matches.");
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Failed to cache unfinished matches.");
            }
        }
    }
}
