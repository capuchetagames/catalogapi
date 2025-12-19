namespace Core.Entity;

public class PlayerLibraryGames : EntityBase
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    
    public virtual Game Game { get; set; }
}