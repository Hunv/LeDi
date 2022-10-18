using LeDi.Server.Classes;
using LeDi.Server.DatabaseModel;
using LeDi.Shared.Enum;

namespace LeDi.Server
{
    public static class MatchEngine
    {
        public static List<MatchHandler> OngoingMatches { get; private set; } = new List<MatchHandler>();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void AddOngoingMatch(MatchHandler matchHandler)
        {
            Logger.Trace("Adding match {0} to cached ongoing matches.", matchHandler.MatchId);

            matchHandler.DisposeMatchHandler += MatchHandler_DisposeMatchHandler;
            OngoingMatches.Add(matchHandler);
        }

        private static void MatchHandler_DisposeMatchHandler(object? sender, EventArgs e)
        {
            if (sender != null)
                OngoingMatches.Remove((MatchHandler)sender);
        }

        /// <summary>
        /// Loads not ended matches. Used to be executed on startup
        /// </summary>
        public static void LoadRunningMatches()
        {
            Logger.Debug("Caching unfinished matches...");
            try
            {
                // Create the match event
                using var dbContext = new TwDbContext();

                if (dbContext.Matches != null)
                {
                    foreach (var aMatch in dbContext.Matches)
                    {
                        if (aMatch.MatchStatus == (int)MatchStatusEnum.Canceled ||
                            aMatch.MatchStatus == (int)MatchStatusEnum.Closed ||
                            aMatch.MatchStatus == (int)MatchStatusEnum.Ended)
                            continue;

                        //If match not already loaded, create a new one
                        if (!MatchEngine.OngoingMatches.Any(x => x.MatchId == aMatch.Id))
                        {
                            MatchEngine.AddOngoingMatch(new MatchHandler(aMatch.Id, false));
                        }
                    }
                }
                Logger.Trace("Cached unfinished matches.");
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Failed to cache unfinished matches.");
            }
        }
    }
}
