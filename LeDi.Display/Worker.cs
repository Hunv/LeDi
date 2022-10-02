using LeDi.Display.Display;

namespace LeDi.Display
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
            var layout = await connector.GetDeviceSettings();

            if (layout == null)
            {
                Console.WriteLine("Unable to load layout.");
                return;
            }

            var displayManager = new DisplayManager(layout, connector);

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}