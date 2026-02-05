namespace API.Models.Dto
{
    public class UsuarioDto
    {
        // DEVUELVO LOS DATOS DEL USUARIO AL FRONTEND PARA EL FORMULARIO DE EDICION
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
    }
}
