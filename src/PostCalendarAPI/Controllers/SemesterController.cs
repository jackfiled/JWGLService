using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PostCalendarAPI.Models;

namespace PostCalendarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private DatabaseContext _context;
        private ILogger _logger;

        public SemesterController(DatabaseContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var semester = _context.Semesters.SingleOrDefault(s => s.ID == id);

            if(semester == default)
            {
                return NotFound();
            }
            else
            {
                return Ok(semester);
            }
        }

        [HttpGet("{semester}")]
        public ActionResult GetBySemester(string semesterString)
        {
            var semester = _context.Semesters.SingleOrDefault(s => s.Semester == semesterString);

            if(semester == default)
            {
                return NotFound();
            }
            else
            {
                return Ok(semester);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateSemester(SemesterInfo info)
        {
            await _context.Semesters.AddAsync(info);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Semester ${info.Semester} created", info.Semester);

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

                _logger.LogInformation("Semester ${info.Semester} Updated", info.Semester);
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var info = await _context.Semesters.FindAsync(id);

            if(info == null)
            {
                _logger.LogInformation("Try to delete ${id} but not found", id);
                return NotFound();
            }
            else
            {
                _context.Semesters.Remove(info);
                await _context.SaveChangesAsync();

                _logger.LogInformation("${info.Semester} deleted", info.Semester);
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
    }
}
