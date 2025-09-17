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
    [Authorize(Roles = "Secretaria")]
    public class AulasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AulasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Aulas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Aulas.ToListAsync());
        }

        // GET: Aulas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aula = await _context.Aulas
                .FirstOrDefaultAsync(m => m.AulaID == id);
            if (aula == null)
            {
                return NotFound();
            }

            return View(aula);
        }

        // GET: Aulas/Create
        public IActionResult Create()
        {
            ViewBag.InstructorList = _context.Instrutores
                .Select(i => new SelectListItem
                {
                    Value = i.InstrutorID.ToString(),
                    Text = $"{i.InstrutorID} - {i.Name}"
                })
                .ToList();

            return View();
        }

        // POST: Aulas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LessonDate,Duration,Numero,InstrutorID")] Aula aula)
        {
            /* ──────────── 1. Forçar aula teórica ──────────── */
            aula.Tipo = Aula.TipoAula.Teórica;

            /* ──────────── 2. Campos gerados pelo sistema ──────────── */
            aula.Codigo = GenerateCode(10);
            aula.Presencas = new List<Presenca>();
            aula.Duration = 60; // sempre 60 min

            /* ──────────── 3. Validações ──────────── */
            // 3-a) limite de número (só 1..28)
            int maxNumero = 28;
            if (aula.Numero < 1 || aula.Numero > maxNumero)
                ModelState.AddModelError(nameof(aula.Numero), $"O número deve estar entre 1 e {maxNumero}.");

            // 3-b) bloco horário já ocupado
            DateTime blocoFim = aula.LessonDate.AddHours(1);
            bool blocoOcupado = await _context.Aulas
                .AnyAsync(a => a.LessonDate >= aula.LessonDate && a.LessonDate < blocoFim);
            if (blocoOcupado)
                ModelState.AddModelError(nameof(aula.LessonDate), "Já existe uma aula neste bloco horário.");

            // 3-c) duplicar número teórico no mesmo dia
            bool numRepetido = await _context.Aulas
                .AnyAsync(a => a.Tipo == Aula.TipoAula.Teórica
                            && a.Numero == aula.Numero
                            && a.LessonDate.Date == aula.LessonDate.Date);
            if (numRepetido)
                ModelState.AddModelError(nameof(aula.Numero), "Já existe uma aula teórica com esse número nesse dia.");

            /* ──────────── 4. Se válido, gravar ──────────── */
            if (ModelState.IsValid)
            {
                _context.Add(aula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(aula);
        }

        // GET: Aulas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aula = await _context.Aulas.FindAsync(id);
            if (aula == null)
            {
                return NotFound();
            }
            return View(aula);
        }

        // POST: Aulas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AulaID,LessonDate,Duration,Tipo,InstructorId")] Aula aula)
        {
            if (id != aula.AulaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AulaExists(aula.AulaID))
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
            return View(aula);
        }

        // GET: Aulas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aula = await _context.Aulas
                .FirstOrDefaultAsync(m => m.AulaID == id);
            if (aula == null)
            {
                return NotFound();
            }

            return View(aula);
        }

        // POST: Aulas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aula = await _context.Aulas.FindAsync(id);
            if (aula != null)
            {
                _context.Aulas.Remove(aula);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Verifica se a aula existe
        private bool AulaExists(int id)
        {
            return _context.Aulas.Any(e => e.AulaID == id);
        }


        //Gerar um código para a aula
        private static string GenerateCode(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789@$%!*?";
            var rng = new Random();
            return new string(Enumerable.Range(0, len).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }
    }

}
