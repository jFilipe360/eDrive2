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
    public class InstrutoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstrutoresController(ApplicationDbContext context,
                                      UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Instrutores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Instrutores.ToListAsync());
        }

        // GET: Instrutores/ListaHorarios  - será a lista publica de instrutores, para que todos possam consultar os horarios
        public async Task<IActionResult> ListaHorarios()
        {
            return View(await _context.Instrutores.ToListAsync());
        }

        // GET: Instrutores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrutor = await _context.Instrutores
                .FirstOrDefaultAsync(m => m.InstrutorID == id);
            if (instrutor == null)
            {
                return NotFound();
            }

            return View(instrutor);
        }

        
        // GET: Instrutors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrutor = await _context.Instrutores.FindAsync(id);
            if (instrutor == null)
            {
                return NotFound();
            }
            return View(instrutor);
        }

        // POST: Instrutors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstrutorID,Name,FotoUrl,Email,NrTelemovel")] Instrutor instrutor)
        {
            if (id != instrutor.InstrutorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instrutor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstrutorExists(instrutor.InstrutorID))
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
            return View(instrutor);
        }

        // GET: Instrutors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instrutor = await _context.Instrutores
                .FirstOrDefaultAsync(m => m.InstrutorID == id);
            if (instrutor == null)
            {
                return NotFound();
            }

            return View(instrutor);
        }

        // POST: Instrutors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instrutor = await _context.Instrutores.FindAsync(id);
            if (instrutor != null)
            {
                _context.Instrutores.Remove(instrutor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstrutorExists(int id)
        {
            return _context.Instrutores.Any(e => e.InstrutorID == id);
        }

        // GET: Instrutores/HorarioSemanal/5?ano=2025&semanaISO=29
        [HttpGet]
        public async Task<IActionResult> HorarioSemanal(
                int id,
                int? ano,
                int? semanaISO)
        {
            var instrutor = await _context.Instrutores
                .Include(i => i.Aulas)
                    .ThenInclude(a => a.Presencas)          // ← presenças
                        .ThenInclude(p => p.Aluno)          // ← aluno de cada presença
                .FirstOrDefaultAsync(i => i.InstrutorID == id);
            if (instrutor == null) return NotFound();

            DateTime inicioSemana = (ano, semanaISO) switch
            {
                (int a, int s) => System.Globalization.ISOWeek.ToDateTime(a, s, DayOfWeek.Monday).Date,
                _ => System.Globalization.ISOWeek.ToDateTime(
                                      System.Globalization.ISOWeek.GetYear(DateTime.Today),
                                      System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Today),
                                      DayOfWeek.Monday).Date
            };

            var fimSemana = inicioSemana.AddDays(7);

            var aulas = instrutor.Aulas
                .Where(a => a.LessonDate >= inicioSemana && a.LessonDate < fimSemana)
                .ToList();

            ViewBag.Inicio = inicioSemana;
            ViewBag.Instrutor = instrutor;
            return View(aulas);     // HorarioSemanal.cshtml
        }

        public record ReservaViewModel(int InstrutorId, string Date, int Hour);

        [HttpPost]
        [Authorize(Roles = "Aluno")]
        public async Task<IActionResult> ReservarAula([FromBody] ReservaViewModel vm)
        {
            if (vm is null) return BadRequest();

            //aluno autenticado
            var user = await _userManager.GetUserAsync(User);
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.AlunoID == user.AlunoID);
            if (aluno == null) return Unauthorized();

            //data e hora pedidas
            if (!DateTime.TryParse($"{vm.Date} {vm.Hour}:00", out var dtInicio))
                return BadRequest("Data inválida.");
            var dtFim = dtInicio.AddHours(1);

            //Verificar se o bloco pertence ao passado, impossibilitando de reservar nova aula
            if (dtInicio < DateTime.Now)
                return Conflict("Não é possível reservar aulas em blocos horários já decorridos.");

            //slot já está ocupado?
            if (await BlocoOcupado(dtInicio, vm.InstrutorId))
                return Conflict("Já existe uma aula nesse bloco horário.");

            // ----- conta aulas práticas do aluno (via Presencas) -----
            var presencasPraticas = _context.Presencas
                .Include(p => p.Aula)
                .Where(p => p.AlunoID == aluno.AlunoID
                         && p.Aula.Tipo == Aula.TipoAula.Prática);

            // 2 por dia?
            int diaCount = await presencasPraticas
                .CountAsync(p => p.Aula.LessonDate.Date == dtInicio.Date);
            if (diaCount >= 2)
                return Conflict("Limite de 2 aulas práticas por dia atingido.");

            // total 32?
            int totalCount = await presencasPraticas.CountAsync();
            if (totalCount >= 32)
                return Conflict("Já reservou 32 aulas práticas.");

            //a reserva só é feita depois da ultima aula reservada
            DateTime? ultimaAula = await presencasPraticas
                .Select(p => p.Aula.LessonDate)
                .OrderByDescending(d => d)
                .FirstOrDefaultAsync();
            if (ultimaAula is not null && dtInicio <= ultimaAula.Value)
                return Conflict($"A sua última aula prática é em {ultimaAula:dd/MM/yyyy HH:mm}. "
                              + "Só pode marcar aulas posteriores.");

            //calculo do nr da próxima aula
            int proximoNumero = (ultimaAula == null) ? 1   // nenhuma ainda
                     : await presencasPraticas.CountAsync() + 1;   // 1 … 32

            // ----- criar Aula -----
            var aula = new Aula
            {
                InstrutorID = vm.InstrutorId,
                LessonDate = dtInicio,
                Duration = 60,
                Tipo = Aula.TipoAula.Prática,
                Numero = proximoNumero,
                Confirmada = false
            };
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();    // obtém AulaID

            // ----- criar Presenca que liga aluno ↔ aula -----
            _context.Presencas.Add(new Presenca
            {
                AlunoID = aluno.AlunoID,
                AulaID = aula.AulaID,
                Estado = Presenca.ListaEstados.Indefinido
            });
            await _context.SaveChangesAsync();

            return Ok(new { alunoNome = aluno.NomeCompleto });
        }

        private async Task<bool> BlocoOcupado(DateTime inicio, int instrutorId)
        {
            DateTime fim = inicio.AddHours(1);
            return await _context.Aulas.AnyAsync(a =>
                a.LessonDate >= inicio && a.LessonDate < fim);
        }

        // GET: Lista a dropdown
        public IActionResult CodigoAulas()
        {
            ViewBag.Aulas = Enumerable.Range(1, 28).ToList();
            return View();
        }


        // Mostra todas as aulas teóricas para um número selecionado
        public async Task<IActionResult> CodAula(int numero)
        {

            var aulas = await _context.Aulas
                .Where(a => a.Tipo == Aula.TipoAula.Teórica && a.Numero == numero)
                .OrderBy(a => a.LessonDate)
                .ToListAsync();

            ViewBag.NumeroAula = numero;
            return View(aulas);
        }
    }
}