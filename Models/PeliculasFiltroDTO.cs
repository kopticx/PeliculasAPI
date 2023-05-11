namespace PeliculasAPI.Models
{
    public class PeliculasFiltroDTO
    {
        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosEstrenos { get; set; }
    }
}
