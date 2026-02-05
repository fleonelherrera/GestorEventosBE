using System.ComponentModel.DataAnnotations;

namespace API.Models.Dto
{
    public class UsuarioModificacionDto
    {
        // DATOS DEL USUARIO QUE SE PUEDEN MODIFICAR
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
    }
}
