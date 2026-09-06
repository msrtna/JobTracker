using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(x => x.Name)
                .IsUnique();

            builder.Property(x => x.Website)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(x => x.Website)
                .IsUnique();

            builder.Property(x => x.Location)
                .HasMaxLength(200);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.UpdatedBy);

            builder.HasMany(x => x.JobApplications)
                    .WithOne(x => x.Company)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
