using LeDi.Server2.Display;
using LeDi.Shared2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Options;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LeDi.Server2.Pages
{
    public partial class SettingsDevice 
    {
        [Parameter]
        public string Id { get; set; } = "";

        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<TblDeviceSetting>? DeviceSettingList;
        private TblDevice? Device { get; set; }
        private TblDeviceSetting NewSetting = new TblDeviceSetting("", "", "");
        private string selectedDeviceCommand = "";

        protected override async Task OnInitializedAsync()
        {
            Device = await DataHandler.GetDeviceAsync(Id);

            if (Device == null || Device.DeviceId == "")
            {
                Logger.Error("No device with ID {0} found.", Id);
                return;
            }

            DeviceSettingList = await DataHandler.GetDeviceSettingListAsync(Id);
            NewSetting = new TblDeviceSetting(Id, "", "");
        }

        private async void SaveSettings()
        {
            if (DeviceSettingList == null)
            {
                Logger.Warn("Not devicesettings to save.");
                await JSRuntime.InvokeVoidAsync("alert", "No DeviceSettings to save.");
                return;
            }

            // Get the current serversettings to check which settings changed
            var currentServerSettings = await DataHandler.GetDeviceSettingListAsync(Id);
            if (currentServerSettings == null)
            {
                Logger.Error("Cannot get current device settings from server");
                await JSRuntime.InvokeVoidAsync("alert", "Cannot get current device settings from server.");
                return;
            }

            // Update all changed settings
            foreach (var aServerSet in currentServerSettings)
            {
                var locSet = DeviceSettingList.SingleOrDefault(x => x.SettingName == aServerSet.SettingName);

                Logger.Debug("Check changed setting {0} to value {1}", locSet == null ? "NULL" : locSet.SettingName, locSet == null ? "NULL" : locSet.SettingValue);

                if (locSet == null)
                    continue;

                if (locSet.SettingValue != aServerSet.SettingValue)
                {
                    Logger.Debug("Save setting {0} to value {1}", locSet.SettingName, locSet.SettingValue);

                    await DataHandler.SetDeviceSettingAsync(Id, locSet.SettingName, locSet.SettingValue);
                }
            }

            //Reload Device settings
            DeviceSettingList = await DataHandler.GetDeviceSettingListAsync(Id);
            await InvokeAsync(() => { StateHasChanged(); });
        }

        /// <summary>
        /// Save a new setting
        /// </summary>
        private async void SaveNewSetting()
        {
            await DataHandler.SetDeviceSettingAsync(Id, NewSetting.SettingName, NewSetting.SettingValue);

            DeviceSettingList = await DataHandler.GetDeviceSettingListAsync(Id);
            NewSetting = new TblDeviceSetting(Id, "", "");

            await InvokeAsync(() => { StateHasChanged(); });
        }

        /// <summary>
        /// Execute a device command on the selected remote device
        /// </summary>
        private async void ExecuteDeviceCommand()
        {
            if (string.IsNullOrWhiteSpace(selectedDeviceCommand))
                return;

            //Send command to device
            await DataHandler.hubContext.Clients.Group(Device.DeviceId).SendAsync("ReceiveCommand", selectedDeviceCommand);

            await InvokeAsync(() => { StateHasChanged(); });
        }

        /// <summary>
        /// Save changes at the device properties
        /// </summary>
        private async void SaveProperties()
        {
            if (Device == null)
                return;

            var dev = await DataHandler.GetDeviceAsync(Device.DeviceId);
            if (dev != null)
            {
                dev.DeviceName = Device.DeviceName;
                dev.DeviceModel = Device.DeviceModel;
                dev.DeviceType = Device.DeviceType;
                dev.Enabled = Device.Enabled;
                dev.Default = Device.Default;
            }
        }
    }
}
