using API.Models;
using API.Models.Dto;

namespace API.Repositories
{
    public interface IUsuarioRepositorio
    {
        bool esUsuarioUnico(string nombreUsuario);

        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

        Task<Usuario> Registrar(RegistroRequestDto registroRequestDto);

        Task<Usuario> ObtenerUsuario(int id);
        Task<Usuario> Actualizar(Usuario usuario);
        Task Guardar();
    }
}
