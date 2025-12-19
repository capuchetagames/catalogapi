using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configuration;

public class PlayerLibraryGamesConfiguration : IEntityTypeConfiguration<PlayerLibraryGames>
{
    public void Configure(EntityTypeBuilder<PlayerLibraryGames> builder)
    {
        builder.ToTable("PlayerLibraryGames");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("INT").ValueGeneratedNever().UseIdentityColumn();
        builder.Property(x => x.GameId).HasColumnType("INT").IsRequired();
        builder.Property(x => x.UserId).HasColumnType("INT").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("DATETIME").IsRequired();
        
        
        //builder.HasOne(x => x.Game).WithMany(x => x.Id).HasForeignKey(x => x.GameId);
    }
}