using LeDi.Shared2.DatabaseModel;
using LeDi.Shared2.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LeDi.Server2.Pages
{
    public partial class MatchEdit
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private TblMatch? Match = null;
        private bool EditCollapsed = true;
        List<TblDevice> DisplayList = new List<TblDevice>(); //The list of devices

        [Parameter]
        public int? Id { get; set; }

        /// <summary>
        /// Optional parameter to define the URL to return after click on Save
        /// </summary>
        private string ReturnUrl { get; set; }

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
                "txtScoreTeam1",
                "txtScoreTeam2",
                "txtPeriodCount",
                "txtTimeLeft",
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
                "txtScoreTeam1",
                "txtScoreTeam2",
                "txtPeriodCount",
                "txtTimeLeft",
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
                "txtScoreTeam1",
                "txtScoreTeam2",
                "txtPeriodCount",
                "txtTimeLeft",
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
                "txtTimeLeft",
                "dtScheduledTime",
                "chkPeriodOvertime",
                "chkPeriodPauseNearEnd",
                "txtPeriodPauseNearEndSec",
                "chkMatchExtensionOnDraw"
            }
        },
    };

        private MatchStatusEnum MatchStatus
        {
            get
            {
                return Match == null ? MatchStatusEnum.Undefined : (MatchStatusEnum)Match.MatchStatus;
            }
            set
            {
                if (Match == null)
                    return;

                Match.MatchStatus = (int)value;
            }
        }

        private int TimeLeftMinutesProxy
        {
            get
            {
                if (Match == null)
                    return 0;

                return (int)(Match.CurrentTimeLeft / 60);
            }
            set
            {
                if (Match == null)
                    return;

                Match.CurrentTimeLeft = value * 60 + TimeLeftSecondsProxy;
            }
        }

        private int TimeLeftSecondsProxy
        {
            get
            {
                if (Match == null)
                    return 0;

                return (int)(Match.CurrentTimeLeft % 60);
            }
            set
            {
                if (Match == null)
                    return;

                Match.CurrentTimeLeft = TimeLeftMinutesProxy + value;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (Id == null)
            {
                Logger.Warn("Cannot load match. No Id given.");
                return;
            }

            Match = await DataHandler.GetMatchAsync(Id.Value);

            if (Match == null)
            {
                Logger.Warn("Cannot load Match ID {0}", Id.Value);
                return;
            }

            // Get query string parameters
            var queryStringParameters = new Uri(NavigationManager.Uri).Query;
            if (queryStringParameters.Length > 0)
            {
                queryStringParameters = queryStringParameters.Substring(1); //remove the leading "?"
                var parameterList = queryStringParameters.Split('&'); //split the parameters by "&". As a result you will have key=value for each of the resulting items
                foreach (var parameter in parameterList)
                {
                    if (parameter.ToLower().StartsWith("returnurl="))
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

            DisplayList = await DataHandler.GetDeviceListAsync();
            DisplayList = DisplayList.Where(x => x.Enabled).ToList();
            foreach (var aDev in DisplayList)
            {
                if (!Match.Devices.Any(x => x.Device?.DeviceId == aDev.DeviceId))
                {
                    aDev.Enabled = false; //After loading enabled reflects the selection state of the device
                }
            }

            await InvokeAsync(() => { StateHasChanged(); });
        }

        private async void SaveMatch()
        {
            if (Match == null)
            {
                Logger.Warn("Cannot save match. It is null.");
                return;
            }

            Logger.Debug("Saving match...");

            // Save the changes on Match variable
            await DataHandler.SetMatchAsync(Match);

            if (ReturnUrl != null)
            {
                NavigationManager.NavigateTo(ReturnUrl);
            }
        }

        /// <summary>
        /// Executed when a device in the device list is (un)checked
        /// </summary>
        /// <param name="device"></param>
        /// <param name="isChecked"></param>
        private async void DisplayClicked(TblDevice device, bool isChecked)
        {
            if (Match == null)
            {
                Logger.Warn("Cannot change devices. Match is null.");
                return;
            }

            // The enabled property is not saved back to the server. So it is used here to save the check state
            DisplayList.Single(x => x.DeviceId == device.DeviceId).Enabled = isChecked;

            // Add the device to the match object
            if (isChecked)
                Match.Devices.Add(new TblDevice2Match() { DeviceId = device.Id });
            else
            {
                var dev = Match.Devices.SingleOrDefault(x => x.Device?.DeviceId == device.DeviceId);
                if (dev != null)
                    Match.Devices.Remove(dev);
            }
        }
    }
}
