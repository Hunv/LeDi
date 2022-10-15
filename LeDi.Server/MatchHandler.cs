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

        public MatchHandler(int matchId, bool setPlannedStatus = true)
        {
            MatchId = matchId;            

            if (setPlannedStatus)
                Task.Run(async () => await SetMatchStatus(MatchStatusEnum.Planned, MatchId)).Wait();
        }

        private async void TmrMatchtimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            await UpdateMatchtimer();
        }

        /// <summary>
        /// Is updating the match timer.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateMatchtimer()
        {
            //If not initialized, cancel
            if (ReferenceSystemTime == null)
                return;

            //If a second of over
            if (ReferenceSecond != DateTime.Now.Second)
            {
                //Get the difference of the seconds. In case of high load or hickup, it may be more than one
                var diff = DateTime.Now.Second - ReferenceSecond;
                if (diff < 0)
                    diff += 60;

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
                        Stop();
                    }
                    else
                    {
                        dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft -= diff;
                        Console.WriteLine("{2} Timeleft: {0}, (-{1}) ", dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft, diff, MatchId);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task Start()
        {
            if (!IsInitialized)
            {
                tmrMatchtimer.Elapsed += TmrMatchtimer_Elapsed;
                tmrDisposeTimer.Elapsed += TmrDisposetimer_Elapsed;
                IsInitialized = true;

                // Load the Match rules
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

                        if (match.CurrentHalftime == 0)
                            match.CurrentHalftime++;

                    }
                    await dbContext.SaveChangesAsync();
                }
            }

            ReferenceSystemTime = DateTime.Now;
            ReferenceSecond = DateTime.Now.Second == 0 ? 59 : DateTime.Now.Second - 1;
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
            tmrMatchtimer.Stop();
            //ReferenceSystemTime = null;
            //await SetMatchStatus(MatchStatusEnum.Ended, MatchId);
        }

        /// <summary>
        /// The game ended and the scores are fixed. Time is not running anymore and no overtime, penalty shots etc. are left.
        /// </summary>
        public async Task Finish()
        {
            tmrMatchtimer.Stop();
            ReferenceSystemTime = null;
            await SetMatchStatus(MatchStatusEnum.Ended, MatchId);
            tmrDisposeTimer.Start();
        }

        /// <summary>
        /// Load the next halftime after another halftime ended (increase halftime counter and reset the halftime length)
        /// </summary>
        public async Task NextHalftime()
        {
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
            await SetMatchStatus(MatchStatusEnum.Ended, MatchId);
            OnDisposeMatchhandler(new EventArgs());
        }

        public event EventHandler<EventArgs>? DisposeMatchHandler;
        protected virtual void OnDisposeMatchhandler(EventArgs e)
        {
            DisposeMatchHandler?.Invoke(this, e);
        }
    }
}
