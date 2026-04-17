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
        builder.Property(x => x.Id).HasColumnType("INTEGER").UseIdentityColumn();
        builder.Property(x => x.Name).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x => x.Category).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x => x.Active).HasColumnType("BOOLEAN").IsRequired();
        builder.Property(x => x.Price).HasColumnType("DECIMAL(7,4)").IsRequired();
        builder.Property(x=> x.CreatedAt).HasColumnType("TIMESTAMP").IsRequired();
    }
}