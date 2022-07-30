using System.Timers;
using Tiwaz.Server.Classes;
using Tiwaz.Server.DatabaseModel;

namespace Tiwaz.Server
{
    public class MatchHandler
    {
        private System.Timers.Timer tmrMatchtimer = new System.Timers.Timer(SystemSettings.MatchHandlerRefreshTime);
        private System.Timers.Timer tmrDisposeTimer = new System.Timers.Timer(SystemSettings.MatchHandlerDisposeTime); //To dispose this Handler 10 Minutes after game finished
        private DateTime? ReferenceSystemTime;
        private int ReferenceSecond = 0;
        private bool IsInitialized = false;
        private MatchStatusEnum MatchStatus = MatchStatusEnum.Undefined;
        public int MatchId { get; set; }

        public MatchHandler(int matchId)
        {
            MatchId = matchId;
            MatchStatus = MatchStatusEnum.Planned;

            using (var dbContext = new TwDbContext())
            {
                var match = dbContext.Matches.SingleOrDefault(x => x.Id == matchId);
                if (match != null)
                {
                    if (MatchRules.Rules == null || MatchRules.Rules.GameName != match.GameName)
                    {
                        MatchRules.LoadRules(match.GameName);
                    }
                }
            }
        }

        private void TmrMatchtimer_Elapsed(object sender, ElapsedEventArgs e)
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
                using (var dbContext = new TwDbContext())
                {
                    //If time is over and there is no overtime, stop the time
                    //otherwise, reduce the time
                    if (dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft <= 0 &&
                        (MatchRules.Rules == null || MatchRules.Rules.HalftimeOvertime == false)
                        )
                    {
                        Stop();
                    }
                    else
                    {
                        dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft -= diff;
                        Console.WriteLine("{2} Timeleft: {0}, (-{1}) ", dbContext.Matches.Single(x => x.Id == MatchId).CurrentTimeLeft, diff, MatchId);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public void Start()
        {
            if (!IsInitialized)
            {
                tmrMatchtimer.Elapsed += TmrMatchtimer_Elapsed;
                tmrDisposeTimer.Elapsed += TmrDisposetimer_Elapsed;
                IsInitialized = true;
            }

            ReferenceSystemTime = DateTime.Now;
            ReferenceSecond = DateTime.Now.Second == 0 ? 59 : DateTime.Now.Second - 1;
            tmrMatchtimer.Start();
            MatchStatus = MatchStatusEnum.Running;
        }

        public void Stop()
        {
            tmrMatchtimer.Stop();
            ReferenceSystemTime = null;
            MatchStatus = MatchStatusEnum.Ended;
        }

        /// <summary>
        /// The game ended and the scores are fixed. Time is not running anymore and no overtime, penalty shots etc. are left.
        /// </summary>
        public void Finish()
        {
            tmrMatchtimer.Stop();
            ReferenceSystemTime = null;
            MatchStatus = MatchStatusEnum.Ended;
            tmrDisposeTimer.Start();
        }


        /// <summary>
        /// Cleanup this Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmrDisposetimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MatchStatus = MatchStatusEnum.Ended;
            OnDisposeMatchhandler(new EventArgs());
        }

        public event EventHandler<EventArgs>? DisposeMatchHandler;
        protected virtual void OnDisposeMatchhandler(EventArgs e)
        {
            EventHandler<EventArgs> handler = DisposeMatchHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
