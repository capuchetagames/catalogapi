using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configuration;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("INT").ValueGeneratedNever().UseIdentityColumn();
        builder.Property(x => x.Name).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x => x.Category).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x => x.Active).HasColumnType("BIT").IsRequired();
        builder.Property(x => x.Price).HasColumnType("DECIMAL(7,4)").IsRequired();
        builder.Property(x=> x.CreatedAt).HasColumnType("DATETIME").IsRequired();
    }
}