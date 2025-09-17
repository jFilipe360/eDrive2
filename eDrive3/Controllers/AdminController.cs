using eDrive3.Data;
using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
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

    //Criar um novo utilizador para a plataforma
    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserVm vm)
    {
        //Valida os dados
        if (!ModelState.IsValid)
        {
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Erro em {key}: {error.ErrorMessage}");
                }
            }
            return View(vm);
        }

        //Verifica a role indicada
        if (!await _roleManager.RoleExistsAsync(vm.Role))
        {
            ModelState.AddModelError("", "Role inválido.");
            return View(vm);
        }

        // Determinar caminho da foto
        string fotoFinal = "/uploads/default.png";

        //Se a imagem for por URL
        if (!string.IsNullOrWhiteSpace(vm.FotoUrl))
        {
            fotoFinal = vm.FotoUrl;
        }
        //Se a imagem for por upload
        else if (vm.FotoUpload != null && vm.FotoUpload.Length > 0)
        {
            //extensões autorizadas
            var allowedExt = new[] { ".png", ".jpg", ".jpeg" };
            var ext = Path.GetExtension(vm.FotoUpload.FileName).ToLower();
            if (!allowedExt.Contains(ext))
            {
                ModelState.AddModelError("FotoUpload", "Apenas ficheiros .png, .jpg, .jpeg são permitidos.");
                return View(vm);
            }

            //Guarda as imagens na diretoria wwwroot/uploads
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + ext;
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await vm.FotoUpload.CopyToAsync(stream);
            }

            //foto que servirá como foto de perfil, em caso de upload
            fotoFinal = "/uploads/" + fileName;
        }

        //Dados do utilizador
        int domainId = 0;
        string role = vm.Role.ToLower();
        string email;

        //Guardar utilizadores, dependendo dos roles.
        //O email tem um placeholder porque será gerado um novo email posteriormente
        switch (vm.Role)
        {
            case "Aluno":
                var aluno = new Aluno
                {
                    NomeCompleto = vm.NomeCompleto,
                    FotoUrl = fotoFinal,
                    NrTelemovel = vm.NrTelemovel,
                    Morada = vm.Morada ?? "",
                    Email = "temp@temp.com" // placeholder temporário
                };
                _db.Alunos.Add(aluno);
                await _db.SaveChangesAsync();
                domainId = aluno.AlunoID;
                break;

            case "Instrutor":
                var instr = new Instrutor
                {
                    Name = vm.NomeCompleto,
                    FotoUrl = fotoFinal,
                    NrTelemovel = vm.NrTelemovel,
                    Email = "temp@temp.com" // placeholder temporário
                };
                _db.Instrutores.Add(instr);
                await _db.SaveChangesAsync();
                domainId = instr.InstrutorID;
                break;

            case "Secretaria":
                var sec = new Secretaria
                {
                    Nome = vm.NomeCompleto,
                    FotoUrl = fotoFinal,
                    NrTelemovel = vm.NrTelemovel,
                    Email = "temp@temp.com" // placeholder temporário
                };
                _db.Secretarias.Add(sec);
                await _db.SaveChangesAsync();
                domainId = sec.SecretariaID;
                break;

            case "Admin":
                domainId = 0;
                break;
        }

        // Gerar e-mail e password (domainId será o id do novo utilizador na aplicação)
        email = $"{role}{domainId}@{role}.com";
        string password = GeneratePassword(8);

        //Cria o novo user Identity
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FotoUrl = fotoFinal
        };

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

        // Guardar e-mail no registo de domínio
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
                               .ExecuteUpdate(x => x.SetProperty(x => x.Email, email));
                break;
        }

        await _db.SaveChangesAsync();

        TempData["NewUserEmail"] = email;
        TempData["NewUserPassword"] = password;
        TempData["NewUserRole"] = vm.Role;

        return RedirectToAction(nameof(Credentials));
    }

    //Vista para mostrar as credenciais do novo utilizador criado
    public IActionResult Credentials()
    {
        if (TempData["NewUserEmail"] == null) return RedirectToAction("Index", "Home");
        return View();
    }

    //Função para gerar nova palavra passe
    private static string GeneratePassword(int length = 8)
    {
        const string lowers = "abcdefghijkmnopqrstuvwxyz";
        const string uppers = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string symbols = "#@$%!&*?";
        var rng = new Random();

        //Lista com todos os caracteres para utilizar na criarção de uma password
        var chars = new List<char>
        {
            lowers[rng.Next(lowers.Length)],
            uppers[rng.Next(uppers.Length)],
            digits[rng.Next(digits.Length)],
            symbols[rng.Next(symbols.Length)]
        };

        //Preenche a futura password
        string all = lowers + uppers + digits + symbols;
        while (chars.Count < length)
            chars.Add(all[rng.Next(all.Length)]);

        //Baralha os caracteres
        return new string(chars.OrderBy(_ => rng.Next()).ToArray());
    }
}
