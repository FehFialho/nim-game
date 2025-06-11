using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NimGame.Data;
using NimGame.Models;

namespace NimGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateMove([FromBody] Move move)
        {
            _context.Moves.Add(move);
            _context.SaveChanges();
            return Ok(move);
        }

        [HttpGet("game/{gameId}")]
        public IActionResult GetMovesByGame(int gameId)
        {
            var moves = _context.Moves.Where(m => m.GameId == gameId).ToList();
            return Ok(moves);
        }
    }
}
