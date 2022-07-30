using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PostCalendarAPI.Models;

namespace PostCalendarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private SemesterInfoContext _context;
        private ILogger _logger;

        public SemesterController(SemesterInfoContext context, ILogger<SemesterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 通过学期字符串获得学期的开学时间
        /// </summary>
        /// <param name="semesterString">学期字符串</param>
        /// <returns>
        /// 200 学期的开学时间 
        /// 404 所查询的学期未开学或者课表未公布
        /// </returns>
        [HttpGet("{semesterString}")]
        public ActionResult GetBySemester(string semesterString)
        {
            var semester = _context.Semesters.SingleOrDefault(s => s.Semester == semesterString);

            if (semester == default)
            {
                return NotFound("学期课表未公布");
            }
            else
            {
                return Ok(semester);
            }
        }

        /// <summary>
        /// 获得当天所在的学期
        /// </summary>
        /// <returns>
        /// 200 学期字符串
        /// 400 
        /// </returns>
        [HttpGet("time")]
        public ActionResult GetTodaySemester()
        {
            String? result = GetSemester(DateTime.Now);

            if(result == null)
            {
                return BadRequest("学期课表未公布");
            }
            else
            {
                return Ok(result);
            }
        }

        /// <summary>
        /// 获得指定时间字符串所在的学期
        /// </summary>
        /// <param name="time">时间字符串</param>
        /// <returns>
        /// 200 学期字符串
        /// 400 
        /// </returns>
        [HttpGet("time/{time}")]
        public ActionResult GetSemester(String time)
        {
            DateTime dateTime = DateTime.Parse(time);

            String? result = GetSemester(dateTime);

            if(result == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateSemester(SemesterInfo info)
        {
            await _context.Semesters.AddAsync(info);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Semester {info.Semester} created", info.Semester);

            return CreatedAtAction(
                nameof(CreateSemester),
                info
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSemester(int id, SemesterInfo info)
        {
            if(id != info.ID)
            {
                return BadRequest();
            }
            else
            {
                _context.Entry(info).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!SemestersExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                _logger.LogInformation("Semester {info.Semester} Updated", info.Semester);
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var info = await _context.Semesters.FindAsync(id);

            if(info == null)
            {
                _logger.LogInformation("Try to delete {id} but not found", id);
                return NotFound();
            }
            else
            {
                _context.Semesters.Remove(info);
                await _context.SaveChangesAsync();

                _logger.LogInformation("{info.Semester} deleted", info.Semester);
                return NoContent();
            }
        }


        /// <summary>
        /// 判断数据库中是否存在指定的学期数据
        /// </summary>
        /// <param name="id">学期数据的编号</param>
        /// <returns></returns>
        private bool SemestersExists(int id)
        {
            return _context.Semesters.Any(s => s.ID == id);
        }

        /// <summary>
        /// 获取指定日期所在的学期
        /// </summary>
        /// <param name="time">指定的日期</param>
        /// <returns>学期字符串</returns>
        private String? GetSemester(DateTime time)
        {
            List<SemesterInfo> infos = _context.Semesters.ToList();

            List<Semester> semesters = new List<Semester>();

            foreach (var info in infos)
            {
                semesters.Add(new Semester(info));
            }

            // 遍历之前先按照时间顺序排序
            semesters.Sort();

            // 初始化为最后一个学期
            // 如果在遍历之后变量没有发生变化
            // 说明当前时间在所有学期开始时间之后
            int target = infos.Count - 1;
            for (int i = 0; i < infos.Count; i++)
            {
                if (time < semesters[i].BeginDateTime)
                {
                    target = i - 1;
                    break;
                }
            }

            if(target == -1)
            {
                return null;
            }
            else
            {
                return semesters[target].SemesterString;
            }
        }  
    }
}
