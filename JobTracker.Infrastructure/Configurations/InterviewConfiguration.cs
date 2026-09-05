using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Configurations
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.ToTable("Interviews");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InterviewDate)
                .IsRequired();

            builder.Property(x => x.InterviewType)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.UpdatedBy);

            builder.HasOne(x => x.JobApplication)
                    .WithMany(x => x.Interviews)
                    .HasForeignKey(x => x.JobApplicationId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.JobApplicationId);
        }
    }
}
