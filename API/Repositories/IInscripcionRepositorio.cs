using API.Models;

namespace API.Repositories
{
    public interface IInscripcionRepositorio : IRepositorioGenerico<Inscripcion>
    {
        // EVENTOS DEL USUARIO
        Task<IEnumerable<Inscripcion>> ObtenerInscPorUsuario(int usuarioId);

        // CANTIDAD DE INSCRITOS EN UN EVENTO
        Task<int> CantInscripcionesPorEvento(int eventoId);
    }
}
