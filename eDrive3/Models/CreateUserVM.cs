using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace eDrive3.Models
{
    public class CreateUserVm : IValidatableObject
    {
        public string Role { get; set; }     //Aluno, Instrutor, Secretaria     


        //Atributos do user
        [Required]
        [DisplayName("Nome Completo")]
        [StringLength(50)]
        public string NomeCompleto { get; set; }

        [Display(Name = "Foto de Perfil")]
        public string? FotoUrl { get; set; }

        [Display(Name = "Foto de Perfil")]
        public IFormFile? FotoUpload { get; set; }

        [Required]
        [Display(Name = "Número de Telemóvel")]
        [RegularExpression("(([+]|00)[0-9]{1,5})?[1-9][0-9]{5,10}", ErrorMessage = "Escreva um nº de telefone. Pode adicionar indicativo do país.")]
        public string NrTelemovel { get; set; }

        public string? Morada { get; set; }  //Aluno

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == "Aluno" && string.IsNullOrWhiteSpace(Morada))
            {
                yield return new ValidationResult(
                    "Morada é obrigatória para Aluno",
                    new[] { nameof(Morada) });
            }
        }
    }
}
