using DapperCRUDSample.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DapperCRUDSample.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Student> Students { get; set; }
    }
}
