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
        [StringLength(50)]
        public string Nome { get; set; }

        //Foto de perfil do(a) secretário(a)
        [Display(Name = "Foto de Perfil")]
        public string? FotoUrl { get; set; }

        //Email do(a) secretário(a)
        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        //Número de telemóvel do(a) secretário(a)

        [Required]
        [Display(Name = "Número de telemóvel")]
        [RegularExpression("(([+]|00)[0-9]{1,5})?[1-9][0-9]{5,10}", ErrorMessage = "Escreva um nº de telemóvel. Pode adicionar indicativo do país.")]
        public string NrTelemovel { get; set; }
    }
}
