using LeDi.Shared2.DatabaseModel;
using LeDi.Shared2.Display;
using LeDi.Shared2.Enum;
using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace LeDi.Display2.Effects
{
    public class Match : IEffect
    {
        /// <summary>
        /// The MatchId of the match to show
        /// </summary>
        public int MatchId { get; set; }

        /// <summary>
        /// The instance of the Connector to use
        /// </summary>
        public Connector? Connector { get; set; }

        /// <summary>
        /// The Match Object received from the server
        /// </summary>
        public TblMatch MatchObj { get; set; }


        public override async void Execute(CancellationToken EffectCancellationToken)
        {
            if (Connector == null)
            {
                Console.WriteLine("Connector is null. Match cannot be shown.");
                return;
            }

            if (Display.Display.LayoutConfig == null)
            {
                Console.WriteLine("No LayoutConfig set.");
                return;
            }

            if (Display.Display.LayoutConfig.AreaList == null)
            {
                Console.WriteLine("No AreaList available.");
                return;
            }

            Console.WriteLine("Running Match Effect to show Match ID {0}", MatchId);
            Display.Display.SetAll(Color.Black);
            Display.Display.Render();

            // Initial request
            await Connector.RequestMatch(MatchId);
        }

        /// <summary>
        /// Updates the currently shown information on the Display
        /// </summary>
        public async void Update()
        {
            Console.WriteLine("Updating Data...");

            if (MatchObj == null)
            {
                Console.WriteLine("Cannot render match. No Match Information received.");
                return;
            }

            // Get the Display Layout
            // Arealist: Display.Display.LayoutConfig.AreaList
            try
            {
                Display.Display.ShowString(MatchObj.Team1Score.ToString(), "team1goals");
                Display.Display.ShowString(MatchObj.Team2Score.ToString(), "team2goals");
                Display.Display.ShowString(":", "goaldivider");

                switch(MatchObj.MatchStatus)
                {
                    case (int)MatchStatusEnum.PeriodEnded:
                        Display.Display.ShowString("Halftime", "time");
                        break;

                    case (int)MatchStatusEnum.Ended:
                        Display.Display.ShowString("End", "time");
                        break;

                    case (int)MatchStatusEnum.Canceled:
                        Display.Display.ShowString("Canceled", "time");
                        break;

                    default:
                        Display.Display.ShowString(MatchObj.GetTimeLeftMinutes() + ":" + MatchObj.GetTimeLeftSeconds().ToString("00"), "time");
                        break;
                }

                Display.Display.ShowString(MatchObj.Team1Name ?? "Team 1", "team1name");
                Display.Display.ShowString(MatchObj.Team2Name ?? "Team 2", "team2name");

                if (MatchObj.MatchPenalties.Count > 0)
                {
                    var team1PenaltyText = "";
                    var team2PenaltyText = "";
                    foreach (var aPenalty in MatchObj.MatchPenalties)
                    {
                        var penLength = aPenalty.PenaltyTimeStart + aPenalty.PenaltyTime;
                        // Check for permanent penalty
                        if (aPenalty.PenaltyTime == 0)
                        {
                            penLength = MatchObj.CurrentTimeLeft;
                        }

                        if (aPenalty.TeamId == 0)
                            team1PenaltyText += "#" + aPenalty.PlayerNumber + " " + (penLength / 60) + ":" + (penLength % 60).ToString("00") + Environment.NewLine;
                        else
                            team2PenaltyText += "#" + aPenalty.PlayerNumber + " " + (penLength / 60) + ":" + (penLength % 60).ToString("00") + Environment.NewLine;
                    }
                    team1PenaltyText = team1PenaltyText.Trim();
                    team2PenaltyText = team2PenaltyText.Trim();

                    Display.Display.ShowString(team1PenaltyText, "team1penalties");
                    Display.Display.ShowString(team2PenaltyText, "team2penalties");
                }

                if (MatchObj.Tournament != null)
                    Display.Display.ShowString(MatchObj.Tournament.Name, "text");

                Display.Display.Render();
            }
            catch(Exception ea)
            {
                Console.WriteLine("Unable to render match display. Error: " + ea.ToString());
            }
        }
    }
}
