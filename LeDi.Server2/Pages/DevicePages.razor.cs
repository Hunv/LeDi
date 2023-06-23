using BlazorBootstrap;
using LeDi.Shared2.DatabaseModel;
using LeDi.Shared2.EffectParameters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LeDi.Server2.Pages
{
    public partial class DevicePages
    {
        // List of all registred and enabled devices.
        private List<TblDevice>? DeviceList = null;

        // The Tab control. Used to get the selected tab.
        private Tabs EffectTabs;

        // The Tab currently active
        private string ActiveTabTitle;

        // Logger Instance
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Device ID that was selected to show an effect
        /// </summary>
        [Parameter]
        public string? SelectedDeviceId { get; set; }

        /// <summary>
        /// The text to show if the text effect is selected
        /// </summary>
        private string? EffectTextContent { get; set; }

        /// <summary>
        /// The countdown text to show if the countdown effect is selected
        /// </summary>
        private string? EffectCountdownText { get; set; }

        /// <summary>
        /// The time the countdown effect should count down in seconds.
        /// </summary>
        private int? EffectCountdownSeconds { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DeviceList = (await DataHandler.GetDeviceListAsync()).Where(x => x.Enabled).ToList();
            
        }
        private void OnTabShownAsync(TabsEventArgs args)
            => ActiveTabTitle = args.ActiveTabTitle;

        /// <summary>
        /// Sets the selected effect for the selected device.
        /// </summary>
        private async Task SetEffect()
        {
            if (SelectedDeviceId == null)
            {
                Logger.Warn("Cannot set an effect of no device is selected.");
                return;
            }


            if (ActiveTabTitle == Localizer["EffectText"])
            {
                await DataHandler.hubContext.Clients.Group(SelectedDeviceId).SendAsync("SetEffect", "text", EffectTextContent);
            }
            else if (ActiveTabTitle == Localizer["EffectMatch"])
            {
            }
            else if (ActiveTabTitle == Localizer["EffectTournament"])
            {
            }
            else if (ActiveTabTitle == Localizer["EffectCountdown"])
            {
                if (!EffectCountdownSeconds.HasValue)
                {
                    Logger.Warn("To run the countdown effect, seconds must have a value.");
                    return;
                }
                await DataHandler.hubContext.Clients.Group(SelectedDeviceId).SendAsync("SetEffect", "countdown", JsonConvert.SerializeObject(new CountdownParameters() { Seconds = EffectCountdownSeconds.Value, Text = EffectCountdownText}));
            }
        }
    }
}
