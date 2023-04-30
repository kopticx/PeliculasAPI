using PeliculasAPI.Models;

namespace PeliculasAPI.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<AlmacenarArchivoResultado> Actualizar(string key, IFormFile archivo);
        Task<AlmacenarArchivoResultado> Almacenar(IFormFile archivo);
        Task Borrar(string key);
    }
}
