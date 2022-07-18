using System.IO;
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
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    string databaseName = builder.Configuration["Sqlite:Location"];
    string databasePath = Path.Join(Directory.GetCurrentDirectory(), databaseName);

    /*// 判断数据库文件是否存在
    if(!File.Exists(databasePath))
    {
        var stream = File.Create(databasePath);
        stream.Dispose();
    }*/

    options.UseSqlite($"Data Source={databasePath}");
});

var app = builder.Build();

app.UseHttpsRedirection();

// 先认证
app.UseAuthentication();
// 再授权
app.UseAuthorization();

app.MapControllers();

app.Run();
