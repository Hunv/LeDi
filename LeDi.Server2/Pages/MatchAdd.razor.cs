using LeDi.Server2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;

namespace LeDi.Server2.Pages
{
    public partial class MatchAdd
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private int SelectedDisplay { get; set; }

        /// <summary>
        /// This contains a list of all available displays
        /// </summary>
        private List<TblDevice> DisplayList { get; set; } = new List<TblDevice>();

        /// <summary>
        /// If this match belongs to a tournament, the tournament ID is != null
        /// </summary>
        private int? TournamentId { get; set; }

        /// <summary>
        /// URL to open after click on "Save". When set, also hides the "Save and start" button.
        /// </summary>
        private string ReturnUrl { get; set; }


        /// <summary>
        /// Contains the roles a user has
        /// </summary>
        private TblUserRole? AuthenticatedUserRole { get; set; }

        /// <summary>
        /// The parameters for a new match
        /// </summary>
        TblMatch NewMatch = new TblMatch()
        {
            GameName = "",
            Team1Name = "Team1",
            Team2Name = "Team2",
            RulePeriodCount = 2,
            CurrentTimeLeft = 600,
            CurrentPeriod = 1,
            ScheduledTime = DateTime.Now,
            MatchStatus = (int)Enum.MatchStatusEnum.Planned
        };

        /// <summary>
        /// The name of the selected sport
        /// </summary>
        public string SelectedSport
        {
            get
            {
                return NewMatch.GameName ?? "";
            }
            set
            {                
#pragma warning disable CS4014
                SelectedSportChanged(value);
#pragma warning restore CS4014
            }
        }


        /// <summary>
        /// Represents the full minutes left of the NewMatch-Object
        /// </summary>
        private int TimeLeftMinutesProxy
        {
            get
            {
                if (NewMatch == null)
                    return 0;

                return (int)(NewMatch.CurrentTimeLeft / 60);
            }
            set
            {
                if (NewMatch == null)
                    return;

                NewMatch.CurrentTimeLeft = value * 60 + TimeLeftSecondsProxy;
                NewMatch.RulePeriodLength = NewMatch.CurrentTimeLeft;
            }
        }

        /// <summary>
        /// Represents the seconds only left of the NewMatch-Object
        /// </summary>
        private int TimeLeftSecondsProxy
        {
            get
            {
                if (NewMatch == null)
                    return 0;

                return (int)(NewMatch.CurrentTimeLeft % 60);
            }
            set
            {
                if (NewMatch == null)
                    return;

                NewMatch.CurrentTimeLeft = TimeLeftMinutesProxy + value;
                NewMatch.RulePeriodLength = NewMatch.CurrentTimeLeft;
            }
        }

        //Dictionary that defines the fields that are shown when SelectedGamename has the value of that match type.
        public Dictionary<string, List<string>> FieldList = new Dictionary<string, List<string>>
    {
        {
            "", new List<string>
            {

            }
        },
        {
            "Underwaterhockey", new List<string>
            {
                "displaylist",
                "txtTeamName1",
                "txtTeamName2",
                "txtPeriodCount",
                "txtPeriodLength",
                "dtScheduledTime",
                "chkPeriodPauseNearEnd",
                "txtPeriodPauseNearEndSec",
                "chkMatchExtensionOnDraw"
            }
        },
        {
            "Handball", new List<string>
            {
                "displaylist",
                "txtTeamName1",
                "txtTeamName2",
                "txtPeriodCount",
                "txtPeriodLength",
                "dtScheduledTime",
                "chkMatchExtensionOnDraw"
            }
        },
        {
            "Soccer", new List<string>
            {
                "displaylist",
                "txtTeamName1",
                "txtTeamName2",
                "txtPeriodCount",
                "txtPeriodLength",
                "dtScheduledTime",
                "chkPeriodOvertime",
                "chkMatchExtensionOnDraw"
            }
        },
        {
            "Other", new List<string>
            {
                "displaylist",
                "txtTeamName1",
                "txtTeamName2",
                "txtPeriodCount",
                "txtPeriodLength",
                "dtScheduledTime",
                "chkPeriodOvertime",
                "chkPeriodPauseNearEnd",
                "txtPeriodPauseNearEndSec",
                "chkMatchExtensionOnDraw"
            }
        },
    };

        // Executed when the selected sport changed
        private async Task SelectedSportChanged(string newSport)
        {
            if (TournamentId != null)
            {
                Logger.Info("Sport cannot being changed because the match will belong to a tournament.");
                return;
            }

            Logger.Debug("Sport changed to \"{0}\"", SelectedSport);
            NewMatch.GameName = newSport;

            if (SelectedSport == "")
                return;

            // Get the standard rules for that game
            var matchRules = DataHandler.GetGameRule(SelectedSport);

            if (matchRules == null)
                return;

            // Apply the standardrules to the new match
            NewMatch = new TblMatch()
            {
                GameName = SelectedSport,
                Team1Name = "Team1",
                Team2Name = "Team2",
                MatchStatus = (int)Enum.MatchStatusEnum.Planned,
                ScheduledTime = DateTime.Now
            };
            NewMatch.RulePeriodCount = matchRules.RulePeriodCount;
            NewMatch.RulePeriodLastPauseTimeOnEvent = matchRules.RulePeriodLastPauseTimeOnEvent;
            NewMatch.RulePeriodLastPauseTimeOnEventSeconds = matchRules.RulePeriodLastPauseTimeOnEventSeconds;
            NewMatch.RulePeriodLength = matchRules.RulePeriodLength;
            NewMatch.RulePeriodOvertime = matchRules.RulePeriodOvertime;
            NewMatch.RuleMatchExtensionOnDraw = matchRules.RuleMatchExtensionOnDraw;
            NewMatch.RulePenaltyList = matchRules.RulePenaltyList;

            // Set Default values based on rules
            NewMatch.RulePeriodCount = NewMatch.RulePeriodCount; //Note: RulePeriodCount cannot be null here because it is already set to 2 in case it is null above
            NewMatch.CurrentTimeLeft = NewMatch.RulePeriodLength;

            // Set the selected displays to the match
            foreach (var aDisplay in DisplayList)
            {
                if (aDisplay.Default)
                    NewMatch.Devices.Add(new TblDevice2Match() { Device = aDisplay });
            }


            await InvokeAsync(() => { StateHasChanged(); });
        }

