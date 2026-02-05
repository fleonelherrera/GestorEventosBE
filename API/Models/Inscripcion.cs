using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(UsuarioId), nameof(EventoId), IsUnique = true)]
    [Table("Inscripciones")]
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    }
}
