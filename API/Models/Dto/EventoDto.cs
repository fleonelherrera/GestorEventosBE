using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto
{
    public class EventoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public decimal PrecioEntrada { get; set; }
        public string PortadaUrl { get; set; }
        public string Ubicacion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
