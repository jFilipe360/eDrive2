using eDrive3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;


namespace eDrive3.Controllers.Api
{
    //API para mostrar o código da aula e o svg do código QR
    [Route("api/[controller]")]
    [ApiController]
    public class AulasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AulasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Aulas/5/QRCode
        [HttpGet("{id}/QRCode")]
        public async Task<IActionResult> GetQrCodeJson(int id)
        {

            //Procura a aula por id
            var aula = await _context.Aulas.FirstOrDefaultAsync(a => a.AulaID == id);

            //Caso não exista aula para o ID indicado, apresenta um aviso
            if (aula == null || string.IsNullOrEmpty(aula.Codigo))
                return NotFound(new { Message = "Aula ou código não encontrado." });

            //Gerar um QR Code com o código da aula
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(aula.Codigo, QRCodeGenerator.ECCLevel.Q);

            var qrCodeSvg = new SvgQRCode(qrCodeData).GetGraphic(10);

            return Ok(new
            {
                Codigo = aula.Codigo,
                QrCodeSvg = qrCodeSvg
            });
        }
    }
}