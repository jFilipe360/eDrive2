using eDrive3.Data;
using eDrive3.Hubs;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace eDrive3.Controllers
{
    [Authorize(Roles = "Secretaria")]
    public class AulasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificacoesHub> _hubContext;

        public AulasController(ApplicationDbContext context, IHubContext<NotificacoesHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
            //Cria uma aula teórica para um determinado instrutor
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
            //Força o tipo de aula para teórica
            aula.Tipo = Aula.TipoAula.Teórica;

            //Campos gerados pelo sistema
            aula.Codigo = GenerateCode(10);
            aula.Presencas = new List<Presenca>();
            aula.Duration = 60; // sempre 60 minutos

            //Validações
            // a) limite do número da aula teórica (só de 1 a 28)
            int maxNumero = 28;
            if (aula.Numero < 1 || aula.Numero > maxNumero)
                ModelState.AddModelError(nameof(aula.Numero), $"O número deve estar entre 1 e {maxNumero}.");

            // b) bloco horário já ocupado?
            DateTime blocoFim = aula.LessonDate.AddHours(1);
            bool blocoOcupado = await _context.Aulas
                .AnyAsync(a => a.LessonDate >= aula.LessonDate && a.LessonDate < blocoFim);
            if (blocoOcupado)
                ModelState.AddModelError(nameof(aula.LessonDate), "Já existe uma aula neste bloco horário.");

            // c) verificar duplicação de aula teórica com o mesmo número no mesmo dia
            bool numRepetido = await _context.Aulas
                .AnyAsync(a => a.Tipo == Aula.TipoAula.Teórica
                            && a.Numero == aula.Numero
                            && a.LessonDate.Date == aula.LessonDate.Date);
            if (numRepetido)
                ModelState.AddModelError(nameof(aula.Numero), "Já existe uma aula teórica com esse número nesse dia.");

            //Se válido, gravar 
            if (ModelState.IsValid)
            {
                _context.Add(aula);
                await _context.SaveChangesAsync();

                // Envia notificação apenas se for aula teórica
                if (aula.Tipo == Aula.TipoAula.Teórica)
                {
                    string msg = $"Foi marcada uma nova aula teórica nº {aula.Numero} para {aula.LessonDate:dd/MM/yyyy HH:mm}.";
                    await _hubContext.Clients.All.SendAsync("ReceberNotificacao", msg);
                }

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
            //O código será uma string de x caracteres que inclui letras(minúsculas e maiúsculas), numeros e símbolos
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789@$%!*?";
            
            //Gera um valor aleatório e vai buscar o valor que se encontra nessa posição na string "chars"
            var rng = new Random();
            return new string(Enumerable.Range(0, len).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }
    }

}
