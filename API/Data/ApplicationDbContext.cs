using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        // TABLAS
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }

        // CONSTRUCTOR
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Evento>()
                .HasOne(e => e.Organizador)
                .WithMany(u => u.EventosCreados)
                .HasForeignKey(e => e.OrganizadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
