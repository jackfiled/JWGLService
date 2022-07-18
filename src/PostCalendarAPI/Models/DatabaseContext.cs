using Microsoft.EntityFrameworkCore;

namespace PostCalendarAPI.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {

        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserInfo> Users { get; set; }   
    }
}
