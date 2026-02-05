using System.Net;

namespace API.Models
{
    public class APIResponse
    {
        public HttpStatusCode CodigoHttp { get; set; }
        public bool EsExitoso { get; set; }
        public List<string> MensajesDeError { get; set; } = new List<string>();
        public object Resultado { get; set; }
    }
}
