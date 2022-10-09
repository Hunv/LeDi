using LeDi.Server.Classes;
using LeDi.Server.DatabaseModel;
using LeDi.Shared.Enum;

namespace LeDi.Server
{
    public static class MatchEngine
    {
        public static List<MatchHandler> OngoingMatches { get; private set; } = new List<MatchHandler>();

        public static void AddOngoingMatch(MatchHandler matchHandler)
        {
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
            // Create the match event
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                foreach(var aMatch in dbContext.Matches)
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

        }
    }
}
