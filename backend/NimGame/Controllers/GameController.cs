using Microsoft.AspNetCore.Mvc;
using NimGame.Models;
using NimGame.Services;

namespace NimGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("start")]
        public IActionResult StartGame([FromBody] StartGameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Player1))
                return BadRequest(new { message = "Jogador 1 é obrigatório." });

            var game = _gameService.CreateNewGame(request.Player1, request.Player2);
            if (game == null)
                return BadRequest(new { message = "Não foi possível criar o jogo." });

            return Ok(game);
        }
    }

    public class StartGameRequest
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
    }
}
