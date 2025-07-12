using eDrive3.Data;
using eDrive3.Models;
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
        public async Task<IActionResult> Create(
        [Bind("LessonDate,Duration,Tipo,Numero,InstrutorID")] Aula aula)
        {
            /* ───────────────────────── 1. Campos gerados pelo sistema ───────────────────── */
            aula.Codigo = GenerateCode(10);
            aula.Presencas = new List<Presenca>();
            aula.Duration = 60;                              // sempre 60 min

            /* ───────────────────────── 2. Validações de regras de negócio ───────────────── */

            // 2‑a) limite de número por tipo
            int maxNumero = (aula.Tipo == Aula.TipoAula.Teórica) ? 28 : 32;
            if (aula.Numero < 1 || aula.Numero > maxNumero)
                ModelState.AddModelError(nameof(aula.Numero),
                    $"Para {aula.Tipo} o número deve estar entre 1 e {maxNumero}.");

            // 2‑b) bloco de 1 h já ocupado por QUALQUER aula (T ou P)
            DateTime blocoFim = aula.LessonDate.AddHours(1);
            bool blocoOcupado = await _context.Aulas.AnyAsync(a =>
                a.LessonDate >= aula.LessonDate && a.LessonDate < blocoFim);
            if (blocoOcupado)
                ModelState.AddModelError(nameof(aula.LessonDate),
                    "Já existe uma aula (teórica ou prática) neste bloco horário.");

            // 2‑c) evitar duplicar (tipo,número) no MESMO dia
            bool numRepetido = await _context.Aulas.AnyAsync(a =>
                a.Tipo == aula.Tipo &&
                a.Numero == aula.Numero &&
                a.LessonDate.Date == aula.LessonDate.Date);
            if (numRepetido)
                ModelState.AddModelError(nameof(aula.Numero),
                    "Já existe uma aula com esse número nesse dia.");

            // 2‑d) sequência cronológica para aulas PRÁTICAS
            if (aula.Tipo == Aula.TipoAula.Prática)
            {
                int ultimoNumero = await _context.Aulas
                    .Where(a => a.Tipo == Aula.TipoAula.Prática)
                    .OrderByDescending(a => a.LessonDate)
                    .Select(a => a.Numero)
                    .FirstOrDefaultAsync();          // 0 se não existir nenhuma

                DateTime? ultimoHorario = await _context.Aulas
                    .Where(a => a.Tipo == Aula.TipoAula.Prática)
                    .OrderByDescending(a => a.LessonDate)
                    .Select(a => a.LessonDate)
                    .FirstOrDefaultAsync();

                // número tem de ser imediatamente a seguir
                if (aula.Numero != ultimoNumero + 1)
                    ModelState.AddModelError(nameof(aula.Numero),
                        $"O próximo número válido é {ultimoNumero + 1}.");

                // horário tem de ser depois da última prática
                if (ultimoHorario is not null && aula.LessonDate <= ultimoHorario.Value)
                    ModelState.AddModelError(nameof(aula.LessonDate),
                        $"A última aula prática é em {ultimoHorario:dd/MM/yyyy HH:mm}. Escolha um horário posterior.");
            }

            /* ───────────────────────── 3. Se falhou, volta para o ecrã ───────────────────── */
            if (!ModelState.IsValid)
            {
                // recarrega lista de instrutores
                ViewBag.InstructorList = _context.Instrutores
                    .Select(i => new SelectListItem
                    {
                        Value = i.InstrutorID.ToString(),
                        Text = $"{i.InstrutorID} - {i.Name}"
                    })
                    .ToList();
                return View(aula);
            }

            /* ───────────────────────── 4. Gravação ───────────────────────────────────────── */
            _context.Add(aula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        private static string GenerateCode(int len = 10)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789@$%!*?";
            var rng = new Random();
            return new string(Enumerable.Range(0, len)
                                        .Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }
    }

}
