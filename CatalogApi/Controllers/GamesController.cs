using Core;
using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudGamesApi.Controllers;

/// <summary>
/// Gerencia as operações CRUD para os jogos da plataforma.
/// </summary>
[ApiController]
[Route("/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _gameRepository;
    private readonly ICacheService _cacheService;
    public GamesController(IGameRepository gameRepository, ICacheService cacheService)
    {
        _gameRepository = gameRepository;
        _cacheService = cacheService;
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
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Game>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Get()
    {
        try
        {
            var gameListKey = "gameList";
            
            var cachedGameList = _cacheService.Get(gameListKey);

            if (cachedGameList != null)
            {
                return Ok(cachedGameList);
            }
            
            var gameList = _gameRepository.GetAll();
            
            if(gameList.Count>0) _cacheService.Set(gameListKey, gameList);
            
            return Ok(gameList);
        }
        catch (Exception e)
        {
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
    /// <param name="id">O ID (int) do jogo a ser buscado.</param>
    /// <remarks>
    /// Os resultados são cacheados individualmente.<br/>
    /// Requer autenticação.
    /// </remarks>
    /// <returns>O objeto do jogo correspondente ao ID.</returns>
    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult Get([FromRoute] int id)
    {
        try
        {
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
            
            return Ok(game);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    /// <summary>
    /// Cria um novo jogo.
    /// </summary>
    /// <remarks>
    /// Apenas usuários com a política 'Admin' podem criar jogos.
    /// </remarks>
    /// <param name="gameInput">Os dados do novo jogo a ser criado.</param>
    /// <returns>O jogo recém-criado.</returns>
    [HttpPost]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    [Consumes("application/json")]
    public IActionResult Post([FromBody] GameInput gameInput)
    {
        try
        {
            var game = new Game()
            {
                Name = gameInput.Name,
                Category = gameInput.Category,
                Active = gameInput.Active,
                Price = gameInput.Price,
            };
            _gameRepository.Add(game);
            
            return CreatedAtAction(nameof(Get), new { id = game.Id }, game);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    /// <summary>
    /// Atualiza um jogo existente.
    /// </summary>
    /// <remarks>
    /// Apenas usuários com a política 'Admin' podem atualizar jogos.
    /// </remarks>
    /// <param name="gameInput">Os dados do jogo a ser atualizado.<br/> O ID é obrigatório.</param>
    /// <returns>Nenhum conteúdo em caso de sucesso.</returns>
    [HttpPut]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Put([FromBody] UpdateGameInput gameInput)
    {
        try
        {
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
            
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    
    /// <summary>
    /// Deleta um jogo pelo ID.
    /// </summary>
    /// <remarks>
    /// Apenas usuários com a política 'Admin' podem deletar jogos.
    /// </remarks>
    /// <param name="id">O ID (int) do jogo a ser deletado.</param>
    /// <returns>Nenhum conteúdo em caso de sucesso.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Delete([FromRoute] int id)
    {
        try
        {
            var game = _gameRepository.GetById(id);
            if (game == null)
            {
                return NotFound(new { message = $"Jogo com ID {id} não encontrado." });
            }
            
            _gameRepository.Delete(id);
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno.", error = e.Message });
        }
    }
    
    
}