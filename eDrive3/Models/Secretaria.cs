using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    public class Secretaria
    {
        //Id único para cada secretário(a)
        public int SecretariaID { get; set; }

        //Nome completo do(a) secretário(a)
        [Required]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        //Foto de perfil do(a) secretário(a)
        [Required]
        [Display(Name = "Foto de Perfil")]
        public string FotoUrl { get; set; }

        //Email do(a) secretário(a)
        [Required]
        public string Email { get; set; }

        //Número de telemóvel do(a) secretário(a)

        [Required]
        public string NrTelemovel { get; set; }
    }
}
