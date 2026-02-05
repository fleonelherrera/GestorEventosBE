using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("Eventos")]
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public string Tipo { get; set; } // PUBLICO O PRIVADO

        [ForeignKey("Organizador")]
        public int OrganizadorId { get; set; }
        public Usuario Organizador { get; set; }

        public int CapacidadMax { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioEntrada { get; set; }

        public string PortadaUrl { get; set; }

        public string Ubicacion { get; set; }

        public DateTime Fecha { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
