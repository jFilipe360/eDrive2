using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eDrive3.Controllers.Api
{

    //API para marcar as presenças
    [Route("api/[controller]")]
    [ApiController]
    //Apenas alunos autenticados com o token JWT podem marcar presença
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Aluno")]
    public class PresencasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PresencasController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Define o corpo esperado no request
        public record PresencaRequest(string Codigo);

        // Endpoint: POST /api/presencas/marcar
        [HttpPost("marcar")]
        public async Task<IActionResult> Marcar([FromBody] PresencaRequest req)
        {
            //Valida o código
            if (string.IsNullOrWhiteSpace(req.Codigo))
                return BadRequest("Código inválido.");

            //Procura a aula com o código recebido
            var aula = await _context.Aulas.FirstOrDefaultAsync(a => a.Codigo == req.Codigo);
            if (aula == null)
                return NotFound("Aula não encontrada.");

            //Procurar o ID do aluno pelo JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            //Buscar o ApplicationUser correspondente ao ID
            var user = await _context.Users
                .Include(u => u.Aluno)
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user == null || user.Aluno == null)
                return Unauthorized();

            var aluno = user.Aluno;

            //Verifica se já existe alguma presença para a aula indicada
            bool jaMarcada = await _context.Presencas
                .AnyAsync(p => p.AlunoID == aluno.AlunoID && p.AulaID == aula.AulaID);

            if (jaMarcada)
                return Conflict("Presença já registada.");

            //Se não há, cria uma presença nova
            var presenca = new Presenca
            {
                AlunoID = aluno.AlunoID,
                AulaID = aula.AulaID,
                Estado = Presenca.ListaEstados.Presente
            };

            //Guarda a presença na DB
            _context.Presencas.Add(presenca);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Presença marcada com sucesso!" });
        }
    }
}
