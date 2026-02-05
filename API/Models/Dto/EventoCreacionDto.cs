namespace API.Models.Dto
{
    public class EventoCreacionDto
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public int CapacidadMax { get; set; }
        public decimal PrecioEntrada { get; set; }
        public string PortadaUrl { get; set; }
        public string Ubicacion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
