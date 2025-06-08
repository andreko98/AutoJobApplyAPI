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

            builder.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Sobrenome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.DataNascimento)
                .IsRequired();

            builder.Property(u => u.Endereco)
                .HasMaxLength(300);

            builder.Property(u => u.Sobre)
                .HasMaxLength(2000);

            builder.Property(u => u.CurriculoPath)
                .HasMaxLength(500);
        }
    }
}