namespace LeDi.Shared.Enum
{
    public enum MatchEventEnum
    {        
        Undefined = 0,

        //Range 1-99 = Time related events like Start/Stop of time
        MatchStart = 1,
        MatchPaused = 2,
        MatchResumed = 3,
        MatchCancel = 4,
        MatchEnd = 5,
        MatchOvertimeStart = 6,
        MatchExtentionStart = 7,
        HalftimeEnd = 10,
        HalftimeStart = 11,
        TimeoutTeam1 = 20,
        TimeoutTeam2 = 21,
        TimeoutReferee = 22,

        //Range 100-199 General Match Events
        ScoreTeam1 = 100,
        ScoreTeam2 = 101,
        ScoreRevokeTeam1 = 102,
        ScoreRevokeTeam2 = 103,
        MatchWinTeam1 = 110,
        MatchWinTeam2 = 111,        

        //Range 200-299 Fouls
        FoulTeam1 = 200,
        FoulTeam2 = 201,
        PenaltyTeam1 = 210,
        PenaltyTeam2 = 211,
        PenaltyTeam1Revoke = 220,
        PenaltyTeam2Revoke = 221,

        //Range 300-399 Postmatch handling
        MatchFinished = 300
    }
}
