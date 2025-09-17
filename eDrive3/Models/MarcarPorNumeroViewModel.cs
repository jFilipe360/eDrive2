using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    //Marcar a presença das aulas teóricas
    public class MarcarPorNumeroViewModel
    {
        //Número da aula para marcar presença
        public int Numero { get; set; }

        //Código da aula
        [Required(ErrorMessage = "O código é obrigatório")]
        [StringLength(10, ErrorMessage = "O código deve ter até 10 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        //Lista de aulas correspondentes ao número formecido
        public List<Aula> Aulas { get; set; }

        public MarcarPorNumeroViewModel()
        {
            Aulas = new List<Aula>(); // Inicialize a lista no construtor
        }
    }
}
