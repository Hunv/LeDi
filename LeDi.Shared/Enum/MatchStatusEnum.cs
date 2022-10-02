namespace LeDi.Shared.Enum
{
    public enum MatchStatusEnum
    {
        Undefined = 0,
        Planned = 10, // Match is created and saved
        ReadyToStart = 20, // Match is marked as ready to start
        Running = 30, // Match is running
        Ended = 40, // Match ended (time over)
        Closed = 100, // Match is closed (debriefing done)
        Canceled = 200, // Match was canceled before it started
        Stopped = 210 // Match was stopped after it started (i.e. on serious injuries)
    }
}
