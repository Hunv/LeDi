﻿namespace LeDi.Server.Classes
{
    public class MatchRuleSetDisplayText
    {
        /// <summary>
        /// Internal ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Language this Item is dedicated to
        /// </summary>
        public string Language { get; set; } = "EN";

        /// <summary>
        /// The text that should be shown in the language mentioned above
        /// </summary>
        public string Text { get; set; } = "";
    }
}