        /// <summary>
        /// Exectued if the "Save"-Button is clicked or if called by other methods like SaveStartMatch()
        /// </summary>
        /// <returns>The DTO object of the new Match</returns>
        private async Task<TblMatch?> SaveMatch()
        {
            // Save and get the saved match as return value
            var newMatch = DataHandler.AddMatch(NewMatch);

            // Reset the NewMatch variable, that represents the current configuration of the match-to-add. Values are not required anymore after command before was executed.
            NewMatch = new TblMatch()
            {
                GameName = "",
                Team1Name = "Team1",
                Team2Name = "Team2",
                CurrentTimeLeft = 10 * 60,
                CurrentPeriod = 1,
                RulePeriodCount = 2,
                ScheduledTime = DateTime.Now
            };

            if (ReturnUrl != null)
            {
                NavigationManager.NavigateTo(ReturnUrl);
            }

            return newMatch;
        }

        /// <summary>
        /// Executed if the "Save and Start"-Button is clicked
        /// </summary>
        private async void SaveStartMatch()
        {
            var newMatch = await SaveMatch();

            if (newMatch == null)
                NavigationManager.NavigateTo("/matchcontrol");
            else
            {
                newMatch.MatchStatus = (int)Enum.MatchStatusEnum.ReadyToStart;
                //await DbContext.SaveChangesAsync();

                NavigationManager.NavigateTo("/matchcontrol/" + newMatch.Id);
            }
        }


        protected override async Task OnInitializedAsync()
        {
            Logger.Trace("Initializing the page");
            
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

            // Get query string parameters
            var queryStringParameters = new Uri(NavigationManager.Uri).Query;
            if (queryStringParameters.Length > 0)
            {
                queryStringParameters = queryStringParameters.Substring(1); //remove the leading "?"
                var parameterList = queryStringParameters.Split('&'); //split the parameters by "&". As a result you will have key=value for each of the resulting items
                foreach( var parameter in parameterList )
                {
                    if (parameter.ToLower().StartsWith("tournamentid="))
                    {
                        try
                        {
                            TournamentId = Convert.ToInt32(parameter.Split(new char[] { '=' }, 2)[1]);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, "Cannot parse the tournament ID parameter to integer.");
                        }
                    }
                    else if (parameter.ToLower().StartsWith("returnurl="))
                    {
                        try
                        {
                            ReturnUrl = parameter.Split(new char[] { '=' }, 2)[1];
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, "Cannot parse the tournament returnurl parameter.");
                        }
                    }
                }
            }
            
            // Get the enabled displays
            DisplayList = DisplayList.Where(x => x.Enabled).ToList();

            // Set default parameters if a tournament is given
            if (TournamentId != null)
            {
                var tournament = await DataHandler.GetTournamentAsync(TournamentId.Value);
                if (tournament != null)
                {
                    NewMatch.GameName = tournament.Sport;
                    NewMatch.Team1Name = tournament.DefaultTeam1Name;
                    NewMatch.Team2Name = tournament.DefaultTeam2Name;
                    NewMatch.RuleMatchExtensionOnDraw = tournament.DefaultRuleMatchExtensionOnDraw;
                    NewMatch.RulePenaltyList = tournament.DefaultRulePenaltyList;
                    NewMatch.RulePeriodCount = tournament.DefaultPeriodCount;
                    NewMatch.RulePeriodLastPauseTimeOnEvent = tournament.DefaultRulePeriodLastPauseTimeOnEvent;
                    NewMatch.RulePeriodLastPauseTimeOnEventSeconds = tournament.DefaultRulePeriodLastPauseTimeOnEventSeconds;
                    NewMatch.RulePeriodLength = tournament.DefaultRulePeriodLength;
                    NewMatch.RulePeriodOvertime = tournament.DefaultRulePeriodOvertime;
                    NewMatch.Tournament = tournament;

                    // Set the selected displays to the match
                    foreach (var aDisplay in tournament.Devices)
                    {
                        // If the tournaments profile contains an enabled Device, add it to the new match
                        if (DisplayList.Select(x => x.DeviceId).Contains(aDisplay.Device.DeviceId))
                            NewMatch.Devices.Add(new TblDevice2Match() { Device = aDisplay.Device });
                    }
                }
            }

            await InvokeAsync(() => { StateHasChanged(); });
        }


        /// <summary>
        /// Executed when a device in the device list is (un)checked
        /// </summary>
        /// <param name="device"></param>
        /// <param name="isChecked"></param>
        private async void DisplayClicked(TblDevice device, bool isChecked)
        {
            // The enabled property is not saved back to the server. So it is used here to save the check state
            DisplayList.Single(x => x.DeviceId == device.DeviceId).Enabled = isChecked;

            // Add the device to the match object
            if (isChecked)
                NewMatch.Devices.Add(new TblDevice2Match() { Device = device });
            else
            {
                var dev = NewMatch.Devices.SingleOrDefault(x => x.DeviceId == device.Id);
                if (dev != null)
                {
                    NewMatch.Devices.Remove(dev);
                }
            }
        }
    }
}
