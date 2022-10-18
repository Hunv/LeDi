using LeDi.Server;
using LeDi.Server.DatabaseModel;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Logging;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    //Ensure database is created
    using (var dbContext = new TwDbContext())
    {
        var created = dbContext.Database.EnsureCreated();
        if (created) //created is true, if the database was just created
        {
            logger.Info("Create database...");
            if (dbContext.Settings != null)
            {
                dbContext.Settings.Add(new Setting("timezone", "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"));
                dbContext.Settings.Add(new Setting("wifi_password", ""));
                dbContext.SaveChanges();
            }
        }
    }

    // load not ended matches
    logger.Info("Caching matches...");
    MatchEngine.LoadRunningMatches();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}