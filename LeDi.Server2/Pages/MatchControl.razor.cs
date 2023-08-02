using LeDi.Server2.Data;
using LeDi.Shared2.DatabaseModel;
using LeDi.Shared2.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LeDi.Server2.Pages
{
    public partial class MatchControl
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //TblMatch Match = new TblMatch(); // currently selected match
        //List<TblMatchEvent>? MatchEventList = null; //List of all match events
        List<TblMatch>? MatchList = null; // List of all not finished matches
        bool ButtonDisableStart = false; //Is the Start button disabled?
        bool ButtonDisableStartStop = false; // Is the Start/Stop-Button disabled?
        string ButtonTextStartStop = "Start"; // The text of the start/Stop button
        bool ButtonPreparePeriodDisabled = true; //Is the Prepare period button disabled?
        bool ButtonCloseMatchDisabled = true; //The close button is disabled and invisible?
        bool ButtonAllDisabled = false; //All buttons are disabled (i.e. when match ended or canceled.)
        bool HideAdvancedControls = true; //Are the advanced control buttons visible?
        string DialogYesNoDangerAction = ""; //This defines what should be executed, after the danger dialog closes.
        bool DialogYesNoDangerShow = false; //Is the modal dialog for a yes/no question shown?
        bool DialogPenaltyPlayerShow = false; // Is the modal dialog for playernumber for penalties of team 1 opened?
        bool DialogPenaltyPenaltyShow = false; //Is the modal dialog for the penalty for penalties of team 1 opened?
        bool DialogTimeShow = false; //Is the modal dialog for setting the time opened?
        bool DialogPenaltyRevokeShow = false; //Is the modal dialog for revoke penalty opened?
        bool DialogDeviceShow = false; //Is the modal dialog for device selection opened?
        int PenaltyTeamId = -1; //The ID (0 = Team 1, 1 = Team 2) of the team the currently selecting penalty is for.
        int PenaltyPlayerNumber = -1; //The Playernumber that get the penalty
        List<int> PenaltyPlayerNumbers = new List<int>(); // The playernumbers of a team. Null or empty in case the number should be entered in the modal dialog.
        List<string> PenaltyList = new List<string>(); // The list of penalties the referee can choose for the previously selected player (or team)
        List<TblDevice> DeviceList = new List<TblDevice>(); //The list of devices. Only filled when the Set-Display button was clicked.

        [Parameter]
        public int? SelectedMatchId { get; set; }

        private TblMatch Match { get { return MatchManager.LoadedMatches.Single(x => x.Id == SelectedMatchId); } }

        private List<TblMatchEvent> MatchEventList { get { return Match.MatchEvents.ToList(); } }

        private async void IncrementCountTeam1Clicked()
        {
            Logger.Trace("IncrementCountTeam1Clicked.");
            await DataHandler.UpdateMatchScoreAsync(Match.Id, 0, 1);
            MatchManager.LoadedMatches.Single(x => x.Id == SelectedMatchId.Value).Team1Score++;
            await UpdateFields();
        }
        private async void DecreaseCountTeam1Clicked()
        {
            Logger.Trace("DecreaseCountTeam1Clicked.");
            await DataHandler.UpdateMatchScoreAsync(Match.Id, 0, -1);
            MatchManager.LoadedMatches.Single(x => x.Id == SelectedMatchId.Value).Team1Score--;
            await UpdateFields();
        }

        private async void IncrementCountTeam2Clicked()
        {
            Logger.Trace("IncrementCountTeam2Clicked.");
            await DataHandler.UpdateMatchScoreAsync(Match.Id, 1, 1);
            MatchManager.LoadedMatches.Single(x => x.Id == SelectedMatchId.Value).Team2Score++;
            await UpdateFields();
        }
        private async void DecreaseCountTeam2Clicked()
        {
            Logger.Trace("DecreaseCountTeam2Clicked.");
            await DataHandler.UpdateMatchScoreAsync(Match.Id, 1, -1);
            MatchManager.LoadedMatches.Single(x => x.Id == SelectedMatchId.Value).Team2Score--;
            await UpdateFields();
        }

        private async void PenaltyTeam1Clicked()
        {
            Logger.Trace("PenaltyTeam1Clicked");
            PenaltyTeamId = 0;
            //PenaltyPlayerNumbers = ... //todo when playernumbers are implemented
            DialogPenaltyPlayerShow = true;
            await InvokeAsync(() => { StateHasChanged(); });
        }

        private async void PenaltyTeam2Clicked()
        {
            Logger.Trace("PenaltyTeam2Clicked");
            PenaltyTeamId = 1;
            //PenaltyPlayerNumbers = ... //todo when playernumbers are implemented
            DialogPenaltyPlayerShow = true;
            await InvokeAsync(() => { StateHasChanged(); });
        }

        private void RevokePenaltyClicked()
        {
            Logger.Trace("RevokePenaltyClicked");

            DialogPenaltyRevokeShow = true;
            //await InvokeAsync(() => { StateHasChanged(); });
        }

        private async void ExpandAdvancedControls()
        {
            Logger.Trace("ExpandAvancedControls clicked.");
            HideAdvancedControls = !HideAdvancedControls;
            await InvokeAsync(() => { StateHasChanged(); });
        }

        private async void OnPenaltyPlayerClose(int playerId)
        {
            DialogPenaltyPlayerShow = false;

            if (playerId >= 0)
            {
                Logger.Trace("Modal dialog select player closed. Selected PlayerId/Number: {0}", playerId);
                PenaltyPlayerNumber = playerId;

                // 0 = the penalty is for the team. All others are for the given player number.
                PenaltyList = Match.RulePenaltyList
                                .SelectMany(x => x.Display)
                                .Where(x => x.Language == "EN")
                                .Select(x => x.Text)
                                .ToList();


                //Ask for the penalty
                DialogPenaltyPenaltyShow = true;
            }
            else
            {
                Logger.Trace("Modal dialog select player canceled.");
                PenaltyTeamId = -1; // Reset penaltyId for the next penalty
            }

            await InvokeAsync(() => { StateHasChanged(); });
        }

        /// <summary>
        /// Executed when the dialog to select the penalty closes.
        /// </summary>
        /// <param name="penalty"></param>
        private async void OnPenaltyPenaltyClose(string penalty)
        {
            if (penalty != null && penalty != "")
            {
                Logger.Trace("Modal dialog select penalty closed. Selected penalty: {0}", penalty);

                var languageCode = "EN"; // This will be the users language code when localization is being implemented
                var customPenalty = false;

                // Get the penalty from the rule list
                var penaltyObj = Match.RulePenaltyList.SingleOrDefault(x => x.Display.Any(y => y.Language == languageCode && y.Text == penalty));
                if (penaltyObj == null)
                {
                    Logger.Info("The selected penalty is cannot be identified by the text \"" + penalty + "\". Expecting it is a custom penalty. Time penalties need to be added seperately.");
                    customPenalty = true;
                }

                // Create the new penalty object
                var newPenalty = new TblMatchPenalty();
                newPenalty.PlayerNumber = PenaltyPlayerNumber;
                newPenalty.PenaltyTimeStart = Match.RulePeriodLength - Match.CurrentTimeLeft;
                newPenalty.Note = string.Format("Penalty \"{0}\" given to player number {1} of {2}", penalty, PenaltyPlayerNumber, (PenaltyTeamId == 0 ? Match.Team1Name : Match.Team2Name));
                newPenalty.TeamId = PenaltyTeamId;
                newPenalty.Timestamp = DateTime.Now;

                if (!customPenalty && penaltyObj != null) // the null check is not required but the null check compiler wants it.
                {
                    newPenalty.PenaltyTime = penaltyObj.PenaltySeconds;
                    newPenalty.PenaltyName = penaltyObj.Display.Single(x => x.Language == "EN").Text; // The internal name is always the English name
                }
                else
                {
                    newPenalty.PenaltyName = penalty;
                }

                // it is a team penalty
                if (PenaltyPlayerNumber == 0)
                {
                    newPenalty.Note = string.Format("Penalty \"{0}\" given to team {1}", penalty, (PenaltyTeamId == 0 ? Match.Team1Name : Match.Team2Name));
                }

                // Register the penalty
                Match.MatchPenalties.Add(newPenalty);
                //await DataHandler.SaveChangesAsync();
            }
            else
            {
                Logger.Trace("Modal dialog select penalty canceled.");
            }

            // Reset all values and hide dialog
            PenaltyPlayerNumber = -1;
            PenaltyTeamId = -1;
            DialogPenaltyPenaltyShow = false;

            // Reload match details (and UI)
            await UpdateFields();
        }

        /// <summary>
        /// Executed when the yes/no modal dialog closes
        /// </summary>
        /// <param name="result"></param>
        private async void OnYesNoClose(bool result)
        {
            if (result)
            {
                switch (DialogYesNoDangerAction)
                {
                    case "CancelMatch":
                        Logger.Info("Running action CancelMatch after confirmation dialog was confirmed.");

                        Logger.Info("Canceling match...");

                        Logger.Info("TODO TODO TODO: Stop the timer of the match");
                        //await Api.ControlMatchtimeAsync(Match.Id, "stop");
                        Match.MatchStatus = (int)MatchStatusEnum.Canceled;
                        //await DataHandler.SaveChangesAsync();

                        // Show/hide buttons
                        ButtonDisableStart = true;
                        ButtonCloseMatchDisabled = true;
                        ButtonPreparePeriodDisabled = true;
                        DialogYesNoDangerShow = false;
                        DialogYesNoDangerAction = "";
                        ButtonAllDisabled = true;

                        await InvokeAsync(() => { StateHasChanged(); });
                        break;
                    case "RestartMatch":
                        Logger.Info("Running action RestartMatch after confirmation dialog was confirmed.");
                        Logger.Info("Restarting match...");

                        Logger.Info("TODO TODO TODO: Stop the timer of the match");
                        //await Api.ControlMatchtimeAsync(Match.Id, "stop");

                        Match.CurrentTimeLeft = Match.RulePeriodLength;
                        Match.CurrentPeriod = 1;
                        Match.MatchPenalties = new List<TblMatchPenalty>();
                        Match.MatchStatus = (int)MatchStatusEnum.ReadyToStart;
                        //await DataHandler.SaveChangesAsync();

                        // Show/hide buttons
                        ButtonDisableStart = false;
                        ButtonCloseMatchDisabled = true;
                        ButtonPreparePeriodDisabled = true;
                        DialogYesNoDangerShow = false;
                        DialogYesNoDangerAction = "";

                        await InvokeAsync(() => { StateHasChanged(); });
                        break;
                    default:
                        Logger.Warn("The action {0} is not know to be executed after yes/no danger dialog closes.", DialogYesNoDangerAction);
                        break;
                }

                await InvokeAsync(() => { StateHasChanged(); });
            }
        }

        /// <summary>
        /// Executed when the Time Modal Dialog closes.
        /// </summary>
        /// <param name="timeleftSecondsModifier"></param>
        private async void OnTimeClose(int? timeleftSecondsModifier)
        {
            if (timeleftSecondsModifier != null)
            {
                Logger.Debug("OnTimeClose dialog closed with time modifer of {0}", timeleftSecondsModifier);

                var newTimeLeft = Match.CurrentTimeLeft + timeleftSecondsModifier;
                newTimeLeft = (newTimeLeft < 0 ? 0 : newTimeLeft);

                Match.CurrentTimeLeft = newTimeLeft.Value;
                //await DataHandler.SaveChangesAsync();
            }

            DialogTimeShow = false;
            await UpdateFields();
        }

        /// <summary>
        /// Exected when the Revoke Penalty Dialog closes
        /// </summary>
        /// <param name="revokedPenalty"></param>
        private async void OnPenaltyRevokeClose(TblMatchPenalty revokedPenalty)
        {
            if (revokedPenalty != null)
            {
                Logger.Debug("Revoking penalty {0}", revokedPenalty.Id);

                var revokedPenaltyDb = Match.MatchPenalties.SingleOrDefault(x => x.Id == revokedPenalty.Id);
                if (revokedPenaltyDb == null)
                    return;

                // Remove the penalty
                revokedPenaltyDb.Revoked = true;
                revokedPenaltyDb.RevokeNote = "Revoking \"" + revokedPenalty.Note + "\"";

                //await DataHandler.SaveChangesAsync();
                await UpdateFields();
            }
            DialogPenaltyRevokeShow = false;
        }

        /// <summary>
        /// Exectuted when the Start/Stop Button was clicked
        /// </summary>
        private async void MatchStartStopClicked()
        {

            Logger.Trace("StartStopMatchClicked");

            if (Match == null)
                return;

            // If the match is currently running, it will be stopped
            if (Match.MatchStatus == (int)MatchStatusEnum.Running)
            {
                await MatchManager.SetMatchStatus(Match.Id, MatchStatusEnum.Stopped);
                ButtonTextStartStop = Localizer["StartTime"]; //Text must be "Start" now, because the time is stopped
                await InvokeAsync(() => { StateHasChanged(); });
            }
            // if the match is active but the time is not running, start the time
            else if (Match.MatchStatus == (int)MatchStatusEnum.Planned || Match.MatchStatus == (int)MatchStatusEnum.ReadyToStart || Match.MatchStatus == (int)MatchStatusEnum.Stopped)
            {
                await MatchManager.SetMatchStatus(Match.Id, MatchStatusEnum.Running);
                ButtonTextStartStop = Localizer["StopTime"]; //Text must be "Stop" now, because the time is running
                await InvokeAsync(() => { StateHasChanged(); });
            }
        }

        private void MatchCancelClicked()
        {
            Logger.Trace("MatchCancelClicked");

            DialogYesNoDangerAction = "CancelMatch";
            DialogYesNoDangerShow = true;
        }

        private void MatchRestartClicked()
        {
            Logger.Trace("MatchCancelClicked");

            DialogYesNoDangerAction = "RestartMatch";
            DialogYesNoDangerShow = true;
        }

        private async void MatchSetTimeClicked()
        {
            Logger.Trace("MatchSetTimeClicked");

            DialogTimeShow = true;
            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected override async Task OnParametersSetAsync()
        {
            Logger.Trace("Reloading the page.");
            await UpdateFields();

            if (Match.MatchStatus == (int)MatchStatusEnum.Canceled || Match.MatchStatus == (int)MatchStatusEnum.Ended || Match.MatchStatus == (int)MatchStatusEnum.Closed)
            {
                ButtonAllDisabled = true;
            }

            await base.OnParametersSetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            Logger.Trace("Initializing the page");

            if (SelectedMatchId == null)
            {
                Logger.Debug("No match selected.");
                await LoadMatchList();
            }
            else
            {
                Logger.Debug("Match {0} selected.", SelectedMatchId);
                await UpdateFields();
            }

            // Get information about updated Match(es)
            MatchManager.OnMatchTimeUpdated += async (object? sender, EventArgs e) =>
            {
                if (sender == null)
                    return;

                // Get the latest infos if the updated matchId is the currently shown match
                if ((int)sender == SelectedMatchId)
                {
                    //var x = await DataHandler.GetMatchAsync(SelectedMatchId.Value);                    
                    await InvokeAsync(() => { StateHasChanged(); });
                }
            };

            // Get information about ended periods
            MatchManager.OnPeriodTimeOver += async (object? sender, EventArgs e) =>
            {
                if (sender == null)
                    return;

                // Get the latest infos if the updated matchId is the currently shown match
                if ((int)sender == SelectedMatchId)
                {
                    await SetPageControls();
                }
            };

            // Get information about updated Match Status
            MatchManager.OnMatchUpdated += async (object? sender, EventArgs e) =>
            { 
                if (sender == null) 
                    return; 
                
                if ((int)sender == SelectedMatchId)
                {
                    await InvokeAsync(() => { StateHasChanged(); });
                }
            };

            await InvokeAsync(() => { StateHasChanged(); });
        }

        /// <summary>
        /// Sets the controls depending on match status
        /// </summary>
        /// <returns></returns>
        private async Task SetPageControls()
        {
            if (Match.MatchStatus == (int)MatchStatusEnum.Canceled || Match.MatchStatus == (int)MatchStatusEnum.Ended || Match.MatchStatus == (int)MatchStatusEnum.Closed)
            {
                ButtonAllDisabled = true;
                return;
            }

            var x = MatchManager.LoadedMatches.SingleOrDefault(x => x.Id == SelectedMatchId);
            if (x != null)
            {
                if (Match.MatchStatus == (int)MatchStatusEnum.PeriodEnded || Match.MatchStatus == (int)MatchStatusEnum.ReadyToStart)
                {
                    Logger.Debug("Not-the-last period is over.");
                    ButtonDisableStartStop = true;
                    ButtonPreparePeriodDisabled = false;
                    ButtonCloseMatchDisabled = true;
                    ButtonTextStartStop = Localizer["StartTime"];
                }
                else if (Match.MatchStatus == (int)MatchStatusEnum.Ended)
                {
                    Logger.Debug("Last period is over.");
                    ButtonDisableStartStop = true;
                    ButtonPreparePeriodDisabled = true;
                    ButtonCloseMatchDisabled = false;
                }
                else if (Match.MatchStatus == (int)MatchStatusEnum.Running)
                {
                    Logger.Debug("Match is running");
                    ButtonDisableStartStop = false;
                    ButtonPreparePeriodDisabled = true;
                    ButtonCloseMatchDisabled = true;
                    ButtonTextStartStop = Localizer["StopTime"];

                }
            }
            await InvokeAsync(() => { StateHasChanged(); });
        }


        /// <summary>
        /// Get the list of all matches from the database and save it to the MatchList variable.
        /// </summary>
        /// <returns></returns>
        private async Task LoadMatchList()
        {
            Logger.Trace("Loading MatchList");
            var matchList = DataHandler.GetMatchList();
            if (matchList == null || matchList.Count == 0)
            {
                Logger.Debug("No matches in matchlist");

                //Create empty list
                MatchList = new List<TblMatch>();
                return;
            }
            MatchList = matchList.Where(x => x.MatchStatus != (int)MatchStatusEnum.Undefined && x.MatchStatus != (int)MatchStatusEnum.Canceled && x.MatchStatus != (int)MatchStatusEnum.Closed).ToList() ?? new List<TblMatch>();
        }

        /// <summary>
        /// Update the fields of the page
        /// </summary>
        /// <returns></returns>
        private async Task UpdateFields()
        {
            if (SelectedMatchId == null)
            {
                Logger.Debug("No match selected to load.");
                return;
            }

            Logger.Debug("Loading match {0}", SelectedMatchId);
            //await DataHandler.GetMatchAsync(SelectedMatchId.Value)
            await MatchManager.LoadMatch(SelectedMatchId.Value);

            if (MatchManager.LoadedMatches.SingleOrDefault(x => x.Id == SelectedMatchId) == null)
            {
                Logger.Warn("Match {0} not loaded.", SelectedMatchId.Value);
                return;
            }

            await SetPageControls();
        }

        /// <summary>
        /// Executed, when the prepare period button is clicked.
        /// </summary>
        private async Task MatchPreparePeriodClicked()
        {
            Logger.Trace("PreparePeriod clicked.");

            if (Match == null)
            {
                Logger.Error("Match is null. Cannot prepare next period.");
                return;
            }

            if (SelectedMatchId == null)
            {
                Logger.Error("Selected Match ID is null.");
                return;
            }

            // only if the current period is over
            if (Match.CurrentTimeLeft != 0)
            {
                Logger.Warn("Current period is not over.");
                ButtonPreparePeriodDisabled = true;
                return;
            }

            //Check if there is another period
            if (Match.CurrentPeriod < Match.RulePeriodCount)
            {
                ButtonPreparePeriodDisabled = true;

                Match.CurrentPeriod++;
                Match.CurrentTimeLeft = Match.RulePeriodLength;
                Match.MatchStatus = (int)MatchStatusEnum.ReadyToStart;
                await DataHandler.SetMatchAsync(Match);

                await UpdateFields();

                await InvokeAsync(() => { StateHasChanged(); });
            }
        }

        /// <summary>
        /// Executed when the close button is clicked
        /// </summary>
        private async Task MatchCloseClicked()
        {
            Logger.Trace("CloseMatch clicked");

            if (Match == null)
                return;

            if (SelectedMatchId == null)
                return;

            // The last period must be over
            if (Match.CurrentPeriod != Match.RulePeriodCount && Match.CurrentTimeLeft == 0)
                return;

            Match.MatchStatus = (int)MatchStatusEnum.Closed;
            await DataHandler.SetMatchAsync(Match);
            //await DataHandler.SaveChangesAsync();

            ButtonCloseMatchDisabled = true;
            ButtonAllDisabled = true;

            // Go back to the match control page
            SelectedMatchId = null;
            await LoadMatchList();
            NavigationManager.NavigateTo("matchcontrol");
        }

        /// <summary>
        /// Executed when the Set Display button is clicked
        /// </summary>
        /// <returns></returns>
        private async void SetDisplayClicked()
        {
            Logger.Trace("SetDisplay clicked");

            if (SelectedMatchId != null)
            {
                var deviceList = await DataHandler.GetDeviceListAsync();
                if (deviceList == null)
                {
                    Logger.Warn("No Devices configured or cannot get devicelist from server.");
                    return;
                }

                DeviceList = deviceList;
                DialogDeviceShow = true;
            }
            else
            {
                Logger.Warn("No match selected");
            }
        }

        /// <summary>
        /// Executed after the DeviceDialog closes
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        private async Task OnDisplayClose(TblDevice dev)
        {
            DialogDeviceShow = false;
            await InvokeAsync(() => { StateHasChanged(); });

            Logger.Debug("Setting display {0} to show match id {1}", dev.DeviceId, SelectedMatchId);

            await DataHandler.hubContext.Clients.Group(dev.DeviceId).SendAsync("SetEffect", "match", SelectedMatchId.ToString());
        }
    }
}
