using Microsoft.EntityFrameworkCore;
using JwglBackend.Models;
using JwglServices.Services.JWService;
using JwglServices.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add the database context to the service
builder.Services.AddDbContext<IcsInformationDbContext>(options =>
{
    string databaseName = builder.Configuration["IcsDatabase"];
    string databasePath = Path.Join(Directory.GetCurrentDirectory(), databaseName);

    options.UseSqlite($"Data Source={databasePath}");
});
builder.Services.AddDbContext<SemesterInfoContext>(options =>
{
    var databaseName = builder.Configuration["SemesterDatabase"];
    var databasePath = Path.Join(Directory.GetCurrentDirectory(), databaseName);

    options.UseSqlite($"Data Source={databasePath}");
});


builder.Services.AddScoped<IJWService, JWService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
