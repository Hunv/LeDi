using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace LeDi.Server2.Shared
{
    public partial class MainLayout
    {
        /// <summary>
        /// Contains a text, that is shown on top of the page. i.e. an event name
        /// </summary>
        private string? CurrentText { get; set; }

        /// <summary>
        /// Contains the ID of the current Event
        /// </summary>
        private string? CurrentEventId { get; set; }


        public MainLayout() { }


        protected override async Task OnInitializedAsync()
        {
            var evId = await DataHandler.GetSettingAsync("eventtournamentid");
            if (evId != null && evId.SettingValue != string.Empty && evId.SettingValue != CurrentEventId)
            {
                var tournament = await DataHandler.GetTournamentAsync(Convert.ToInt32(evId.SettingValue));
                if (tournament != null)
                {
                    CurrentText = tournament.Name;
                    CurrentEventId = evId.SettingValue;
                }
            }
            await base.OnInitializedAsync();
            return;
        }
    }
}
