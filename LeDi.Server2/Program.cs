using LeDi.Server2;
using LeDi.Server2.Data;
using LeDi.Server2.DatabaseModel;
using LeDi.Shared2.DatabaseModel;
using LeDi.Server2.Areas.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using LeDi.Server2.Pages;
using LeDi.Server2.Display;
using Microsoft.AspNetCore.SignalR;
using BootstrapBlazor;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

// Add services to the container.
// Identity services:
builder.Services.AddDbContext<LeDiDbContext>(options =>
    options.UseSqlite("Filename=LeDi.db"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LeDiDbContext>();

//To check: .AddDefaultUI()

builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddServerSideBlazor();
builder.Services.AddBootstrapBlazor(); //Blazor.Zone
builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
builder.Services.AddSingleton<MatchManagerService>();
//builder.Services.AddSingleton<MatchEngine>();

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
            dbContext.TblSettings.Add(new TblSetting("welcometitle", "Welcome to LeDi"));
            dbContext.TblSettings.Add(new TblSetting("welcometext", "See the currently ongoing matches at the dashboard or login to start managing the match, tournament or device."));

            dbContext.TblUserRoles.Add(new TblUserRole() { RoleName = "Role-Guests" });
            dbContext.TblUserRoles.Add(new TblUserRole() { RoleName = "Role-Administrators", CanDeviceCommands = true, CanDeviceManage = true, CanMatchAdd = true, CanMatchAdvancedControls = true, CanMatchDelete = true, CanMatchEdit = true, CanMatchEnd = true, CanMatchPenalty = true, CanMatchStart = true, CanMatchStop = true, CanPlayerAdd = true, CanPlayerDelete = true, CanPlayerEdit = true, CanRoleAdd = true, CanRoleDelete = true, CanRoleEdit = true, CanSettingManage = true, CanTeamAdd = true, CanTeamDelete = true, CanTeamEdit = true, CanTemplateManage = true, CanTournamentAdd = true, CanTournamentEdit = true, CanTournamentMatchAdd = true, CanTournamentMatchDelete = true, CanTournamentMatchEdit = true, CanUserAdd = true, CanUserDelete = true, CanUserEdit = true, CanUserPasswordEdit = true, IsAdmin = true });
            dbContext.TblUserRoles.Add(new TblUserRole() { RoleName = "Role-Referees", CanDeviceCommands = false, CanDeviceManage = false, CanMatchAdd = false, CanMatchAdvancedControls = true, CanMatchDelete = false, CanMatchEdit = false, CanMatchEnd = true, CanMatchPenalty = true, CanMatchStart = true, CanMatchStop = true, CanPlayerAdd = false, CanPlayerDelete = false, CanPlayerEdit = false, CanRoleAdd = false, CanRoleDelete = false, CanRoleEdit = false, CanSettingManage = false, CanTeamAdd = false, CanTeamDelete = false, CanTeamEdit = false, CanTemplateManage = false, CanTournamentAdd = false, CanTournamentEdit = false, CanTournamentMatchAdd = false, CanTournamentMatchDelete = false, CanTournamentMatchEdit = false, CanUserAdd = false, CanUserDelete = false, CanUserEdit = false, CanUserPasswordEdit = false, IsAdmin = false });
            dbContext.TblUserRoles.Add(new TblUserRole() { RoleName = "Role-Operators", CanDeviceCommands = true, CanDeviceManage = false, CanMatchAdd = true, CanMatchAdvancedControls = true, CanMatchDelete = true, CanMatchEdit = true, CanMatchEnd = true, CanMatchPenalty = true, CanMatchStart = true, CanMatchStop = true, CanPlayerAdd = true, CanPlayerDelete = true, CanPlayerEdit = true, CanRoleAdd = true, CanRoleDelete = true, CanRoleEdit = true, CanSettingManage = false, CanTeamAdd = true, CanTeamDelete = true, CanTeamEdit = true, CanTemplateManage = true, CanTournamentAdd = true, CanTournamentEdit = true, CanTournamentMatchAdd = true, CanTournamentMatchDelete = true, CanTournamentMatchEdit = true, CanUserAdd = true, CanUserDelete = true, CanUserEdit = true, CanUserPasswordEdit = true, IsAdmin = false });
            dbContext.SaveChanges();
        }
    }
}

// Start Display:


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
app.MapHub<DisplayHub>("/Display");

app.Run();
