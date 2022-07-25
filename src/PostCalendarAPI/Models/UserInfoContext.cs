using Microsoft.EntityFrameworkCore;

namespace PostCalendarAPI.Models
{
    public class UserInfoContext : DbContext
    {
        public UserInfoContext()
        {

        }

        public UserInfoContext(DbContextOptions<UserInfoContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserInfo> Users { get; set; } = null!;
    }
}
