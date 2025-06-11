using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] LoginRequest request)
    {
        // Aqui é só exemplo: substitua por validação no banco de dados
        if (request.Username == "admin" && request.Password == "123")
        {
            return Ok(new { message = "Login bem-sucedido!" });
        }

        return Unauthorized(new { message = "Usuário ou senha inválidos!" });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
