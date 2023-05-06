using LeDi.Server2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LeDi.Server2.Pages
{
    public partial class MatchPlanning
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<TblMatch>? MatchList = new List<TblMatch>();


        /// <summary>
        /// Contains the roles a user has
        /// </summary>
        private TblUserRole? AuthenticatedUserRole { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Get the roles of the currently logged in user
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState != null && authState.User.Identity != null && authState.User.Identity.IsAuthenticated)
            {
                // Get the Roles from Identity management. Should only be one always.
                var username = authState.User.Identity.Name;
                if (username != null)
                {
                    var roles = await _UserManager.GetRolesAsync(await _UserManager.FindByNameAsync(username));

                    if (roles != null && roles.Count >= 1)
                    {
                        AuthenticatedUserRole = await DataHandler.GetUserRoleAsync(roles[0]);
                    }
                }
            }
            else
            {
                AuthenticatedUserRole = await DataHandler.GetUserRoleAsync("Guests");
            }

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
