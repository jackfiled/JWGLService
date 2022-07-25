using Microsoft.EntityFrameworkCore;

namespace PostCalendarAPI.Models
{
    /// <summary>
    /// 用户信息类
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户的唯一标识
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户的名称
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 用户的学号
        /// </summary>
        public int StudentID { get; set; }
        /// <summary>
        /// 用户的班号
        /// </summary>
        public int ClassNumber { get; set; }

    }

    public class UserInfoContext : DbContext
    {
        public UserInfoContext()
        {
            Database.EnsureCreated();
        }

        public UserInfoContext(DbContextOptions<UserInfoContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserInfo> Users { get; set; } = null!;
    }
}
