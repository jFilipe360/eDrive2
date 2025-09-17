using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eDrive3.Controllers
{
    public class SecretariasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SecretariasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Secretarias
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Secretarias.ToListAsync());
        }

        // GET: Secretarias/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secretaria = await _context.Secretarias
                .FirstOrDefaultAsync(m => m.SecretariaID == id);
            if (secretaria == null)
            {
                return NotFound();
            }

            return View(secretaria);
        }

        // GET: Secretarias/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secretaria = await _context.Secretarias.FindAsync(id);
            if (secretaria == null)
            {
                return NotFound();
            }
            return View(secretaria);
        }

        // POST: Secretarias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SecretariaID,Nome,FotoUrl,Email,NrTelemovel")] Secretaria secretaria)
        {
            if (id != secretaria.SecretariaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secretaria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecretariaExists(secretaria.SecretariaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(secretaria);
        }

        // GET: Secretarias/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secretaria = await _context.Secretarias
                .FirstOrDefaultAsync(m => m.SecretariaID == id);
            if (secretaria == null)
            {
                return NotFound();
            }

            return View(secretaria);
        }

        // POST: Secretarias/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Encontrar a secretaria
            var secretaria = await _context.Secretarias.FindAsync(id);
            if (secretaria == null)
            {
                return NotFound();
            }

            // Desassociar o ApplicationUser que está ligado a esta secretaria
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.SecretariaID == id);

            if (user != null)
            {
                user.SecretariaID = null;
                await _userManager.UpdateAsync(user);
            }

            //Remover a secretaria
            _context.Secretarias.Remove(secretaria);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Lista de reservas pendentes
        [Authorize(Roles = "Secretaria")]
        public async Task<IActionResult> ReservasPendentes()
        {

            //Gera uma lista de todas as reservas -> aulas práticas, marcadas pelos alunos, cuja presença ainda não foi validada
            var pendentes = await _context.Aulas
                .Include(a => a.Instrutor)
                .Include(a => a.Presencas).ThenInclude(p => p.Aluno)
                .Where(a => a.Tipo == Aula.TipoAula.Prática && !a.Confirmada)
                .OrderBy(a => a.LessonDate)
                .ToListAsync();
            return View(pendentes);
        }

        //Confirmação da presença na aula
        // POST: Secretarias/Confirmar/123
        [Authorize(Roles = "Secretaria")]
        [HttpPost]
        public async Task<IActionResult> Confirmar(int id)
        {
            //Procura a aula
            var aula = await _context.Aulas
                .Include(a => a.Presencas)
                .FirstOrDefaultAsync(a => a.AulaID == id);

            if (aula == null) return NotFound();

            //Confirma a aula
            aula.Confirmada = true;

            //Na BD, altera o estado para concluíu
            foreach (var p in aula.Presencas)
            {
                p.Estado = Presenca.ListaEstados.Concluíu;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ReservasPendentes));
        }

        // POST: Secretarias/Rejeitar/123
        [Authorize(Roles = "Secretaria")]
        [HttpPost]
        public async Task<IActionResult> Rejeitar(int id)
        {
            //Procura a aula
            var aula = await _context.Aulas
                        .Include(a => a.Presencas)
                        .FirstOrDefaultAsync(a => a.AulaID == id);

            //Erro caso não exista aula
            if (aula == null) return NotFound();
            if (aula.Tipo != Aula.TipoAula.Prática) return BadRequest();

            //Remove a aula marcada
            int alunoId = aula.Presencas.First().AlunoID;
            int numeroEliminado = aula.Numero;

            _context.Presencas.RemoveRange(aula.Presencas);
            _context.Aulas.Remove(aula);

            //Renumeração das aulas práticas posteriores do mesmo aluno
            //Exemplo: se o aluno marcar as aulas práticas 2 e 3, caso falte à aula 2, a aula prática 3 passa a ser a nova aula 2
            var posteriores = await _context.Aulas
                .Include(a => a.Presencas)
                .Where(a => a.Tipo == Aula.TipoAula.Prática &&
                            a.Numero > numeroEliminado &&
                            a.Presencas.Any(p => p.AlunoID == alunoId))
                .OrderBy(a => a.Numero)
                .ToListAsync();

            foreach (var a in posteriores)
                a.Numero--;                       // reduz 1 posição

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ReservasPendentes));
        }

        private bool SecretariaExists(int id)
        {
            return _context.Secretarias.Any(e => e.SecretariaID == id);
        }
    }
}
