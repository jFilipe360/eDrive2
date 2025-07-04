namespace eDrive3.Models
{
    public class Instrutor
    {
        //ID único do instrutor
        public int InstrutorID { get; set; }

        //Nome completo do instrutor
        public string Name { get; set; }

        //Email do instrutor
        public string Email { get; set; }

        //Número de telemóvel do instrutor
        public string NrTelemovel { get; set; }



        // Navigation property
        public ICollection<Aula> Aulas { get; set; }
    }
}
