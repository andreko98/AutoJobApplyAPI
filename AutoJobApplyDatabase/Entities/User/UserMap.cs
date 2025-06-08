using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoJobApplyDatabase.Entities
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.DateOfBirth)
                .IsRequired();

            builder.Property(u => u.Address)
                .HasMaxLength(300);

            builder.Property(u => u.About)
                .HasMaxLength(2000);

            builder.Property(u => u.CvPath)
                .HasMaxLength(500);

            builder.Property(u => u.EmailCredentialId);

            builder.HasOne(u => u.EmailCredential)
                .WithOne(e => e.User)
                .HasForeignKey<EmailCredential>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}