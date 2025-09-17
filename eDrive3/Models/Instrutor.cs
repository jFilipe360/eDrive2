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
        public string Name { get; set; }

        [Required]
        [Display(Name = "Foto de Perfil")]
        public string FotoUrl { get; set; }


        //Email do instrutor
        [Required]
        public string Email { get; set; }

        //Número de telemóvel do instrutor
        [Required]
        [DisplayName("Número de telemóvel")]
        public string NrTelemovel { get; set; }



        // Navigation property
        public ICollection<Aula> Aulas { get; set; } = new List<Aula>();
    }
}
