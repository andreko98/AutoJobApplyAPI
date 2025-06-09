using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoJobApplyDatabase.Entities
{
    public class EmailLogMap : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.ToTable("EmailLogs");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FromEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.ToEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Subject)
                .HasMaxLength(300);

            builder.Property(e => e.Body)
                .HasMaxLength(4000);

            builder.Property(e => e.AttachmentPath)
                .HasMaxLength(500);

            builder.Property(e => e.SentAt)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.Message)
                .HasMaxLength(1000);
        }
    }
}