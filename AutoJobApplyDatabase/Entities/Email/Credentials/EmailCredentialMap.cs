using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoJobApplyDatabase.Entities
{
    public class EmailCredentialsMap : IEntityTypeConfiguration<EmailCredential>
    {
        public void Configure(EntityTypeBuilder<EmailCredential> builder)
        {
            builder.ToTable("EmailCredentials");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.EncryptedPassword)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(e => e.UserId);

            builder.HasOne(e => e.User)
                .WithOne(u => u.EmailCredential)
                .HasForeignKey<User>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}