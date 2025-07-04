using eDrive3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eDrive3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Instrutor> Instrutores { get; set; }
        public DbSet<Aula> Aulas { get; set; }
        public DbSet<Presenca> Presencas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração das relações
            modelBuilder.Entity<Presenca>()
                .HasKey(a => new { a.PresencaID });

            modelBuilder.Entity<Presenca>()
                .HasOne(a => a.Aluno)
                .WithMany(s => s.Presencas)
                .HasForeignKey(a => a.AlunoID);

            modelBuilder.Entity<Presenca>()
                .HasOne(a => a.Aula)
                .WithMany(l => l.Presencas)
                .HasForeignKey(a => a.AulaID);
        }
    }
}
