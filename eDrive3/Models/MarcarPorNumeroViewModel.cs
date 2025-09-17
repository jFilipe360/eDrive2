using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    public class MarcarPorNumeroViewModel
    {
        public int Numero { get; set; }

        [Required(ErrorMessage = "O código é obrigatório")]
        [StringLength(10, ErrorMessage = "O código deve ter até 10 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        public List<Aula> Aulas { get; set; }

        public MarcarPorNumeroViewModel()
        {
            Aulas = new List<Aula>(); // Inicialize a lista no construtor
        }
    }
}
