using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.CompilerServices;

namespace LeDi.Server2.Pages
{
    public partial class TournamentPlanning
    {
        private List<TblTournament> TournamentList { get; set; } = new List<TblTournament>();
        private string? EventTournamentId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TournamentList = await DataHandler.GetTournamentListAsync();

            var evId = await DataHandler.GetSettingAsync("eventtournamentid");
            if (evId != null)
            {
                EventTournamentId = evId.SettingValue;
            }
        }

        private async void StartTournament(int tournamentId)
        {
            await DataHandler.SetSettingAsync("eventtournamentid", tournamentId.ToString());
            EventTournamentId = tournamentId.ToString();
            //await InvokeAsync(() => { StateHasChanged(); });
            NavigationManager.NavigateTo("/tournamentplanning",true);
        }
        private async void StopTournament()
        {
            await DataHandler.SetSettingAsync("eventtournamentid", "");
            EventTournamentId = string.Empty;
            //await InvokeAsync(() => { StateHasChanged(); });
            NavigationManager.NavigateTo("/tournamentplanning", true);
        }
    }
}
