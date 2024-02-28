using Microsoft.AspNetCore.SignalR;
using LeDi.Shared2.Display;
using Microsoft.CodeAnalysis.CSharp;
using LeDi.Shared2.DatabaseModel;

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
        /// The client asks for an update of the data. i.e. after the client was startet.
        /// </summary>
        /// <returns></returns>
        public async Task RequestMatch(int matchId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Match-" + matchId);

            // Remove the MatchEvents. They are not required and cause dependency Cycles.
            var match = await DataHandler.GetMatchAsync(matchId);
            match.MatchEvents = new List<TblMatchEvent>();
            match.Tournament.Matches = new List<TblMatch>(); //Reset Match to avoid dependency circles.
            match.Tournament.Template = new TblTemplate(); // Reset Template to avoid dependency circles.

            await Clients.Caller.SendAsync("ReceiveMatch", match);
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
