using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

using PostCalendarAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Jwt Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        };
    });

// Add the database
builder.Services.AddDbContext<UserInfoContext>(options =>
{
    string databaseName = builder.Configuration["Sqlite:UserInfoLocation"];
    string databasePath = Path.Join(Directory.GetCurrentDirectory(), databaseName);

    options.UseSqlite($"Data Source={databasePath}");
});
builder.Services.AddDbContext<SemesterInfoContext>(options =>
{
    string databaseName = builder.Configuration["Sqlite:SemesterInfoLocation"];
    string databasePath = Path.Join(Directory.GetCurrentDirectory(), databaseName);

    options.UseSqlite($"Data Source={databasePath}");
});
builder.Services.AddDbContext<ICSInfoContext>(options =>
{
    string databaseName = builder.Configuration["Sqlite:ICSInfoLocation"];
    string databasePath = Path.Join(Directory.GetCurrentDirectory(), databaseName);

    options.UseSqlite($"Data Source={databasePath}");
});

var app = builder.Build();

// 先认证
app.UseAuthentication();
// 再授权
app.UseAuthorization();

app.MapControllers();

app.Run();
