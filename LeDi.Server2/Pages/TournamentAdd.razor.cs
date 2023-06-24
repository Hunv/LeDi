using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LeDi.Server2.Pages
{
    public partial class TournamentAdd
    {
        /// <summary>
        /// The tournament that will be created when click on save
        /// </summary>
        private TblTournament ToSaveTournament { get; set; } = new TblTournament();

        private List<TblDevice> DisplayList { get; set; } = new List<TblDevice>();

        /// <summary>
        /// Dictionary that defines the fields that are shown when SelectedGamename has the value of that match type.
        /// </summary>
        private readonly Dictionary<string, List<string>> FieldList = new Dictionary<string, List<string>>
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

        /// <summary>
        /// Is the rules section shown?
        /// </summary>
        public bool HideRules { get; set; } = true;

        /// <summary>
        /// Is the given ID edited instead of adding a new tournament?
        /// </summary>
        [Parameter]
        public int? EditId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DisplayList = await DataHandler.GetDeviceListAsync();

            if (EditId != null)
            {
                var nt = await DataHandler.GetTournamentAsync(EditId.Value);
                if (nt != null)
                {
                    ToSaveTournament = nt;
                }
            }
        }



        /// <summary>
        /// Represents the full minutes left of the NewMatch-Object
        /// </summary>
        private int PeriodLengthMinutesProxy
        {
            get
            {
                if (ToSaveTournament == null)
                    return 0;

                return (int)(ToSaveTournament.DefaultRulePeriodLength / 60);
            }
            set
            {
                if (ToSaveTournament == null)
                    return;

                ToSaveTournament.DefaultRulePeriodLength = value * 60 + PeriodLengthSecondsProxy;
            }
        }

        /// <summary>
        /// Represents the seconds only left of the NewMatch-Object
        /// </summary>
        private int PeriodLengthSecondsProxy
        {
            get
            {
                if (ToSaveTournament == null)
                    return 0;

                return (int)(ToSaveTournament.DefaultRulePeriodLength % 60);
            }
            set
            {
                if (ToSaveTournament == null)
                    return;

                ToSaveTournament.DefaultRulePeriodLength = PeriodLengthMinutesProxy + value;
            }
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
            {
                ToSaveTournament.Devices.Add(new TblDevice2Tournament() { Device = device });
            }
            else
            {
                var dev = ToSaveTournament.Devices.SingleOrDefault(x => x.DeviceId == device.Id);
                if (dev != null)
                {
                    ToSaveTournament.Devices.Remove(dev);
                }
            }
        }


        /// <summary>
        /// Exectued if the "Save"-Button is clicked
        /// </summary>
        /// <returns>The DTO object of the new Match</returns>
        private async Task<TblTournament?> SaveTournament()
        {
            TblTournament? savedTournament = null;
            if (EditId.HasValue)
            {
                var tour = await DataHandler.SetTournamentAsync(ToSaveTournament);
                if (tour != null) {
                    savedTournament = tour;
                }
            }
            else
            {
                var tour = await DataHandler.AddTournamentAsync(ToSaveTournament);
                if (tour != null)
                {
                    // Save and get the saved match as return value
                    savedTournament = tour;
                }
            }

            // Reset the NewMatch variable, that represents the current configuration of the match-to-add. Values are not required anymore after command before was executed.
            ToSaveTournament = new TblTournament()
            {
                Sport = "",
                DefaultTeam1Name = "Team1",
                DefaultTeam2Name = "Team2",
                DefaultRulePeriodLength = 10 * 60,
                DefaultPeriodCount = 2
            };

            NavigationManager.NavigateTo("/tournamentplanning");

            return savedTournament;
        }

        /// <summary>
        /// Shows or hides the rules section
        /// </summary>
        /// <returns></returns>
        private async Task ExpandRules()
        {
            HideRules = !HideRules;

            await InvokeAsync(() => { StateHasChanged(); });
        }
    }
}
