using Microsoft.EntityFrameworkCore;

#nullable disable

namespace JwglBackend.Models
{
    public class IcsInformation
    {
        public int ID { get; set; }
        public string StudentID { get; set; }
        public string Semester { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] Data { get; set; }
    }

    public class IcsInformationDbContext : DbContext
    {
        public DbSet<IcsInformation> Informations { get; set; }

        public IcsInformationDbContext(DbContextOptions<IcsInformationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
