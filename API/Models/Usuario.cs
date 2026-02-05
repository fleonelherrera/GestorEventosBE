using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(Dni), IsUnique = true)]
    [Index(nameof(Correo), IsUnique = true)]
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Dni { get; set; }

        [Required]
        public string Correo { get; set; }

        public string NombreUsuario { get; set; }

        [Required]
        public string Clave { get; set; } // GUARDAR EL HASH DE LA CLAVE

        [Required]
        public string Celular { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string Rol { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

        public ICollection<Evento> EventosCreados { get; set; } = new List<Evento>();
    }
}
