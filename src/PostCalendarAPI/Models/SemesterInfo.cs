using Microsoft.EntityFrameworkCore;

namespace PostCalendarAPI.Models
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

    internal class Semester : IComparable<Semester>
    {
        public string SemesterString { get; set; }

        public DateTime BeginDateTime { get; set; }

        public Semester(SemesterInfo info)
        {
            SemesterString = info.Semester!;
            BeginDateTime = DateTime.Parse(info.BeginDateTimeString!);
        }

        public int CompareTo(Semester? other)
        {
            return BeginDateTime.CompareTo(other?.BeginDateTime);
        }

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
