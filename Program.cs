using Tiwaz.Server.DatabaseModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Ensure database is created
using (var dbContext = new TwDbContext())
{    
    var created = dbContext.Database.EnsureCreated();
    if (created) //created is true, if the database was just created
    {
        dbContext.Settings.Add(new Setting() { SettingName = "timezone", SettingValue = "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna" });
        dbContext.SaveChanges();
    }
}

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
