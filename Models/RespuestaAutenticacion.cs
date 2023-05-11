namespace PeliculasAPI.Models
{
    public class RespuestaAutenticacion
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
        public string FotoUsuario { get; set; }
    }
}
