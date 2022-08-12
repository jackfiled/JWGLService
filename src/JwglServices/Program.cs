using Microsoft.EntityFrameworkCore;
using JwglServices.Models;
using JwglServices.Services;
using JwglServices.Services.JWService;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

// Add the Entity Framework Service
builder.Services.AddDbContext<SemesterInfoContext>(options =>
{
    var databaseName = builder.Configuration["sqlite:semester"];
    var databasePath = Path.Join(Directory.GetCurrentDirectory(), databaseName);

    options.UseSqlite($"Data Source={databasePath}");
});

// Add the jwgl Service
builder.Services.AddScoped<IJWService, JWService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<JwglService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
