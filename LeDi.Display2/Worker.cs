namespace LeDi.Display2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private Connector _connector = new Connector();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connector.Connect();

            while (!stoppingToken.IsCancellationRequested)
            {
                
                _connector.SendMessage();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}