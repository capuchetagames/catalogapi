using Core.Entity;
using Core.Repository;

namespace Infrastructure.Repository;

public class PlayerLibraryGamesRepository : EfRepository<PlayerLibraryGames> , IPlayerLibraryGames
{
    public PlayerLibraryGamesRepository(ApplicationDbContext context) : base(context)
    {
    }
}