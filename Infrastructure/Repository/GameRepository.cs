using Core.Entity;
using Core.Repository;

namespace Infrastructure.Repository;

public class GameRepository : EfRepository<Game>, IGameRepository
{
    public GameRepository(ApplicationDbContext context) : base(context)
    {
    }
}