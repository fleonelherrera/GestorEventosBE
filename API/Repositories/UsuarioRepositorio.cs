using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Models;
using API.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        private string keySecreta;

        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            keySecreta = config.GetValue<string>("ApiSettings:Secret");
        }

        public bool esUsuarioUnico(string nombreUsuario)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());
            
            if (usuario == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario.ToLower() == loginRequestDto.NombreUsuario.ToLower());

            // USUARIO NO EXISTE
            if (usuario == null)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            bool esValido = BCrypt.Net.BCrypt.Verify(loginRequestDto.Clave, usuario.Clave);

            // LA CLAVE ES INCORRECTA
            if (esValido == false)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            // SI EL USUARIO EXISTE GENERAMOS EL JW TOKEN
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keySecreta);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponseDto = new()
            {
                Token = tokenHandler.WriteToken(token),
                Usuario = usuario
            };
            loginResponseDto.Usuario.Clave = "";
            return loginResponseDto;
        }

        public async Task<Usuario> Registrar(RegistroRequestDto registroRequestDto)
        {
            Usuario usuario = new()
            {
                Nombre = registroRequestDto.Nombre,
                Apellido = registroRequestDto.Apellido,
                Dni = registroRequestDto.Dni,
                Correo = registroRequestDto.Correo,
                NombreUsuario = registroRequestDto.NombreUsuario,
                Clave = BCrypt.Net.BCrypt.HashPassword(registroRequestDto.Clave),
                Celular = registroRequestDto.Celular,
                FechaCreacion = DateTime.UtcNow,
                Rol = "Usuario"
            };
            await _db.Usuarios.AddAsync(usuario);
            await _db.SaveChangesAsync();

            usuario.Clave = "";
            return usuario;
        }

        public async Task<Usuario> ObtenerUsuario(int id)
        {
            return await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> Actualizar(Usuario usuario)
        {
            _db.Usuarios.Update(usuario);
            return usuario;
        }

        public async Task Guardar()
        {
            await _db.SaveChangesAsync();
        }
    }
}
