using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoJobApplyDatabase.Entities
{
    public class ExternalApiKeyMap : IEntityTypeConfiguration<ExternalApiKey>
    {
        public void Configure(EntityTypeBuilder<ExternalApiKey> builder)
        {
            builder.ToTable("ExternalApiKeys");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Provider)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.ApiKey)
                .IsRequired();
        }
    }
}