using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

using JwglServices.Services.JWService;
using JwglBackend.Models;
using NPOI.OpenXmlFormats.Dml;

namespace JwglBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IJWService _jwService;
        private readonly IcsInformationDbContext _icsInformations;

        public CalendarController(IJWService jwService, IcsInformationDbContext icsInformations)
        {
            _jwService = jwService;
            _icsInformations = icsInformations;
        }

        [HttpPost("get-semester")]
        public async Task<ActionResult> GetSemester(GetSemesterModel model)
        {
            IcsInformation? information = _icsInformations.Informations.SingleOrDefault(
                item => item.StudentID == model.StudentID && item.Semester == model.Semester);

            if (information != default)
            {
                // 如果在数据库中存在记录

                // 判断距离上次更新的时间
                TimeSpan span = DateTime.Now - information.UpdatedAt;
                // TODO
                
            }

            bool result = await _jwService.Login(model.StudentID, model.Password);

            if (!result)
            {
                // 如果登录失败
                return BadRequest(new ErrorModel("登录错误，请检查账号和密码"));
            }

            await _jwService.GetSemester(model.Semester);
            Task<byte[]?> futureStream = _jwService.GetICSStream();
            List<JwglServices.Models.CourseInfo>? courses = _jwService.GetCourses()?.ToList();
            byte[]? stream = await futureStream;

            if (courses == null || stream == null)
            {
                return BadRequest(new ErrorModel("获取课表错误，请选择正确的学期"));
            }
            else
            {
                if (information == default)
                {
                    IcsInformation newInformation = new IcsInformation();
                    newInformation.StudentID = model.StudentID;
                    newInformation.Semester = model.Semester;
                    newInformation.UpdatedAt = DateTime.Now;
                    newInformation.Data = stream;

                    _icsInformations.Add(newInformation);
                }
                else
                {
                    information.UpdatedAt = DateTime.Now;
                    information.Data = stream;
                }
                _icsInformations.SaveChanges();

                return Ok(courses);
            }
        }

        [HttpGet("{id}/{semester}.ics")]
        public ActionResult GetICSFile(string id, string semester)
        {
            Console.WriteLine(id + semester);
            IcsInformation? information = _icsInformations.Informations.FirstOrDefault(
                item => item.StudentID == id && item.Semester == semester);

            if (information == default)
            {
                return NotFound(new ErrorModel("请先登录获取课程"));
            }
            else
            {
                return File(information.Data, "text/calendar");
            }
        }
    }
}
