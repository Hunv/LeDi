using LeDi.Server2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.CompilerServices;

namespace LeDi.Server2.Pages
{
    public partial class TournamentMatchlist
    {
        private TblTournament Tournament { get; set; }


        [Parameter]
        public int? Id { get; set; }


        protected override async Task OnInitializedAsync()
        {
            if (!Id.HasValue)
                return;

            Tournament = await DataHandler.GetTournamentAsync(Id.Value);
        }

    }
}
