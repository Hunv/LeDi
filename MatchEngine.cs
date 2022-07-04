using Tiwaz.Server.Classes;
using Tiwaz.Server.DatabaseModel;

namespace Tiwaz.Server
{
    public static class MatchEngine
    {
        private static System.Timers.Timer tmrTimeEngine = new System.Timers.Timer(1000);
        public static List<MatchEventItem> MatchEventDefinition = new List<MatchEventItem>();
        private static Match? CurrentMatch = null;

        public static void Load()
        {
            // Add details about MatchEvents
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.Undefined });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchStart, StartTime = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchPaused, StopTime = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchResumed, StartTime = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchCancel, EndsMatch = true});
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchEnd, EndsMatch = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchOvertimeStart, StartTime = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchExtentionStart, StartTime = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.HalftimeEnd, StopTime = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.HalftimeStart, StartTime = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.TimeoutTeam1, StopTime = true, StopTimeOnMatchEndEvents = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.TimeoutTeam2, StopTime = true, StopTimeOnMatchEndEvents = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.TimeoutReferee, StopTime = true, StopTimeOnMatchEndEvents = true });

            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.ScoreTeam1 });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.ScoreTeam2 });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.ScoreRevokeTeam1 });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.ScoreRevokeTeam2 });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchWinTeam1 });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.MatchWinTeam2 });

            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.FoulTeam1, StopTimeOnMatchEndEvents = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.FoulTeam2, StopTimeOnMatchEndEvents = true });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.TimePenaltyTeam1 });
            MatchEventDefinition.Add(new MatchEventItem() { EventName = MatchEventEnum.TimePenaltyTeam2 });

            tmrTimeEngine.Elapsed += TmrTimeEngine_Elapsed;            
        }

        /// <summary>
        /// Loads the next match
        /// </summary>
        /// <param name="currentMatch"></param>
        public static void LoadMatch(Match currentMatch)
        {
            CurrentMatch = currentMatch;
        }

        /// <summary>
        /// Starts the Match
        /// </summary>
        public static void StartMatch()
        {
            if (CurrentMatch == null)
            {
                Console.WriteLine("Unable to start match because no match is loaded.");
            }
            else if ((MatchStatusEnum)CurrentMatch.MatchStatus != MatchStatusEnum.Planned)
            {
                Console.WriteLine("Unable to start match because is is not in planned state.");
            }

            Console.WriteLine("Starting Match {0}", CurrentMatch.Id);
            tmrTimeEngine.Start();
        }

        /// <summary>
        /// Resumes the Match
        /// </summary>
        public static void ResumeMatch()
        {
            tmrTimeEngine.Start();
        }

        /// <summary>
        /// Ends the Match
        /// </summary>
        public static void EndMatch()
        {
            tmrTimeEngine.Stop();
        }

        /// <summary>
        /// Pause the Match
        /// </summary>
        public static void PauseMatch()
        {
            tmrTimeEngine.Stop();
        }

        /// <summary>
        /// A new event happened at the match
        /// </summary>
        /// <param name="matchEvent"></param>
        public static void MatchEvent(MatchEventItem matchEvent)
        {
            MatchEvent dbEvent = new MatchEvent();
            dbEvent.Event = matchEvent.EventName;
            dbEvent.Text = matchEvent.EventComment;
            dbEvent.Timestamp = matchEvent.TimeOfEvent;

            if (CurrentMatch.MatchEvents == null)
            {
                CurrentMatch.MatchEvents = new List<MatchEvent>();
            }
            CurrentMatch.MatchEvents.Add(dbEvent);
        }

        private static void TmrTimeEngine_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // The current time for refencing the time over
            var CurrentTime = DateTime.Now;

            // The time that is over in the current match respecting the breaks
            var TimeOver = new TimeSpan();

            // Was the last event in the foreach below an event that is starting (true) or stopping (false) the time?
            var TimeRunning = false;

            // The time of the last relevant event handled in foreach below
            var LastStartTime = new DateTime();

            // Get timespans from Events, that did not had running times
            foreach(var anEvent in MatchEventDefinition)
            {
                // If the event starts the time
                if (anEvent.StartTime)
                {
                    // Only if the last event was a stop event
                    if (TimeRunning == false) 
                    {
                        // Set the last start time
                        LastStartTime = anEvent.TimeOfEvent;

                        // Set flag that the last event was a start event
                        TimeRunning = true;
                    }
                }
                // If the event is a stop time event
                else if (anEvent.StopTime)
                {
                    // Only if the time is running
                    if (TimeRunning)
                    {
                        //Add the time since last startevent
                        var timeSinceLastStart = CurrentTime.Subtract(LastStartTime);
                        TimeOver.Add(timeSinceLastStart);

                        // Set flag that the last event was a stop event
                        TimeRunning = false;
                    }
                }
            }
            Console.WriteLine("Time over since start: {0} Seconds", TimeOver.TotalSeconds);
        }
    }
}
