namespace eDrive3.Models
{
    //Separar as aulas teóricas das práticas, para que o utilizador possa marcar presença separadamente
    public class MarcarPresencaMenuViewModel
    {
        public List<int> AulasTeoricas { get; set; }
        public List<int> AulasPraticas { get; set; }
    }
}
