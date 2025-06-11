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
            var game = _gameService.CreateNewGame(request.Player1, request.Player2);
            return Ok(game);
        }
    }

    public class StartGameRequest
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
    }
}
