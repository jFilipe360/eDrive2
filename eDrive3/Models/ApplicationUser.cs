using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace eDrive3.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? AlunoID { get; set; }

        [ForeignKey("AlunoID")]
        public Aluno Aluno { get; set; }

        public int? InstrutorID { get; set; }

        [ForeignKey("InstrutorID")]
        public Instrutor Instrutor { get; set; }

        public int? SecretariaID { get; set; }

        [ForeignKey("SecretariaID")]
        public Secretaria Secretaria { get; set; }
    }
}
