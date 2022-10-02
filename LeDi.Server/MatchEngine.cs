using LeDi.Server.Classes;
using LeDi.Server.DatabaseModel;

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
    }
}
