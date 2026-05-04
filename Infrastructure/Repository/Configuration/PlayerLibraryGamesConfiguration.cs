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
        builder.Property(x => x.Id).HasColumnType("INTEGER").UseIdentityColumn();
        builder.Property(x => x.GameId).HasColumnType("INTEGER").IsRequired();
        builder.Property(x => x.UserId).HasColumnType("INTEGER").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("TIMESTAMP").IsRequired();
        
        
        //builder.HasOne(x => x.Game).WithMany(x => x.Id).HasForeignKey(x => x.GameId);
    }
}