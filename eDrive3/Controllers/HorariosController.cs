using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eDrive3.Controllers
{
    public class HorariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HorariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Horarios/Semana?ano=2025&semanaISO=37   (se não vier nada => semana actual)
        public async Task<IActionResult> Semana(int? ano, int? semanaISO)
        {
            var hoje = DateTime.Today;
            // calcula 1º dia da semana ISO pedida (segunda‑feira)
            var cal = System.Globalization.ISOWeek.ToDateTime(
                        ano ?? System.Globalization.ISOWeek.GetYear(hoje),
                        semanaISO ?? System.Globalization.ISOWeek.GetWeekOfYear(hoje),
                        DayOfWeek.Monday);

            DateTime inicioSemana = cal.Date;                  // segunda 00:00
            DateTime fimSemana = inicioSemana.AddDays(7);   // próxima segunda 00:00

            // busca todas as aulas teóricas no intervalo
            var aulas = await _context.Aulas
                .Where(a => a.LessonDate >= inicioSemana && a.LessonDate < fimSemana
                         && a.Tipo == Aula.TipoAula.Teórica)
                .Include(a => a.Instrutor)
                .ToListAsync();

            ViewBag.Inicio = inicioSemana;
            return View(aulas);
        }
    }
}
