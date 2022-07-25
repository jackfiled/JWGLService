using Microsoft.EntityFrameworkCore;

namespace PostCalendarAPI.Models
{
    public class SemesterInfoContext : DbContext
    {
        public SemesterInfoContext()
        {

        }

        public SemesterInfoContext(DbContextOptions<SemesterInfoContext> options)
        {
            Database.EnsureCreated();
        }

        public DbSet<SemesterInfo> Semesters { get; set; } = null!;
    }
}
