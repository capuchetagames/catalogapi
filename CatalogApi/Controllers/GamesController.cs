using System.Security.Claims;
using Core;
using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers;

/// <summary>
/// Gerencia as operações CRUD para os jogos da plataforma.
/// Utiliza autenticação distribuída via UserAPI.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _gameRepository;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(IGameRepository gameRepository, ICacheService cacheService, ILogger<GamesController> logger, IRabbitMqService rabbitMqService)
    {
        _gameRepository = gameRepository;
        _cacheService = cacheService;
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }

    /// <summary>
    /// Lista todos os jogos cadastrados.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna uma lista de todos os jogos.<br/>
    /// Os resultados são cacheados para melhorar a performance.<br/>
    /// Requer autenticação.
    /// </remarks>
    /// <returns>Uma lista de jogos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Game>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Get()
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation($"Usuário {username} acessando lista de jogos.");

            var gameListKey = "gameList";
            
            var cachedGameList = _cacheService.Get(gameListKey);

            if (cachedGameList != null)
            {
                return Ok(cachedGameList);
            }
            
            var gameList = _gameRepository.GetAll();
            
            if(gameList.Count > 0) 
                _cacheService.Set(gameListKey, gameList);
            
            _logger.LogInformation($"Retornados {gameList.Count} jogos para usuário {username}.");
            return Ok(gameList);
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao buscar lista de jogos: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Ocorreu um erro interno ao buscar os jogos.",
                error = e.Message
            });
        }
    }

    /// <summary>
    /// Busca um jogo específico pelo seu ID.
    /// </summary>
    /// <param name="id">O ID do jogo a ser buscado.</param>
    /// <remarks>
    /// Os resultados são cacheados individualmente.<br/>
    /// Requer autenticação.
    /// </remarks>
    /// <returns>O objeto do jogo correspondente ao ID.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Get([FromRoute] int id)
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation($"Usuário {username} buscando jogo ID: {id}");

            var gameKey = $"game-{id}";
            
            var cachedGame = _cacheService.Get(gameKey);
            
            if (cachedGame != null)
            {
                return Ok(cachedGame);
            }

            var game = _gameRepository.GetById(id);
            
            if (game == null)
            {
                return NotFound(new { message = $"Jogo com ID {id} não encontrado." });
            }
            
            _cacheService.Set(gameKey, game);
            
            _logger.LogInformation($"Jogo {id} ({game.Name}) retornado para usuário {username}");
            return Ok(game);
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao buscar jogo {id}: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Erro interno do servidor.",
                error = e.Message
            });
        }
    }

    /// <summary>
    /// Cria um novo jogo. Requer permissão de Admin.
    /// </summary>
    /// <param name="gameInput">Os dados do novo jogo a ser criado.</param>
    /// <returns>O jogo recém-criado.</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Game), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Post([FromBody] GameInput gameInput)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            // Verificar se o usuário tem permissão de Admin
            if (userRole != nameof(PermissionType.Admin))
            {
                _logger.LogWarning($"Usuário {username} tentou criar jogo sem permissão de Admin.");
                return Forbid("Acesso negado. Apenas administradores podem criar jogos.");
            }

            _logger.LogInformation($"Admin {username} criando novo jogo: {gameInput.Name}");

            var game = new Game()
            {
                Name = gameInput.Name,
                Category = gameInput.Category,
                Active = gameInput.Active,
                Price = gameInput.Price,
            };
            _gameRepository.Add(game);
            
            // Limpar cache da lista de jogos
            _cacheService.Remove("gameList");
            
            _logger.LogInformation($"Jogo {game.Name} (ID: {game.Id}) criado com sucesso pelo admin {username}");
            return CreatedAtAction(nameof(Get), new { id = game.Id }, game);
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao criar jogo: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Erro interno do servidor.",
                error = e.Message
            });
        }
    }

    /// <summary>
    /// Atualiza um jogo existente. Requer permissão de Admin.
    /// </summary>
    /// <param name="gameInput">Os dados do jogo a ser atualizado. O ID é obrigatório.</param>
    /// <returns>Nenhum conteúdo em caso de sucesso.</returns>
    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Put([FromBody] UpdateGameInput gameInput)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            // Verificar se o usuário tem permissão de Admin
            if (userRole != nameof(PermissionType.Admin))
            {
                _logger.LogWarning($"Usuário {username} tentou atualizar jogo {gameInput.Id} sem permissão de Admin.");
                return Forbid("Acesso negado. Apenas administradores podem atualizar jogos.");
            }

            _logger.LogInformation($"Admin {username} atualizando jogo ID: {gameInput.Id}");

            var game = _gameRepository.GetById(gameInput.Id);
            
            if (game == null)
            {
                return NotFound(new { message = $"Jogo com ID {gameInput.Id} não encontrado." });
            }
            
            game.Name = gameInput.Name;
            game.Category = gameInput.Category;
            game.Active = gameInput.Active;
            game.Price = gameInput.Price;
            
            _gameRepository.Update(game);
            
            // Limpar cache relacionado
            _cacheService.Remove($"game-{gameInput.Id}");
            _cacheService.Remove("gameList");
            
            _logger.LogInformation($"Jogo {game.Name} (ID: {game.Id}) atualizado com sucesso pelo admin {username}");
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao atualizar jogo {gameInput.Id}: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Erro interno do servidor.",
                error = e.Message
            });
        }
    }
    
    /// <summary>
    /// Deleta um jogo pelo ID. Requer permissão de Admin.
    /// </summary>
    /// <param name="id">O ID do jogo a ser deletado.</param>
    /// <returns>Nenhum conteúdo em caso de sucesso.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Delete([FromRoute] int id)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            // Verificar se o usuário tem permissão de Admin
            if (userRole != nameof(PermissionType.Admin))
            {
                _logger.LogWarning($"Usuário {username} tentou deletar jogo {id} sem permissão de Admin.");
                return Forbid("Acesso negado. Apenas administradores podem deletar jogos.");
            }

            _logger.LogInformation($"Admin {username} tentando deletar jogo ID: {id}");

            var game = _gameRepository.GetById(id);
            if (game == null)
            {
                return NotFound(new { message = $"Jogo com ID {id} não encontrado." });
            }
            
            _gameRepository.Delete(id);
            
            // Limpar cache relacionado
            _cacheService.Remove($"game-{id}");
            _cacheService.Remove("gameList");
            
            _logger.LogInformation($"Jogo {game.Name} (ID: {id}) deletado com sucesso pelo admin {username}");
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao deletar jogo {id}: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Ocorreu um erro interno.", 
                error = e.Message 
            });
        }
    }
    
    /// <summary>
    /// Cria uma ordem de compra 
    /// </summary>
    /// <param name="orderInput">Os dados de compra.</param>
    /// <returns>O jogo recém-criado.</returns>
    [HttpPost("/order-game")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(OrderInput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> OrderGame([FromBody] OrderInput orderInput)
    {
        try
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            Console.WriteLine($"userRole: {userRole} | username:  {username} | userId: {userId}");
            
            Console.WriteLine($"Cria Ordem de compra User: {orderInput.UserId} | Game:  {orderInput.GameId} ");
            
            var game = _gameRepository.GetById(orderInput.GameId);
            
            if (game == null)
            {
                return NotFound(new { message = $"Jogo com ID {orderInput.GameId} não encontrado." });
            }

            await _rabbitMqService.PublishAsync(
                "order.events",
                "order.ordered",
                new OrderPlacedEvent(orderInput.UserId, "teste", "TesTT", orderInput.GameId, game.Price),CancellationToken.None
            );
            
            
            //_logger.LogInformation($"Jogo {game.Name} (ID: {game.Id}) Criado Ordem de compra por: {username}");
            return CreatedAtAction(nameof(Get), new { id = game.Id }, orderInput);
        }
        catch (Exception e)
        {
            _logger.LogError($"Erro ao criar jogo: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                message = "Erro interno do servidor.",
                error = e.Message
            });
        }
    }

    /// <summary>
    /// Endpoint público para verificar status do serviço de catálogo.
    /// </summary>
    /// <returns>Status do serviço.</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "healthy", 
            service = "CatalogAPI - Games", 
            timestamp = DateTime.UtcNow 
        });
    }
    
    
}