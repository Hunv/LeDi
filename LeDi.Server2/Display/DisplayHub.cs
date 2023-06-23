using Microsoft.AspNetCore.SignalR;
using LeDi.Shared2.Display;
using Microsoft.CodeAnalysis.CSharp;

namespace LeDi.Server2.Display
{
    public class DisplayHub : Hub
    {
        public DisplayHub(IHubContext<DisplayHub> context)
        {
            DataHandler.hubContext = context;
        }

        /// <summary>
        /// Executed when a client wants to register by the client
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public async Task RegisterDevice(string deviceId, string deviceName, string deviceType, string deviceModel)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
            await DataHandler.RegisterDevice(deviceId, deviceName, deviceType, deviceModel);
        }
        
        /// <summary>
        /// The client asks for an update of the data. i.e. after the client was startet.
        /// </summary>
        /// <returns></returns>
        public async Task RequestUpdate(string deviceId)
        {            
            await Clients.Caller.SendAsync("ReceiveDataUpdate", DataHandler.GetDeviceSettingAsync(deviceId, "mode"));
        }

        /// <summary>
        /// Sends a command to a specifc Device
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task SendDeviceCommand(string command, string deviceId)
        {
            await DataHandler.hubContext.Clients.Group(deviceId).SendAsync("ReceiveCommand", command);
        }

        /// <summary>
        /// Sends a command to a specifc Device
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task SendDeviceCommandWithParameter(string command, string deviceId, string jsonParameter)
        {
            await DataHandler.hubContext.Clients.Group(deviceId).SendAsync("ReceiveCommand", command, jsonParameter);
        }
    }
}
