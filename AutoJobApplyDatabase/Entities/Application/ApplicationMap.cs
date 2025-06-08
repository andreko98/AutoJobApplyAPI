using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoJobApplyDatabase.Entities
{
    public class ApplicationMap : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("Applications");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.MessageSent)
                .HasMaxLength(2000);

            builder.Property(a => a.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(a => a.AppliedAt)
                .IsRequired();

            builder.HasOne(a => a.Job)
                .WithMany()
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}