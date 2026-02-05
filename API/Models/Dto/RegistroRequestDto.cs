using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto
{
    public class RegistroRequestDto
    {
        // ESTE DTO DEBE TENER SOLO LOS DATOS NECESARIOS PARA CREAR UNA CUENTA.
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Dni { get; set; }
        [Required]
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        [Required]
        public string Clave { get; set; }
        public string Celular { get; set; }
    }
}
