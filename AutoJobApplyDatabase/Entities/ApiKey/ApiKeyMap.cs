using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoJobApplyDatabase.Entities
{
    public class ApiKeyMap : IEntityTypeConfiguration<ApiKey>
    {
        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.ToTable("ApiKeys");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Provider)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Key)
                .IsRequired()
                .HasMaxLength(1000);
        }
    }
}