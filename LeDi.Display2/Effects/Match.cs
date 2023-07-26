using LeDi.Shared2.DatabaseModel;
using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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
        }
    }
}
