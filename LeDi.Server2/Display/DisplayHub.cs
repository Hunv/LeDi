using Microsoft.AspNetCore.SignalR;
using LeDi.Shared2.Display;

namespace LeDi.Server2.Display
{
    public class DisplayHub : Hub
    {
        public async Task SendMessage(string user, string message)
            => await Clients.All.SendAsync("ReceiveArea", new Area() { Width = 10, Height = 16});

        public async Task SendMessageToCaller(string user, string message)
            => await Clients.Caller.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToGroup(string user, string message)
            => await Clients.Group("Displays").SendAsync("ReceiveMessage", user, message);




        /// <summary>
        /// Executed when a client wants to register
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public async Task RegisterDevice(string deviceId, string deviceName, string deviceType, string deviceModel)
        {
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
        /// Sends the Mode for the Display to all Displays.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task SendModeAll(string mode)
            => await Clients.All.SendAsync("ReceiveMode", mode);

        /// <summary>
        /// Sends the Mode for a specific Device
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public async Task SendModeDevice(string mode, string connectionId)
            => await Clients.Client(connectionId).SendAsync("ReceiveMode", mode);






















        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Displays");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
