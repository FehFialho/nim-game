using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NimGame.Data;
using NimGame.Models;

namespace NimGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u =>
                u.Username == request.Username
            );
            if (user == null)
            {
                return Unauthorized(new { message = "Usuário ou senha inválidos!" });
            }

            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!validPassword)
            {
                return Unauthorized(new { message = "Usuário ou senha inválidos!" });
            }

            // Aqui você pode gerar e retornar um token JWT depois

            return Ok(
                new
                {
                    message = "Login bem-sucedido!",
                    username = user.Username,
                    userId = user.Id,
                }
            );
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
