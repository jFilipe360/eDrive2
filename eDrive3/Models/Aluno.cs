using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    public class Aluno
    {
        //ID único para o aluno
        public int AlunoID { get; set; }

        //Nome completo do aluno
        [Required]
        [DisplayName("Nome Completo")]
        public string NomeCompleto { get; set; }

        [Required]
        [Display(Name = "Foto de Perfil")]
        public string FotoUrl { get; set; }

        //Email do aluno
        [Required]
        public string Email { get; set; }

        //Numero de telemóvel do aluno
        [Required]
        [DisplayName("Número de Telemóvel")]
        public string NrTelemovel { get; set; }

        //Morada do aluno
        [Required]
        public string Morada { get; set; }


        //Lista de presenças do aluno
        public ICollection<Presenca> Presencas { get; set; }
    }
}
