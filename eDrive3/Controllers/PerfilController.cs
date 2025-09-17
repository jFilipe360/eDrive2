using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eDrive3.Controllers
{
    //Mostrar o perfil do utilizador
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PerfilController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Perfil/Details
        public async Task<IActionResult> Details()
        {
            //Obter o ID do utilizador autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users
                .Include(u => u.Aluno)
                .Include(u => u.Instrutor)
                .Include(u => u.Secretaria)
                .FirstOrDefaultAsync(u => u.Id == userId);

            //Erro caso o utilizador não seja encontrado
            if (user == null) return NotFound();

            //Redirecionar para a view do perfil conforme a role
            if (user.Aluno != null) return View("AlunoDetails", user.Aluno);
            if (user.Instrutor != null) return View("InstrutorDetails", user.Instrutor);
            if (user.Secretaria != null) return View("SecretariaDetails", user.Secretaria);

            return RedirectToAction("Index", "Home"); //Volta à página Home caso o utilizador seja de outro tipo (ex: Admin)
        }
    }
}
