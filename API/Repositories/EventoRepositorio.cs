using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class EventoRepositorio : RepositorioGenerico<Evento>, IEventoRepositorio
    {
        private readonly ApplicationDbContext _db;

        public EventoRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Evento> Actualizar(Evento evento)
        {
            // CAMBIOS 05/02: REENGANCHAR ENTIDAD QUE ESTA FUERA DEL CONTEXTO
            var entry = _db.Entry(evento);

            if (entry.State == EntityState.Detached)
            {
                _db.Eventos.Attach(evento);
            }
            _db.Entry(evento).State = EntityState.Modified;

            await _db.SaveChangesAsync();
            return evento;
        }

        public async Task<List<Evento>> ObtenerEventosPorUsuario(int idUsuario)
        {
            return await _db.Eventos
                .Where(e => e.OrganizadorId == idUsuario)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }
    }
}
