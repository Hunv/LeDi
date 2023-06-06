using LeDi.Display2.Display;
using LeDi.Display2.Events;
using LeDi.Shared2.DatabaseModel;
//using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SignalR Client: https://learn.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-6.0&tabs=visual-studio

namespace LeDi.Display2
{
    internal class Connector
    {
        HubConnection connection;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Connector(string ServerUrl) {
            if (string.IsNullOrWhiteSpace(ServerUrl))
            {
                Logger.Error("ServerURL property is empty. Set a ServerURL before starting the application. The following error is expected.");

                //Create a fake connection
                connection = new HubConnectionBuilder().WithUrl("").Build();
                return;
            }

            connection = new HubConnectionBuilder()
                .WithUrl("https://" + ServerUrl + ":7168/Display")
                .WithAutomaticReconnect() //Automatically reconnect after 0, 2, 10 and 30 seconds.
                .Build();

            // Setup automatic reconnect on connection lost if reconnect fails.
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            Logger.Info("Connector initialized");
        }

        // Events
        public event EventHandler<DataUpdateEventArgs> DataUpdateReceived; // Triggered when the server sends data updates.

        /// <summary>
        /// Connect to the server
        /// </summary>
        public async void Connect()
        {
            connection.On<string>("ReceiveDataUpdate", (mode) =>
            {
                DataUpdateReceived?.Invoke(this, new DataUpdateEventArgs() { Mode = mode });
                Logger.Info("DeviceMode set to {0}", mode);
            });

            try
            {
                await connection.StartAsync();
                Logger.Info("Connected");
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "Failed to connect to Server");                
            }
        }

        /// <summary>
        /// Trigger register at the server
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public async Task RegisterDevice(string deviceId, string deviceName, string deviceType, string deviceModel)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                Logger.Warn("Cannot request update. Connection to server not established.");
                return;
            }

            await connection.InvokeAsync("RegisterDevice", deviceId, deviceName, deviceType, deviceModel);
        }

        /// <summary>
        /// Get the current data to show from the server
        /// </summary>
        /// <returns></returns>
        public async Task RequestUpdate(string deviceId)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                Logger.Warn("Cannot request update. Connection to server not established.");
                return;
            }

            await connection.InvokeAsync("RequestUpdate", deviceId);
        }
    }
}
