namespace Core.Dtos;

/// <summary>
/// Modelo de dados para a criação de um novo jogo.
/// </summary>
public class GameInput
{
    /// <summary>
    /// O nome principal do jogo.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// A categoria ou gênero do jogo.
    /// </summary>
    public string Category { get; set; }
    
    /// <summary>
    /// Define se o jogo já deve ser criado como ativo.
    /// </summary>
    public bool Active { get; set; }
    
    /// <summary>
    /// O preço de compra do jogo.
    /// </summary>
    public decimal Price { get; set; }
}