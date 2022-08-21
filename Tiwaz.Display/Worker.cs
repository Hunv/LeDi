using Tiwaz.Display.Api;
using Tiwaz.Display.Display;

namespace Tiwaz.Display
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connector = new Connector();
            await connector.LoadLocalDeviceConfigAsync();
            await connector.RegisterDevice();
            var displayManager = new DisplayManager(connector.Layout);

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}