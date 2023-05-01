using LeDi.Server2.DatabaseModel;
using Microsoft.AspNetCore.Components;

namespace LeDi.Server2.Pages
{
    public partial class MatchPlanning
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<TblMatch>? MatchList = new List<TblMatch>();

        protected override async Task OnInitializedAsync()
        {
            var settingEventId = await DataHandler.GetSettingAsync("eventtournamentid");
            if (settingEventId != null && settingEventId.SettingValue != "")
            {
                // switch to tournament matchlist in case a tournament is running
                NavigationManager.NavigateTo("/tournamentmatchlist/" + settingEventId.SettingValue);
            }

            // Get all matches that do not belong to a tournament
            MatchList = DataHandler.GetMatchList().Where(x => x.Tournament == null).ToList();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        private string SecondsToTime(int seconds)
        {
            return ((seconds / 60).ToString("D2") + ":" + (seconds % 60).ToString("D2"));
        }
    }
}
