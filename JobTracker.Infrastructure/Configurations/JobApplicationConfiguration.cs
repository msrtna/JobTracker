using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Configurations
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.ToTable("JobApplications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Location)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Position)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Salary)
                .HasPrecision(18,2);

            builder.Property(x => x.WorkPlace)
                .IsRequired();

            builder.Property(x => x.JobUrl)
                .HasMaxLength(200);

            builder.Property(x => x.ApplicationDate);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.UpdatedBy);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CompanyId);
            builder.HasIndex(x => x.JobCategoryId);
        }
    }
}
