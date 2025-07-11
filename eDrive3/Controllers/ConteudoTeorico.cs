using Microsoft.AspNetCore.Mvc;

namespace eDrive3.Controllers
{
    public class ConteudoTeorico : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Aulas = Enumerable.Range(1, 28).ToList();
            return View();
        }

        // Ação para exibir o conteúdo de uma aula específica
        public IActionResult Aula(int id)
        {
            // Aqui você pode adicionar lógica para buscar o conteúdo específico da aula
            // Por enquanto, vamos apenas passar o número da aula para a view
            ViewBag.AulaId = id;
            return View();
        }
    }
}
