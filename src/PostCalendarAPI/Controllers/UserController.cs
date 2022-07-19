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
    public class UsersController : ControllerBase
    {
        private IConfiguration _configuration;
        private DatabaseContext _context;
        private ILogger<UsersController> _logger;

        public UsersController(
            IConfiguration configuration, 
            DatabaseContext databaseContext, 
            ILogger<UsersController> logger)
        {
            _configuration = configuration;
            _context = databaseContext;
            _logger = logger;
        }

        [HttpGet("{studentID}")]
        [Authorize]
        public ActionResult GetUserInfo(int studentID)
        {
            var user = _context.Users.SingleOrDefault(u => u.StudentID == studentID);

            if(user == default)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserInfo>> CreateUser(UserInfo user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("student ${user.StudentId} created", user.StudentID);

            return CreatedAtAction(
                nameof(CreateUser),
                user
                );
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                _logger.LogInformation("Failed to delete {id}", id);
                return NotFound();
            }
            else
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Succeed in deleting {id}", id);
                return NoContent();
            }
        }

        [HttpGet("admin")]
        public ActionResult Admin(string userName, int studentID)
        {
            // 把管理员账号和密码写死在代码里有点愚蠢
            if(userName == "admin" && studentID == 123456)
            {
                var claims = new[]
                {
                    // 签发者
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    // 唯一标识
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    // 签发时间
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                    // 学号
                    new Claim("StudentID", studentID.ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: signIn
                    );

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("login")]
        public ActionResult Login(string userName, int studentID)
        {
            if(CheckUserExist(userName, studentID))
            {
                var claims = new[]
                {
                    // 签发者
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    // 唯一标识
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    // 签发时间
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                    // 学号
                    new Claim("StudentID", studentID.ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: signIn
                    );

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
            {
                _logger.LogInformation("{studentID} try to log in", studentID);
                return BadRequest("No such student");
            }
        }

        private bool CheckUserExist(string userName, int studentID)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(
                    u => u.UserName == userName && u.StudentID == studentID);

                if(user == default)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                // 捕获这个异常说明数据库存在相同的人
                _logger.LogWarning("The database exists two same students");
                return false;
            }
        }
    }
}
