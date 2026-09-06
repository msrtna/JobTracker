using JobTracker.Domain.Common;
using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Interview> Interviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(
                CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(
                        DateTime.UtcNow,
                        null);
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(
                        DateTime.UtcNow,
                        null);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
