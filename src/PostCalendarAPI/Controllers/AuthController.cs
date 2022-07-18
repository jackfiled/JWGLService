using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using PostCalendarAPI.Models;

namespace PostCalendarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(UserInfo info)
        {
            if(info.UserName == null || info.PassWord == null)
            {
                // 如果名字或者密码为空
                // 直接返回400
                return BadRequest();
            }
            else
            {
                var result = UserAuthorize(info.UserName, info.PassWord);

                if(result)
                {
                    // JWT中的声明部分
                    var claims = new[]
                    {
                        // JWT所面向的用户
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        // JWT唯一的身份表示
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        // JWT签发时间
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        // 用户名称
                        new Claim("UserName", info.UserName),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn
                    );

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        private bool UserAuthorize(string userName, string password)
        {
            return userName == password;
        }
    }
}
