using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoJobApplyDatabase.Entities
{
    public class JobMap : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Company)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Location)
                .HasMaxLength(200);

            builder.Property(j => j.Description)
                .HasMaxLength(4000);

            builder.Property(j => j.Url)
                .HasMaxLength(1000);

            builder.Property(j => j.DatePosted)
                .IsRequired();
        }
    }
}