using System.Net;
using System.Security.Claims;
using API.Models;
using API.Models.Dto;
using API.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepo;
        private APIResponse _respuesta;
        private readonly IMapper _mapper;

        public UsuarioController(IUsuarioRepositorio usuarioRepo, IMapper mapper)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _respuesta = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto modelo)
        {
            var loginResponse = await _usuarioRepo.Login(modelo);

            if (loginResponse.Usuario == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add("Usuario o clave son incorrectas.");

                return BadRequest(_respuesta);
            }

            _respuesta.CodigoHttp = HttpStatusCode.OK;
            _respuesta.EsExitoso = true;
            _respuesta.Resultado = loginResponse;

            return Ok(_respuesta);
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroRequestDto modelo)
        {
            bool esUsuarioUnico = _usuarioRepo.esUsuarioUnico(modelo.NombreUsuario);

            if (!esUsuarioUnico)
            {
                _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add("El usuario ya existe.");
                return BadRequest(_respuesta);
            }

            var usuario = await _usuarioRepo.Registrar(modelo);

            if (usuario == null)
            {
                _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add("Error al registrar el usuario.");
                return BadRequest(_respuesta);
            }

            _respuesta.CodigoHttp = HttpStatusCode.OK;
            _respuesta.EsExitoso = true;
            
            return Ok(_respuesta);
        }

        [Authorize]
        [HttpGet("mi-perfil")]
        public async Task<ActionResult<APIResponse>> ObtenerUsuario()
        {
            try
            {
                var claimId = User.FindFirst(ClaimTypes.Name);
                int usuarioId = int.Parse(claimId.Value);

                var usuario = await _usuarioRepo.ObtenerUsuario(usuarioId);

                _respuesta.CodigoHttp = HttpStatusCode.OK;
                _respuesta.EsExitoso = true;
                _respuesta.Resultado = _mapper.Map<UsuarioDto>(usuario);

                return Ok(_respuesta);
            }
            catch (Exception ex)
            {
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.Message);
                return StatusCode(500, _respuesta);
            }
        }

        [Authorize]
        [HttpPut("editar-perfil")]
        public async Task<ActionResult<APIResponse>> ActualizarUsuario([FromBody] UsuarioModificacionDto usuarioModifDto)
        {
            try
            {
                var claimId = User.FindFirst(ClaimTypes.Name);
                int usuarioid = int.Parse(claimId.Value);

                var usuarioAModificar = await _usuarioRepo.ObtenerUsuario(usuarioid);

                if (usuarioAModificar == null)
                {
                    _respuesta.CodigoHttp = HttpStatusCode.NotFound;
                    _respuesta.EsExitoso = false;
                    _respuesta.MensajesDeError.Add("El usuario a modificar no existe.");
                    return NotFound(_respuesta);
                }

                _mapper.Map(usuarioModifDto, usuarioAModificar);

                await _usuarioRepo.Actualizar(usuarioAModificar);
                await _usuarioRepo.Guardar();

                _respuesta.CodigoHttp = HttpStatusCode.OK;
                _respuesta.EsExitoso = true;
                return Ok(_respuesta);
            }
            catch (Exception ex)
            {
                _respuesta.CodigoHttp = HttpStatusCode.BadRequest;
                _respuesta.EsExitoso = false;
                _respuesta.MensajesDeError.Add(ex.Message);

                return BadRequest(_respuesta);
            }
        }
    }
}
