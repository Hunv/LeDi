using System.Timers;
using LeDi.Server.Classes;
using LeDi.Server.DatabaseModel;
using LeDi.Shared.Enum;

namespace LeDi.Server
{
    public class MatchHandler
    {
        private readonly System.Timers.Timer tmrMatchtimer = new(SystemSettings.MatchHandlerRefreshTime);
        private readonly System.Timers.Timer tmrDisposeTimer = new(SystemSettings.MatchHandlerDisposeTime); //To dispose this Handler 10 Minutes after game finished
        private DateTime? ReferenceSystemTime;
        private int ReferenceSecond = 0;
        private bool IsInitialized = false;
        private MatchStatusEnum MatchStatus = MatchStatusEnum.Undefined;
        public int MatchId { get; set; }
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public MatchHandler(int matchId, bool setPlannedStatus = true)
        {
            _logger.Debug("Initializing new MatchHandler for match {0}", matchId);

            MatchId = matchId;

            if (setPlannedStatus)
                Task.Run(async () => await SetMatchStatus(MatchStatusEnum.Planned, MatchId)).Wait();
        }

        private async void TmrMatchtimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _logger.Trace("tmrMatchtimer elapsed");
            await UpdateMatchtimer();
        }

        /// <summary>
        /// Is updating the match timer.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateMatchtimer()
        {
            _logger.Trace("UpdateMatchtimer called.");

            //If not initialized, cancel
            if (ReferenceSystemTime == null)
            {
                _logger.Warn("ReferenceSystemTime not set.");
                return;
            }

            //If a second is over
            if (ReferenceSecond != DateTime.Now.Second)
            {
                _logger.Trace("A Second is over.");

                //Get the difference of the seconds. In case of high load or hickup, it may be more than one
                var diff = DateTime.Now.Second - ReferenceSecond;
                if (diff < 0)
                {
                    _logger.Debug("Correcting the reference setting due to possible high load or hickup.");
                    diff += 60;
                }

                //Set the new reference value
                ReferenceSecond = DateTime.Now.Second;

                //Decrease SecondsLeft
                using var dbContext = new TwDbContext();

                if (dbContext.Matches != null)
                {
                    //If time is over and there is no overtime, stop the time
                    //otherwise, reduce the time
                    if (dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft <= 0 &&
                        (MatchRules.Rules == null || MatchRules.Rules.RuleHalftimeOvertime == false)
                        )
                    {
                        await LogEvent(MatchId, MatchEventEnum.HalftimeEnd, "Halftime over.");
                        Stop();
                    }
                    else
                    {                        
                        dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft -= diff;
                        _logger.Debug("Setting new time left {0} (-{1}) for match {2}", dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft, diff, MatchId);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task Start()
        {
            _logger.Info("Starting match timer for match {0}...", MatchId);

            if (!IsInitialized)
            {
                _logger.Debug("Handler is not initialized. Initializing...");

                tmrMatchtimer.Elapsed += TmrMatchtimer_Elapsed;
                tmrDisposeTimer.Elapsed += TmrDisposetimer_Elapsed;
                IsInitialized = true;

                // Load the Match
                using var dbContext = new TwDbContext();
                if (dbContext.Matches != null)
                {
                    var match = dbContext.Matches.SingleOrDefault(x => x.Id == MatchId);
                    if (match != null)
                    {
                        if ((MatchRules.Rules == null || MatchRules.Rules.GameName != match.GameName) && match.GameName != null)
                        {
                            await MatchRules.LoadRules(match.GameName);
                        }

                        // If the match was not already running, run it by increasing the halftime to 1.
                        if (match.CurrentHalftime == 0)
                            match.CurrentHalftime++;

                    }
                    await dbContext.SaveChangesAsync();
                }
            }

            ReferenceSystemTime = DateTime.Now;
            ReferenceSecond = DateTime.Now.Second == 0 ? 59 : DateTime.Now.Second - 1;

            _logger.Debug("ReferenceSystemTime is {0}", ReferenceSystemTime);
            _logger.Debug("ReferenceSecond is {0}", ReferenceSecond);

            tmrMatchtimer.Start();
            await UpdateMatchtimer();
            await SetMatchStatus(MatchStatusEnum.Running, MatchId);
        }

        /// <summary>
        /// Stops the timer but not end the match
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            _logger.Info("Stopping matchtimer for match {0}...", MatchId);
            tmrMatchtimer.Stop();
        }

        /// <summary>
        /// The game ended and the scores are fixed. Time is not running anymore and no overtime, penalty shots etc. are left.
        /// </summary>
        public async Task Finish()
        {
            _logger.Info("Finishing match {0}...", MatchId);
            tmrMatchtimer.Stop();
            ReferenceSystemTime = null;
            await SetMatchStatus(MatchStatusEnum.Ended, MatchId);
            tmrDisposeTimer.Start();
            await LogEvent(MatchId, MatchEventEnum.MatchFinished, "Match finished by the main referee.");
        }

        /// <summary>
        /// Load the next halftime after another halftime ended (increase halftime counter and reset the halftime length)
        /// </summary>
        public async Task NextHalftime()
        {
            _logger.Info("Preparing next Halftime for match {0}...", MatchId);
            using var dbContext = new TwDbContext();
            if (dbContext.Matches != null)
            {
                var match = dbContext.Matches.SingleOrDefault(x => x.Id == MatchId);
                if (match != null)
                {
                    if (match.CurrentHalftime < match.RuleHalftimeCount && match.CurrentTimeLeft == 0)
                    {
                        match.CurrentHalftime++;
                        match.CurrentTimeLeft = match.RuleHalftimeLength;
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Sets the MatchStatus in the cache and in the Database.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="matchId"></param>
        private async Task SetMatchStatus(MatchStatusEnum status, int matchId)
        {
            _logger.Trace("Executing SetMatchStatus...");

            // Set the cached status
            MatchStatus = status;

            // Set the Database status
            using var dbContext = new TwDbContext();

            if (dbContext.Matches != null)
            {
                var match = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (match != null)
                {
                    // Set the match status in DB
                    match.MatchStatus = (int)status;
                    await dbContext.SaveChangesAsync();

                    _logger.Info("Set status for match {0} to {1}.", MatchId, (int)status);
                }
            }
        }

        /// <summary>
        /// Cleanup this Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TmrDisposetimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _logger.Trace("Disposetimer elapsed...");
            await SetMatchStatus(MatchStatusEnum.Ended, MatchId);
            OnDisposeMatchhandler(new EventArgs());
        }

        public event EventHandler<EventArgs>? DisposeMatchHandler;
        protected virtual void OnDisposeMatchhandler(EventArgs e)
        {
            DisposeMatchHandler?.Invoke(this, e);
        }


        /// <summary>
        /// Logs an event match
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="matchEvent"></param>
        /// <param name="matchText"></param>
        /// <returns></returns>
        private async Task LogEvent(int matchId, MatchEventEnum matchEvent, string matchText)
        {
            _logger.Trace("Logging new matchevent {0} for match {1} with text {2}...", (int)matchEvent, matchId, matchText);

            using var dbContext = new TwDbContext();
            if (dbContext.Matches != null)
            {
                var match = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (match != null)
                {
                    if (match.MatchEvents == null)
                        match.MatchEvents = new List<MatchEvent>();

                    if (match.MatchEvents != null)
                    {
                        var timeSinceStart = match.RuleHalftimeLength - match.CurrentTimeLeft + (match.CurrentHalftime - 1) * match.RuleHalftimeLength;
                        if (timeSinceStart < 0)
                            timeSinceStart = 0;

                        match.MatchEvents.Add(new MatchEvent
                        {
                            Event = matchEvent,
                            Text = matchText,
                            Timestamp = DateTime.Now,
                            Matchtime = timeSinceStart
                        });
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
