namespace eDrive3.Models
{
    public class Aula
    {
        //ID único para cada aula
        public int AulaID { get; set; }

        //Data para a aula
        public DateTime LessonDate { get; set; }

        //Duração da aula
        public int Duration { get; set; }

        //Tipo de aula (Teórica ou prática)
        public TipoAula Tipo { get; set; }



        // Foreign key
        public int InstructorId { get; set; }
        public Instrutor Instrutor { get; set; }

        // Navigation property
        public ICollection<Presenca> Presencas { get; set; }




        // Define a lista de possibilidades para o tipo de aula
        public enum TipoAula
        {
            Teórica,
            Prática
        }
    }
}
