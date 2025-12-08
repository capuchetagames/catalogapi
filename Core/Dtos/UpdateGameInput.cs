namespace Core.Dtos;

/// <summary>
/// Modelo de dados para a atualização de um jogo existente.
/// </summary>
public class UpdateGameInput
{
    /// <summary>
    /// O ID do jogo que será atualizado.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// O novo nome do jogo.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// A nova categoria ou gênero do jogo.
    /// </summary>
    public string Category { get; set; }
    
    /// <summary>
    /// O novo status de ativação do jogo.
    /// </summary>
    public bool Active { get; set; }
    
    /// <summary>
    /// O novo preço do jogo.
    /// </summary>
    public decimal Price { get; set; }
}