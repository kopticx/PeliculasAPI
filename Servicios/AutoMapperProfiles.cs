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
            CreateMap<GeneroDTO, Genero>();

            CreateMap<ActorDTO, Actor>()
                .ForMember(x => x.FotoURL, options => options.Ignore());

            CreateMap<CineDTO, Cine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.Lng, dto.Lat))));

            CreateMap<Cine, CineDTO>()
                .ForMember(x => x.Lat, dto => dto.MapFrom(campo => campo.Ubicacion.Y))
                .ForMember(x => x.Lng, dto => dto.MapFrom(campo => campo.Ubicacion.X));
        }
    }
}
