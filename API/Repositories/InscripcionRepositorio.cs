using System.Linq.Expressions;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class InscripcionRepositorio : RepositorioGenerico<Inscripcion>, IInscripcionRepositorio
    {
        private readonly ApplicationDbContext _db;

        public InscripcionRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Inscripcion>> ObtenerInscPorUsuario(int usuarioId)
        {
            return await _db.Inscripciones
                .Include(i => i.Evento)
                .Where(i => i.UsuarioId == usuarioId)
                .OrderByDescending(i => i.FechaInscripcion)
                .ToListAsync();
        }

        public async Task<int> CantInscripcionesPorEvento(int eventoId)
        {
            return await _db.Inscripciones
                .CountAsync(i => i.EventoId == eventoId);
        }
    }
}
