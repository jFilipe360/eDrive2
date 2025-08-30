using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eDrive3.Controllers
{
    public class AlunosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlunosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Alunos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Alunos.ToListAsync());
        }

        // GET: Alunos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(m => m.AlunoID == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        
        // GET: Alunos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }
            return View(aluno);
        }

        // POST: Alunos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlunoID,NomeCompleto,FotoUrl,Email,NrTelemovel,Morada")] Aluno aluno)
        {
            if (id != aluno.AlunoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aluno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlunoExists(aluno.AlunoID))
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
            return View(aluno);
        }

        // GET: Alunos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aluno = await _context.Alunos
                .FirstOrDefaultAsync(m => m.AlunoID == id);
            if (aluno == null)
            {
                return NotFound();
            }

            return View(aluno);
        }

        // POST: Alunos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);
            if (aluno != null)
            {
                _context.Alunos.Remove(aluno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlunoExists(int id)
        {
            return _context.Alunos.Any(e => e.AlunoID == id);
        }

        //Função para mostrar as presenças do aluno
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> MapaPresencas()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var numerosTeoricos = Enumerable.Range(1, 28).ToList();
            var numerosPraticos = Enumerable.Range(1, 32).ToList();

            var presencasTeoricas = await _context.Presencas
                .Include(p => p.Aula)
                .Where(p => p.AlunoID == user.AlunoID
                         && p.Aula.Tipo == Aula.TipoAula.Teórica
                         && p.Estado == Presenca.ListaEstados.Presente)
                .ToListAsync();

            var presencasPraticas = await _context.Presencas
                .Include(p => p.Aula)
                .Where(p => p.AlunoID == user.AlunoID
                         && p.Aula.Tipo == Aula.TipoAula.Prática
                         && p.Estado == Presenca.ListaEstados.Concluíu)
                .ToListAsync();

            var presentesPorNumeroTeorico = new HashSet<int>(presencasTeoricas.Select(p => p.Aula.Numero));
            var presentesPorNumeroPratico = new HashSet<int>(presencasPraticas.Select(p => p.Aula.Numero));

            ViewBag.NumerosTeoricos = numerosTeoricos;
            ViewBag.PresentesPorNumeroTeorico = presentesPorNumeroTeorico;
            ViewBag.NumerosPraticos = numerosPraticos;
            ViewBag.PresentesPorNumeroPratico = presentesPorNumeroPratico;

            return View();
        }
    }
}
