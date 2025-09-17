using System.Diagnostics;
using eDrive3.Models;
using Microsoft.AspNetCore.Mvc;

namespace eDrive3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //P�gina principal
        public IActionResult Index()
        {
            return View();
        }

        //P�gina com informa��o do projeto
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
