namespace LeDi.Server
{
    public static class SystemSettings
    {
        public static int MatchHandlerRefreshTime { get; set; } = 500;
        public static int MatchHandlerDisposeTime { get; set; } = 600000;
        public static string RuleFilePath { get; set; } = "gamerules.json";
    }
}
