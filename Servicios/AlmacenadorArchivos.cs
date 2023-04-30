using PeliculasAPI.Models;
using RestSharp;

namespace PeliculasAPI.Servicios
{
    public class AlmacenadorArchivos : IAlmacenadorArchivos
    {
        private string urlBase = "https://www.filestackapi.com/api/";
        private string FileStackKey;
        private string FileStackPolicy;
        private string FileStackSignature;

        public AlmacenadorArchivos(IConfiguration configuration)
        {
            FileStackKey = configuration.GetConnectionString("FileStackKey");
            FileStackPolicy = configuration.GetConnectionString("FileStackPolicy");
            FileStackSignature = configuration.GetConnectionString("FileStackSignature");
        }

        public async Task<AlmacenarArchivoResultado> Almacenar(IFormFile archivo)
        {
            var nombreArchivoOriginal = Path.GetFileName(archivo.FileName);
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";

            using MemoryStream file = new MemoryStream();
            archivo.CopyTo(file);

            var cliente = new RestClient(urlBase);
            var request = new RestRequest("store/S3")
                              .AddQueryParameter("key", FileStackKey)
                              .AddQueryParameter("filename", nombreArchivo)
                              .AddParameter(archivo.ContentType, file.ToArray(), ParameterType.RequestBody);

            var response = await cliente.PostAsync<ResponseUploadFile>(request);

            var result = new AlmacenarArchivoResultado
            {
                URL = response.Url,
                Titulo = nombreArchivoOriginal,
                Key = response.Url.Split("/").Last()
            };

            return result;
        }

        public async Task<AlmacenarArchivoResultado> Actualizar(string key, IFormFile archivo)
        {
            var nombreArchivoOriginal = Path.GetFileName(archivo.FileName);

            using MemoryStream file = new MemoryStream();
            archivo.CopyTo(file);

            var cliente = new RestClient(urlBase);
            var request = new RestRequest($"file/{key}")
                .AddQueryParameter("key", FileStackKey)
                .AddQueryParameter("policy", FileStackPolicy)
                .AddQueryParameter("signature", FileStackSignature)
                .AddParameter(archivo.ContentType, file.ToArray(), ParameterType.RequestBody);

            var respuesta = await cliente.PostAsync<ResponseUploadFile>(request);

            var result = new AlmacenarArchivoResultado
            {
                Titulo = nombreArchivoOriginal
            };

            return result;
        }

        public async Task Borrar(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var cliente = new RestClient(urlBase);
            var request = new RestRequest($"file/{key}")
                .AddQueryParameter("key", FileStackKey)
                .AddQueryParameter("policy", FileStackPolicy)
                .AddQueryParameter("signature", FileStackSignature)
                .AddQueryParameter("skip_storage", false);

            await cliente.DeleteAsync(request);
        }
    }
}
