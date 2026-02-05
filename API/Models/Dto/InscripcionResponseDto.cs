namespace API.Models.Dto
{
    public class InscripcionResponseDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EventoId { get; set; }
        public DateTime FechaInscripcion { get; set; }

        public EventoDto Evento { get; set; }
    }
}
