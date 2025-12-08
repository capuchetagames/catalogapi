namespace Core.Entity;

/// <summary>
/// Representa um jogo na plataforma.
/// </summary>
public class Game : EntityBase
{
    /// <summary>
    /// O nome principal do jogo.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// A categoria ou gênero do jogo.
    /// </summary>
    public required string Category { get; set; }
    
    /// <summary>
    /// Indica se o jogo está ativo e disponível na plataforma.
    /// </summary>
    public bool Active { get; set; }
    
    /// <summary>
    /// O preço de compra do jogo.
    /// </summary>
    public decimal Price { get; set; }
    
    //public virtual ICollection<Order> Orders { get; set; }
}