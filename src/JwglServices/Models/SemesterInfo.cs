using Microsoft.EntityFrameworkCore;

namespace JwglServices.Models
{
    public class SemesterInfo
    {
        /// <summary>
        /// 数据库中的编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 学期
        /// </summary>
        public string? Semester { get; set; }
        /// <summary>
        /// 学期开始的时间
        /// </summary>
        public string? BeginDateTimeString { get; set; }
    }

    public class SemesterInfoContext : DbContext
    {
        public SemesterInfoContext()
        {
            Database.EnsureCreated();
        }

        public SemesterInfoContext(DbContextOptions<SemesterInfoContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<SemesterInfo> Semesters { get; set; } = null!;
    }
}
