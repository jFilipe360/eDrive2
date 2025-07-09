using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    public class Secretaria
    {
        public int SecretariaID { get; set; }

        [Required]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Foto de Perfil")]
        public string FotoUrl { get; set; }
    }
}
