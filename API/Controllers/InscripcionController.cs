using System.Net;
using System.Security.Claims;
using API.Models;
using API.Models.Dto;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InscripcionController : ControllerBase
    {
        private readonly IInscripcionRepositorio _inscripcionRepo;
        private readonly IEventoRepositorio _eventoRepo;
        private readonly IMapper _mapper;
        protected APIResponse _respuesta;

        public InscripcionController(IInscripcionRepositorio inscripcionRepo, IEventoRepositorio eventoRepo, IMapper mapper)
        {
            _inscripcionRepo = inscripcionRepo;
            _eventoRepo = eventoRepo;
            _mapper = mapper;
            _respuesta = new();
        }

        // METODO PARA INSCRIBIRSE A UN EVENTO
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Inscribirse([FromBody] InscripcionRequestDto inscripcionDto)
        {
            try
            {
                var claimId = User.FindFirst(ClaimTypes.Name);
                if (claimId == null) return Unauthorized(); 
                int usuarioId = int.Parse(claimId.Value);
                int eventoId = inscripcionDto.EventoId;

                var evento = await _eventoRepo.Obtener(e => e.Id == eventoId, tracked: false);

                if (evento == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.NotFound;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("El evento no existe.");

                    return NotFound(_respuesta);
                }

                // VALIDACIONES DE NEGOCIO

                // EVENTO QUE YA PASO
                if (evento.Fecha < DateTime.UtcNow)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("El evento ha finalizado.");

                    return BadRequest(_respuesta);
                }

                // EL USUARIO YA ESTA INSCRITO
                var inscripcionExistente = await _inscripcionRepo.Obtener(i => i.UsuarioId == usuarioId && i.EventoId == eventoId, tracked: false);
                if (inscripcionExistente != null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("El usuario ya esta inscrito en el evento.");

                    return BadRequest(_respuesta);
                }

                // NO HAY CUPO
                var inscritosActuales = await _inscripcionRepo.CantInscripcionesPorEvento(eventoId);
                if (inscritosActuales >= evento.CapacidadMax)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Entradas agotadas.");

                    return BadRequest(_respuesta);
                }

                Inscripcion nuevaInscripcion = new()
                {
                    UsuarioId = usuarioId,
                    EventoId = eventoId,
                    FechaInscripcion = DateTime.UtcNow
                };

                await _inscripcionRepo.Crear(nuevaInscripcion);
                await _inscripcionRepo.Guardar();

                _respuesta.CodigoHttp = HttpStatusCode.Created;
                _respuesta.EsExitoso = true;
                _respuesta.Resultado = _mapper.Map<InscripcionResponseDto>(nuevaInscripcion);

                return CreatedAtAction(nameof(ObtenerEventosDeUsuario), _respuesta);
            }
            catch (Exception ex)
            {
                _respuesta.CodigoHttp = HttpStatusCode.InternalServerError;
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError, _respuesta);
            }
        }

        // METODO PARA OBTENER LAS INSCRIPCIONES DEL USUARIO
        [HttpGet("mis-eventos")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> ObtenerEventosDeUsuario()
        {
            try
            {
                var claimId = User.FindFirst(ClaimTypes.Name);
                if (claimId == null) return Unauthorized();
                int usuarioId = int.Parse(claimId.Value);

                var listaEventos = await _inscripcionRepo.ObtenerInscPorUsuario(usuarioId);

                _respuesta.CodigoHttp = HttpStatusCode.OK;
                _respuesta.EsExitoso = true;
                _respuesta.Resultado = _mapper.Map<IEnumerable<InscripcionResponseDto>>(listaEventos);

                return Ok(_respuesta);
            }
            catch (Exception ex)
            {
                _respuesta.CodigoHttp = HttpStatusCode.InternalServerError;
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError, _respuesta);
            }
        }
    }
}
