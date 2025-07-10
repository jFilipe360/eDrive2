using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eDrive3.Controllers
{
    [Authorize(Roles = "Admin")]  // Only admins can access
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        // GET: /Admin/CreateUser
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: /Admin/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserVm vm)
        {
            if (!await _roleManager.RoleExistsAsync(vm.Role))
            {
                ModelState.AddModelError("", "Role inválido.");
                return View(vm);
            }

            int domainId = 0;                      // o ID int das tabelas da app
            string role = vm.Role.ToLower();   // aluno / instrutor / secretaria
            string email;                          // será preenchido já com o ID

            // Criar e guardar a identidade conforme a role selecionada
            switch (vm.Role)
            {
                case "Aluno":
                    var aluno = new Aluno
                    {
                        NomeCompleto = vm.NomeCompleto,
                        FotoUrl = vm.FotoUrl,
                        Email = "",               // será preenchido depois
                        NrTelemovel = vm.NrTelemovel,
                        Morada = vm.Morada
                    };
                    _db.Alunos.Add(aluno);
                    await _db.SaveChangesAsync();
                    domainId = aluno.AlunoID;
                    break;

                case "Instrutor":
                    var instr = new Instrutor
                    {
                        Name = vm.NomeCompleto,
                        FotoUrl = vm.FotoUrl,
                        Email = "",
                        NrTelemovel = vm.NrTelemovel
                    };
                    _db.Instrutores.Add(instr);
                    await _db.SaveChangesAsync();
                    domainId = instr.InstrutorID;
                    break;

                case "Secretaria":
                    var sec = new Secretaria
                    {
                        Nome = vm.NomeCompleto,
                        FotoUrl = vm.FotoUrl,
                        Email = "",
                        NrTelemovel = vm.NrTelemovel
                    };
                    _db.Secretarias.Add(sec);
                    await _db.SaveChangesAsync();
                    domainId = sec.SecretariaID;
                    break;

                case "Admin":
                    // Admin não tem tabela própria então não faz nada
                    domainId = 0;
                    break;
            }

            // Construir o email
            email = $"{role}{domainId}@{role}.com";   // ex: aluno3@aluno.com

            // Gerar uma password
            string password = GeneratePassword(8);

            // Cria o ApplicationUser
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            // ligar ForeignKeys
            if (vm.Role == "Aluno") user.AlunoID = domainId;
            if (vm.Role == "Instrutor") user.InstrutorID = domainId;
            if (vm.Role == "Secretaria") user.SecretariaID = domainId;

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                foreach (var e in createResult.Errors) ModelState.AddModelError("", e.Description);
                return View(vm);
            }
            await _userManager.AddToRoleAsync(user, vm.Role);

            // Guardar o e‑mail no registo de domínio
            switch (vm.Role)
            {
                case "Aluno":
                    _db.Alunos.Where(a => a.AlunoID == domainId)
                              .ExecuteUpdate(setters => setters.SetProperty(a => a.Email, email));
                    break;
                case "Instrutor":
                    _db.Instrutores.Where(i => i.InstrutorID == domainId)
                                  .ExecuteUpdate(s => s.SetProperty(i => i.Email, email));
                    break;
                case "Secretaria":
                    _db.Secretarias.Where(s => s.SecretariaID == domainId)
                                   .ExecuteUpdate(s => s.SetProperty(x => x.Email, email));
                    break;
            }

            await _db.SaveChangesAsync();

            TempData["NewUserEmail"] = email;
            TempData["NewUserPassword"] = password;
            TempData["NewUserRole"] = vm.Role;

            return RedirectToAction(nameof(Credentials));
        }

        //Página para mostrar credenciais para que o admin possa enviar os dados aos utilizadores
        public IActionResult Credentials()
        {
            if (TempData["NewUserEmail"] == null) return RedirectToAction("Index", "Home");
            return View();
        }


        // gerador de passwords ---------------------------------------------
        private static string GeneratePassword(int length = 8)
        {
            const string lowers = "abcdefghijkmnopqrstuvwxyz";
            const string uppers = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "#@$%!&*?";
            var rng = new Random();

            // garantir pelo menos um elemento de cada requisito
            var chars = new List<char>
            {
                lowers[rng.Next(lowers.Length)],
                uppers[rng.Next(uppers.Length)],
                digits[rng.Next(digits.Length)],
                symbols[rng.Next(symbols.Length)]
            };

            // preencher a password até chegar ao lenght indicado
            string all = lowers + uppers + digits + symbols;
            while (chars.Count < length)
                chars.Add(all[rng.Next(all.Length)]);

            // baralhar caracteres
            return new string(chars.OrderBy(_ => rng.Next()).ToArray());
        }
    }
}



