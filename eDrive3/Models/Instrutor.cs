using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    public class Instrutor
    {
        //ID único do instrutor
        public int InstrutorID { get; set; }

        //Nome completo do instrutor
        [Required]
        [DisplayName("Nome Completo")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Foto de Perfil")]
        public string? FotoUrl { get; set; }


        //Email do instrutor
        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        //Número de telemóvel do instrutor
        [Required]
        [DisplayName("Número de telemóvel")]
        [RegularExpression("(([+]|00)[0-9]{1,5})?[1-9][0-9]{5,10}", ErrorMessage = "Escreva um nº de telemóvel. Pode adicionar indicativo do país.")]
        public string NrTelemovel { get; set; }



        // Navigation property
        public ICollection<Aula> Aulas { get; set; } = new List<Aula>();
    }
}
