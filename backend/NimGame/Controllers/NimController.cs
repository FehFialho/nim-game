using Microsoft.AspNetCore.Mvc;

namespace NimGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NimController : ControllerBase
    {
        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            // Aqui você implementaria a lógica do Nim
            int[] board = request.Board;
            int index = request.Column;
            int remove = request.Count;

            if (index < 0 || index >= board.Length || remove < 1 || board[index] < remove)
            {
                return BadRequest(new { error = "Jogada inválida" });
            }

            board[index] -= remove;

            return Ok(new { newBoard = board });
        }
    }

    public class MoveRequest
    {
        public int[] Board { get; set; } = new int[5];
        public int Column { get; set; }
        public int Count { get; set; }
    }
}
