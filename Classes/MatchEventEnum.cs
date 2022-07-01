namespace Tiwaz.Server.Classes
{
    public enum MatchEventEnum
    {        
        Undefined = 0,
        //Range 1-99 = Time related events like Start/Stop of time
        MatchStart = 1,
        MatchPaused = 2,
        HalftimeEnd = 3,
        HalftimeStart = 4,
        MatchCancel = 5,
        MatchEnd = 6,
        TimeoutTeam1 = 7,
        TimeoutTeam2 = 8,
        TimeoutReferee = 9,

        //Range 100-199 General Match Events
        ScoreTeam1 = 100,
        ScoreTeam2 = 101,
        ScoreRevokeTeam1 = 102,
        ScoreRevokeTeam2 = 103,
        MatchWinTeam1 = 104,
        MatchWinTeam2 = 105,        

        //Range 200-299 Fouls
        FoulTeam1 = 200,
        FoulTeam2 = 201,
        TimePenaltyTeam1 = 202,
        TimePenaltyTeam2 = 203,
    }
}
