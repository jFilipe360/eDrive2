using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eDrive3.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Aluno")]
    public class PresencasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PresencasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public record PresencaRequest(string Codigo);

        [HttpPost("marcar")]
        public async Task<IActionResult> Marcar([FromBody] PresencaRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Codigo))
                return BadRequest("Código inválido.");

            var aula = await _context.Aulas.FirstOrDefaultAsync(a => a.Codigo == req.Codigo);
            if (aula == null)
                return NotFound("Aula não encontrada.");

            // buscar ID do aluno pelo claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            // buscar o ApplicationUser logado
            var user = await _context.Users
                .Include(u => u.Aluno)
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user == null || user.Aluno == null)
                return Unauthorized();

            var aluno = user.Aluno;

            bool jaMarcada = await _context.Presencas
                .AnyAsync(p => p.AlunoID == aluno.AlunoID && p.AulaID == aula.AulaID);

            if (jaMarcada)
                return Conflict("Presença já registada.");

            var presenca = new Presenca
            {
                AlunoID = aluno.AlunoID,
                AulaID = aula.AulaID,
                Estado = Presenca.ListaEstados.Presente
            };

            _context.Presencas.Add(presenca);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Presença marcada com sucesso!" });
        }
    }
}
