using eDrive3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eDrive3.Controllers.Api
{
    //API que devolve um token JWT quando o login do utilizador é executado com sucesso
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        //Endpoint POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginData login)
        {
            //Procura utilizadores pelo username
            var user = await _userManager.FindByNameAsync(login.Username);

            //Verifica se, caso o utilizador exista, a password esteja correta
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {

                //Procura roles associadas ao utilizador (Admin, Aluno, Instrutor)
                var roles = await _userManager.GetRolesAsync(user);


                // Define claims básicas que vão dentro do token
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName ?? "")
                };

                //Adiciona roles do utilizador ao token
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                //Chave de segurança
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

                //Credenciais de assinatura, com algoritmo de segurança
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //Criação do token JWT
                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );

                //Devolve o token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized("Credenciais inválidas");
        }
    }

    //Representa o login request
    public class LoginData
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
