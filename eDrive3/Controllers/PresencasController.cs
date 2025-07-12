using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eDrive3.Controllers
{
    public class PresencasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PresencasController> _logger;

        public PresencasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<PresencasController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Form to mark presence by number
        [HttpGet]
        public async Task<IActionResult> MarcarPorNumero(int numero)
        {
            var aulas = await _context.Aulas
                .Where(a => a.Tipo == Aula.TipoAula.Teórica && a.Numero == numero)
                .ToListAsync();

            if (!aulas.Any())
            {
                TempData["Error"] = $"A aula teórica nº {numero} ainda não foi criada.";
                return RedirectToAction("MapaPresencas", "Alunos");
            }

            var vm = new MarcarPorNumeroViewModel
            {
                Numero = numero,
                Aulas = aulas
            };

            return View(vm);
        }

        // POST: Mark presence with code validation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarPorNumero(MarcarPorNumeroViewModel model)
        {
            _logger.LogInformation("Método MarcarPorNumero chamado.");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Usuário não autorizado.");
                return Unauthorized();
            }

            _logger.LogInformation($"Número da aula: {model.Numero}, Código: {model.Codigo}");

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("ModelState inválido.");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogInformation(error.ErrorMessage);
                    }
                }
                model.Aulas = await _context.Aulas
                    .Where(a => a.Tipo == Aula.TipoAula.Teórica && a.Numero == model.Numero)
                    .ToListAsync();
                return View(model);
            }

            var aula = await _context.Aulas
                .FirstOrDefaultAsync(a => a.Numero == model.Numero
                                       && a.Tipo == Aula.TipoAula.Teórica
                                       && a.Codigo == model.Codigo);

            if (aula == null)
            {
                _logger.LogInformation("Aula não encontrada com o número e código fornecidos.");
                ModelState.AddModelError(nameof(model.Codigo), "Código inválido para esta aula.");
                model.Aulas = await _context.Aulas
                    .Where(a => a.Tipo == Aula.TipoAula.Teórica && a.Numero == model.Numero)
                    .ToListAsync();
                return View(model);
            }

            bool existePresenca = await _context.Presencas
                .AnyAsync(p => p.AulaID == aula.AulaID && p.AlunoID == user.AlunoID);

            if (existePresenca)
            {
                _logger.LogInformation("Presença já registada para esta aula.");
                ModelState.AddModelError(string.Empty, "Presença já registada para esta aula.");
                model.Aulas = await _context.Aulas
                    .Where(a => a.Tipo == Aula.TipoAula.Teórica && a.Numero == model.Numero)
                    .ToListAsync();
                return View(model);
            }

            var presenca = new Presenca
            {
                AlunoID = user.AlunoID.Value,
                AulaID = aula.AulaID,
                Estado = Presenca.ListaEstados.Presente
            };

            _context.Presencas.Add(presenca);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Presença registrada com sucesso.");

            return RedirectToAction("MapaPresencas", "Alunos");
        }


    }
}
