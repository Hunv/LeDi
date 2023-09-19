﻿using LeDi.Shared2.DatabaseModel;
using LeDi.Shared2.EffectParameters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LeDi.Server2.Pages
{
    public partial class Start
    {
        private string LinkNewMatch = "matchadd";

        protected override async Task OnInitializedAsync()
        {
            var evId = await DataHandler.GetSettingAsync("eventtournamentid");
            if (evId != null && evId.SettingValue != string.Empty)
            {
                // In case a tournament is running, matchadd will add a new match to the tournament.
                LinkNewMatch = "/matchadd?tournamentid=" + evId.SettingValue + "&returnurl=/start";
            }
        }
    }
}
