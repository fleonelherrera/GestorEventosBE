using System.Diagnostics.Eventing.Reader;
using System.Net;
using API.Models;
using API.Models.Dto;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IEventoRepositorio _eventoRepo;
        private readonly IMapper _mapper;
        protected APIResponse _respuesta;


        public EventoController(IEventoRepositorio eventoRepo, IMapper mapper)
        {
            _eventoRepo = eventoRepo;
            _mapper = mapper;
            _respuesta = new();
        }

        // OBTENER TODOS LOS EVENTOS
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ObtenerEventos()
        {
            try
            {
                IEnumerable<Evento> eventos = await _eventoRepo.ObtenerTodos();

                _respuesta.CodigoHttp = HttpStatusCode.OK;
                _respuesta.EsExitoso = true;
                _respuesta.Resultado = _mapper.Map<IEnumerable<EventoDto>>(eventos);

                return Ok(_respuesta);
            }
            catch (Exception ex)
            {
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError, _respuesta);
            }
        }

        // OBTENER UN EVENTO
        [HttpGet("{id:int}", Name = "ObtenerEvento")]
        [AllowAnonymous]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> ObtenerEvento(int id)
        {
            try
            {
                if (id == 0)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("El id del evento a buscar no puede ser cero.");

                    return BadRequest(_respuesta);
                }

                var evento = await _eventoRepo.Obtener(e => e.Id == id);

                // VALIDO QUE EL EVENTO EXISTA
                if (evento == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.NotFound;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("El evento no existe.");

                    return NotFound(_respuesta);
                }

                _respuesta.CodigoHttp = HttpStatusCode.OK;
                _respuesta.EsExitoso = true;
                _respuesta.Resultado = _mapper.Map<EventoDto>(evento);

                return Ok(_respuesta);
            }
            catch (Exception ex)
            {
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError, _respuesta);
            }
        }

        // OBTENER EVENTOS DEL USUARIO
        [HttpGet("mis-eventos")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> ObtenerEventosDelUsuario()
        {
            try
            {
                var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.Name);

                if (claimId == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.Unauthorized;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Identidad no disponible.");

                    return Unauthorized(_respuesta);
                }
                int usuarioId = int.Parse(claimId.Value);

                var listaEventos = await _eventoRepo.ObtenerEventosPorUsuario(usuarioId);

                _respuesta.CodigoHttp = HttpStatusCode.OK;
                _respuesta.EsExitoso = true;
                _respuesta.Resultado = _mapper.Map<IEnumerable<EventoDto>>(listaEventos);

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

        // CREAR UN EVENTO
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearEvento([FromBody] EventoCreacionDto nuevoEventoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (nuevoEventoDto == null)
                {
                    return BadRequest(nuevoEventoDto);
                }

                var claimId = User.FindFirst(ClaimTypes.Name);

                if (claimId == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.Unauthorized;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Usuario no registrado.");
                    return Unauthorized(_respuesta);
                }

                int organizadorId = int.Parse(claimId.Value);

                Evento nuevoEvento = _mapper.Map<Evento>(nuevoEventoDto);
                nuevoEvento.OrganizadorId = organizadorId;

                await _eventoRepo.Crear(nuevoEvento);
                await _eventoRepo.Guardar();

                _respuesta.CodigoHttp = HttpStatusCode.Created;
                _respuesta.EsExitoso = true;

                // MAPEO PARA NO MOSTRAR LA ENTIDAD COMPLETA EN EL RESULTADO
                EventoDto evento = _mapper.Map<EventoDto>(nuevoEvento);
                _respuesta.Resultado = evento;

                return CreatedAtRoute("ObtenerEvento", new { id = nuevoEvento.Id }, _respuesta);
            }
            catch (Exception ex)
            {
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError, _respuesta);
            }
        }

        // MODIFICAR UN EVENTO
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModificarEvento(int id, [FromBody] EventoModificacionDto eventoModif)
        {
            try
            {
                if (eventoModif == null || id != eventoModif.Id)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Error al modificar el evento.");

                    return BadRequest(_respuesta);
                }

                // OBTENGO ID DEL USUARIO LOGUEADO
                var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.Name);

                if (claimId == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.Unauthorized;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Usuario no encontrado.");
                    return Unauthorized(_respuesta);
                }
                int usuarioLogueadoId = int.Parse(claimId.Value);

                var eventoOriginal = await _eventoRepo.Obtener(e => e.Id == id, tracked: false);

                if (eventoOriginal == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.NotFound;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("El evento a modificar no existe.");
                    return NotFound(_respuesta);
                }

                if (eventoOriginal.OrganizadorId != usuarioLogueadoId)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.Forbidden;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Acceso denegado.");

                    return StatusCode(StatusCodes.Status403Forbidden, _respuesta);
                }

                _mapper.Map(eventoModif, eventoOriginal);

                await _eventoRepo.Actualizar(eventoOriginal);

                // ACA PODRIA ENVIAR NoContent, PERO COMO RETORNO Ok, MANDO Ok
                _respuesta.CodigoHttp = HttpStatusCode.OK;
                _respuesta.EsExitoso = true;

                // RETORNO OK PARA ENVIAR EL OBJETO _respuesta
                return Ok(_respuesta);
            }
            catch (Exception)
            {
                _respuesta.CodigoHttp = HttpStatusCode.InternalServerError;
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add("Error al modificar");

                return StatusCode(StatusCodes.Status500InternalServerError, _respuesta);
            }
        }

        // ELIMINAR UN EVENTO
        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarEvento(int id)
        {
            try
            {
                if (id == 0)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                    _respuesta.EsExitoso = false;

                    return BadRequest(_respuesta);
                }

                // OBTENGO ID DEL USUARIO LOGUEADO
                var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.Name);

                if (claimId == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.Unauthorized;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Usuario no encontrado.");
                    return Unauthorized(_respuesta);
                }
                int usuarioLogueadoId = int.Parse(claimId.Value);
                
                var eventoAEliminar = await _eventoRepo.Obtener(e => e.Id == id);

                if (eventoAEliminar == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.NotFound;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Evento no encontrado.");

                    return NotFound(_respuesta);
                }

                if (eventoAEliminar.OrganizadorId != usuarioLogueadoId)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.Forbidden;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("Acceso denegado.");

                    return StatusCode(StatusCodes.Status403Forbidden, _respuesta);
                }

                await _eventoRepo.Remover(eventoAEliminar);
                await _eventoRepo.Guardar();

                _respuesta.CodigoHttp = HttpStatusCode.NoContent;
                _respuesta.EsExitoso = true;

                return Ok(_respuesta); // ENVIO OK PARA PODER ENVIAR RESPONSE EN EL BODY
            }
            catch (Exception ex)
            {
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.ToString());

                return StatusCode(StatusCodes.Status500InternalServerError, _respuesta);
            }
        }
    }
}
