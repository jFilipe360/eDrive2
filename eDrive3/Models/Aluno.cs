namespace eDrive3.Models
{
    public class Aluno
    {
        //ID único para o aluno
        public int AlunoID { get; set; }

        //Nome completo do aluno
        public string NomeCompleto { get; set; }

        //Email do aluno
        public string Email { get; set; }

        //Numero de telemóvel do aluno
        public string NrTelemovel { get; set; }

        //Morada do aluno
        public string Morada { get; set; }


        //Lista de presenças do aluno
        public ICollection<Presenca> Presencas { get; set; }
    }
}
