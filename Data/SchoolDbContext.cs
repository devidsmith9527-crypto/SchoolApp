using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;

namespace SchoolApp.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        // តំណាងឱ្យតារាង Students នៅក្នុង SQL Server
        public DbSet<Student> Students { get; set; }
    }
}