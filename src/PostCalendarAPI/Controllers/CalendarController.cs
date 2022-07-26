using Microsoft.AspNetCore.Mvc;

using PostCalendarAPI.Models;
using PostCalendarAPI.Services.JWService;

namespace PostCalendarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICSInfoContext _context;
        private readonly IJWService _jWService;
        
        public CalendarController(
            ILogger<CalendarController> logger,
            ICSInfoContext context,
            IJWService jWService
            )
        {
            _logger = logger;
            _context = context;
            _jWService = jWService;
        }

        [HttpPost]
        public async Task<ActionResult> Login(JWLoginModel model)
        {
            var ics = _context.ICSInfos.SingleOrDefault(i => i.UserName == model.username);

            // 先判断该用户是否已经请求过
            // 如果在六小时内请求过则拒绝请求
            // 在测试时先取消这个限制
            // 生产环境取消注释
            if (ics != default)
            {
                DateTime createDateTime = DateTime.Parse(ics.CreatedDateTimeString);
                TimeSpan span = DateTime.Now - createDateTime;

                if (span >= new TimeSpan(24, 0, 0))
                {
                    _logger.LogInformation("User {username} reject", model.username);
                    return BadRequest($"请求过于频繁, 请在{span}后再试");
                }
            }

            if (await _jWService.Login(model.username, model.password))
            {
                _logger.LogInformation("User {username} log in", model.username);

                await _jWService.GetSemester(model.semester);

                byte[]? stream = await _jWService.GetICSStream();

                if(stream == null)
                {
                    _logger.LogWarning("User {username} ICS stream generate failed", model.username);
                }
                else
                {
                    if(ics == default)
                    {
                        // 如果在数据库中不存在就创建
                        ICSInfo info = new ICSInfo(DateTime.Now, stream, model.username, model.semester);

                        _context.ICSInfos.Add(info);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // 在数据库中存在就更新
                        ics.Data = stream;
                        ics.CreatedDateTimeString = DateTime.Now.ToString("s");

                        await _context.SaveChangesAsync();
                    }
                }

                var courses = _jWService.GetCourses();
                if(courses == null)
                {
                    _logger.LogWarning("User {username} get courses failed", courses);
                    return BadRequest("获取课程失败");
                }
                else
                {
                    return Ok(courses);
                }

            }
            else
            {
                _logger.LogInformation("User {username} log failed", model.username);
                return BadRequest("登录失败，请检查账号或者密码");
            }
        }

        [HttpGet("{username}")]
        public ActionResult GetUserSemesters(string username)
        {
            var infos = _context.ICSInfos
                .Where(i => i.UserName == username)
                .ToList();

            return Ok(infos);
        }

        [HttpGet("{username}/{semester}")]
        public ActionResult GetICSStream(string username, string semester)
        {
            var info = _context.ICSInfos.SingleOrDefault(
                i => i.UserName == username && i.Semester == semester);

            if(info == default)
            {
                _logger.LogInformation("User {username} get {semester} ICS file failed", username, semester);
                return BadRequest("请先登录");
            }
            else
            {
                _logger.LogInformation("User {username} get {semester} ICS success", username, semester);

                return File(info.Data, "text/calendar");
            }
        }
    }
}
