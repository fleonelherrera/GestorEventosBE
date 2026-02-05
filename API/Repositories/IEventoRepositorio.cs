using API.Models;

namespace API.Repositories
{
    public interface IEventoRepositorio : IRepositorioGenerico<Evento>
    {
        Task<Evento> Actualizar(Evento evento);
        Task<List<Evento>> ObtenerEventosPorUsuario(int idUsuario);
    }
}
