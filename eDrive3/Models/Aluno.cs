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
        [StringLength(50)]
        public string NomeCompleto { get; set; }

        [Display(Name = "Foto de Perfil")]
        public string? FotoUrl { get; set; }

        //Email do aluno
        [Required]
        [StringLength(30)]
        public string Email { get; set; }

        //Numero de telemóvel do aluno
        [Required]
        [DisplayName("Número de Telemóvel")]
        [RegularExpression("(([+]|00)[0-9]{1,5})?[1-9][0-9]{5,10}", ErrorMessage = "Escreva um nº de telemóvel. Pode adicionar indicativo do país.")]
        public string NrTelemovel { get; set; }

        //Morada do aluno
        [Required]
        [StringLength(50)]
        public string Morada { get; set; }


        //Lista de presenças do aluno
        public ICollection<Presenca> Presencas { get; set; }

    }
}