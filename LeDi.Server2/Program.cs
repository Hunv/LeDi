using LeDi.Server2;
using LeDi.Server2.Data;
using LeDi.Server2.DatabaseModel;
using LeDi.Server2.Areas.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using LeDi.Server2.Pages;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

// Add services to the container.
// Identity services:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
builder.Services.AddSingleton<MatchManagerService>();
builder.Services.AddSingleton<MatchEngine>();
builder.Services.AddSingleton<CircuitHandler>(new CircuitHandlerService());

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

//Ensure database is created
using (var dbContext = new LeDiDbContext())
{
    var created = dbContext.Database.EnsureCreated();
    if (created) //created is true, if the database was just created
    {
        logger.Info("Create database...");
        if (dbContext.TblSettings != null)
        {
            dbContext.TblSettings.Add(new TblSetting("timezone", "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"));
            dbContext.TblSettings.Add(new TblSetting("wifi_password", ""));
            dbContext.TblSettings.Add(new TblSetting("eventtournamentid", ""));
            dbContext.SaveChanges();
        }
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
