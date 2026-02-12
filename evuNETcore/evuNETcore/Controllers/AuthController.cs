using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace evuNETcore.Controllers
{
    [Route("api/auth")] // Esto define la ruta base: https://localhost:xxxx/api/auth
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Esta clave DEBE SER IDÉNTICA a la que pusiste en Program.cs
        private const string SecretKey = "ESTA_ES_MI_CLAVE_SECRETA_SUPER_LARGA_PARA_LA_EVALUACION";

        [HttpPost("login")] // Esto completa la ruta: /api/auth/login
        public IActionResult Login([FromBody] LoginDto login)
        {
            // 1. Simulación de validación de usuario
            // Aquí podrías consultar tu base de datos si quisieras, pero para la evaluación basta esto:
            if (login.Username == "admin" && login.Password == "admin123")
            {
                // 2. Generar el Token
                var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, login.Username));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(10), // Duración del token
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(tokenConfig);

                // 3. Devolver el token al usuario
                return Ok(new { token = tokenString });
            }
            else
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }
        }
    }

    // Clase pequeña para recibir los datos del JSON
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}