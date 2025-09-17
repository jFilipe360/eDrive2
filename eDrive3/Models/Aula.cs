using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    public class Aula
    {
        //ID único para cada aula
        public int AulaID { get; set; }

        //Data para a aula
        [Required]
        [Display(Name = "Data da aula")]
        public DateTime LessonDate { get; set; }

        //Duração da aula
        [Required]
        [Display(Name = "Duração")]
        public int Duration { get; set; }

        //Tipo de aula (Teórica ou prática)
        [Required]
        public TipoAula Tipo { get; set; }
        
        //Numero da aula
        [Required]
        [Range(1, 32)]
        [Display(Name = "Número da aula")]
        public int Numero { get; set; }          // 1‑28 ou 1‑32 consoante o Tipo

        //Codigo para marcara presença
        [MaxLength(10)]
        [Display(Name = "Código da aula")]
        public string? Codigo { get; set; }       // será uma string aleatória de 10 caracteres

        public bool Confirmada { get; set; } = false;




        // Foreign keys
        public int InstrutorID { get; set; }
        public Instrutor? Instrutor { get; set; }


        // Navigation property
        public ICollection<Presenca> Presencas { get; set; } = new List<Presenca>();




        // Define a lista de possibilidades para o tipo de aula
        public enum TipoAula
        {
            Teórica,
            Prática
        }
    }
}
