using AutoMapper;
using NetTopologySuite.Geometries;
using PeliculasAPI.Entidades;
using PeliculasAPI.Models;

namespace PeliculasAPI.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            //Mapear del dto a la entidad
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<Genero, GeneroDTO>().ReverseMap();

            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.FotoURL, options => options.Ignore());

            CreateMap<CineDTO, Cine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.Lng, dto.Lat))));

            CreateMap<Cine, CineDTO>()
                .ForMember(x => x.Lat, dto => dto.MapFrom(campo => campo.Ubicacion.Y))
                .ForMember(x => x.Lng, dto => dto.MapFrom(campo => campo.Ubicacion.X));

            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.PosterUrl, opciones => opciones.Ignore())
                .ForMember(x => x.PeliculasGeneros, opciones => opciones.MapFrom(MapearPeliculasGeneros))
                .ForMember(x => x.PeliculasCines, opciones => opciones.MapFrom(MapearPeliculasCines))
                .ForMember(x => x.PeliculasActores, opciones => opciones.MapFrom(MapearPeliculasActores));

            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(x => x.Generos, opciones => opciones.MapFrom(MapearPeliculasGeneros))
                .ForMember(x => x.Actores, opciones => opciones.MapFrom(MapearPeliculasActores))
                .ForMember(x => x.Cines, opciones => opciones.MapFrom(MapearPeliculasCines));
        }

        #region .       Mapeo Peliculas       .
        private List<PeliculasGeneros> MapearPeliculasGeneros(PeliculaCreacionDTO peliculaDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();

            if (peliculaDTO.GenerosIds is null)
            {
                return resultado;
            }

            foreach (var id in peliculaDTO.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasCines> MapearPeliculasCines(PeliculaCreacionDTO peliculaDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasCines>();

            if (peliculaDTO.CinesIds is null)
            {
                return resultado;
            }

            foreach (var id in peliculaDTO.CinesIds)
            {
                resultado.Add(new PeliculasCines() { CineId = id });
            }

            return resultado;
        }

        private List<PeliculasActores> MapearPeliculasActores(PeliculaCreacionDTO peliculaDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();

            if (peliculaDTO.Actores is null)
            {
                return resultado;
            }

            foreach (var actor in peliculaDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.Id, Personaje = actor.Personaje });
            }

            return resultado;
        }
        #endregion

        #region .       Mapeo PeliculasDTO       .
        private List<GeneroDTO> MapearPeliculasGeneros(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<GeneroDTO>();

            if (pelicula.PeliculasGeneros is null)
            {
                return resultado;
            }

            foreach (var genero in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = genero.GeneroId, Nombre = genero.Genero.Nombre });
            }

            return resultado;
        }

        private List<PeliculaActorDTO> MapearPeliculasActores(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<PeliculaActorDTO>();

            if (pelicula.PeliculasActores is null)
            {
                return resultado;
            }

            foreach (var actor in pelicula.PeliculasActores)
            {
                resultado.Add(new PeliculaActorDTO() { Id = actor.ActorId, Nombre = actor.Actor.Nombre, Foto = actor.Actor.FotoURL, Personaje = actor.Personaje, Orden = actor.Orden });
            }

            return resultado;
        }

        private List<CineDTO> MapearPeliculasCines(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<CineDTO>();

            if (pelicula.PeliculasCines is null)
            {
                return resultado;
            }

            foreach (var cine in pelicula.PeliculasCines)
            {
                resultado.Add(new CineDTO() { Id = cine.CineId, Nombre = cine.Cine.Nombre, Lat = cine.Cine.Ubicacion.Y, Lng = cine.Cine.Ubicacion.X });
            }

            return resultado;
        }
        #endregion
    }
}
