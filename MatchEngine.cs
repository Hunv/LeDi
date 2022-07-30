using Tiwaz.Server.Classes;
using Tiwaz.Server.DatabaseModel;

namespace Tiwaz.Server
{
    public static class MatchEngine
    {
        public static List<MatchHandler> OngoingMatches { get; private set; } = new List<MatchHandler>();

        public static void AddOngoingMatch(MatchHandler matchHandler)
        {
            matchHandler.DisposeMatchHandler += MatchHandler_DisposeMatchHandler;
            OngoingMatches.Add(matchHandler);
        }

        private static void MatchHandler_DisposeMatchHandler(object sender, EventArgs e)
        {
            OngoingMatches.Remove((MatchHandler)sender);
        }
    }
}
