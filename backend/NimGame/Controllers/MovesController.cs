using Microsoft.AspNetCore.Mvc;
using NimGame.Data;
using NimGame.Models;
using System.Threading.Tasks;

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

        // POST: api/moves
        [HttpPost]
        public async Task<IActionResult> CreateMove([FromBody] Move move)
        {
            if (move == null)
            {
                return BadRequest("Move is null.");
            }

            // Aqui você pode adicionar validações, por exemplo, se o movimento é válido no jogo

            await _context.Moves.AddAsync(move);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMoveById), new { id = move.Id }, move);
        }

        // GET: api/moves/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMoveById(int id)
        {
            var move = await _context.Moves.FindAsync(id);
            if (move == null)
                return NotFound();

            return Ok(move);
        }
    }
}
