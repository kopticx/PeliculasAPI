using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.Entidades;
using PeliculasAPI.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Models
{
    public class PeliculaCreacionDTO
    {
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public IFormFile Poster { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> CinesIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculaDTO>>))]
        public List<ActorPeliculaDTO> Actores { get; set; }
    }

    public class PeliculaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string PosterUrl { get; set; }
        public List<GeneroDTO> Generos { get; set; }
        public List<PeliculaActorDTO> Actores { get; set; }
        public List<CineDTO> Cines { get; set; }
    }

    public class PeliculaActorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Personaje { get; set; }
        public string Foto { get; set; }
        public int Orden { get; set; }
    }
}
