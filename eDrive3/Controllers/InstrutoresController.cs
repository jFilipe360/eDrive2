using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eDrive3.Data;
using eDrive3.Models;

namespace eDrive3.Controllers
{
    public class InstrutoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstrutoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Instrutores
        public async Task<IActionResult> Index()
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
    }
}
