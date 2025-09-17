using System.ComponentModel.DataAnnotations;

namespace eDrive3.Models
{
    public class CreateUserVm
    {
        public string Role { get; set; }     //Aluno, Instrutor, Secretaria     

        
        //Atributos do user
        public string NomeCompleto { get; set; }
        public string FotoUrl { get; set; }
        public string NrTelemovel { get; set; }
        public string Morada { get; set; }  //Aluno       
    }
}
