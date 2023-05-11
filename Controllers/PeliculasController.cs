using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;
using PeliculasAPI.Models;
using PeliculasAPI.Servicios;
using PeliculasAPI.Utilidades;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;

        public PeliculasController(ApplicationDbContext context, IMapper mapper,
                                   IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet("GetPeliculas")]
        public async Task<IActionResult> GetPeliculas()
        {
            var top = 6;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(x => x.FechaEstreno > hoy)
                .OrderBy(x => x.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var enCines = await context.Peliculas
                .Where(x => x.EnCines)
                .OrderBy(x => x.FechaEstreno)
                .Take(top)
                .ToListAsync();

            var resultado = new LandingPageDTO();
            resultado.ProximosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

            return Ok(resultado);
        }

        [HttpGet("GetPelicula/{id:int}")]
        public async Task<IActionResult> GetPelicula(int id)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculasCines).ThenInclude(x => x.Cine)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(pelicula is null)
            {
                return NotFound();
            }

            var model = mapper.Map<PeliculaDTO>(pelicula);
            model.Actores = model.Actores.OrderBy(x => x.Orden).ToList();

            return Ok(model);
        }

        [HttpGet("Filtrar")]
        public async Task<IActionResult> Filtrar([FromQuery] PeliculasFiltroDTO model)
        {
            var peliculasQuery = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(model.Titulo))
            {
                peliculasQuery = peliculasQuery.Where(x => x.Titulo.Contains(model.Titulo));
            }

            if (model.EnCines)
            {
                peliculasQuery = peliculasQuery.Where(x => x.EnCines);
            }

            if (model.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQuery = peliculasQuery.Where(x => x.FechaEstreno > hoy);
            }

            if (model.GeneroId != 0)
            {
                peliculasQuery = peliculasQuery.Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                                   .Contains(model.GeneroId));
            }

            var peliculas = await peliculasQuery.ToListAsync();

            return Ok(mapper.Map<List<PeliculaDTO>>(peliculas));
        }

        [HttpPost("PostPelicula")]
        public async Task<IActionResult> PostPelicula([FromForm] PeliculaCreacionDTO model)
        {
            var pelicula = mapper.Map<Pelicula>(model);

            var result = await almacenadorArchivos.Almacenar(model.Poster);

            pelicula.PosterUrl = result.URL;

            EscribirOrdenActores(pelicula);

            await context.AddAsync(pelicula);
            await context.SaveChangesAsync();

            return Ok(pelicula.Id);
        }

        [HttpPut("PutPelicula/{id:int}")]
        public async Task<IActionResult> PutPelicula(int id, [FromForm] PeliculaCreacionDTO model)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasActores)
                .Include(x => x.PeliculasGeneros)
                .Include(x => x.PeliculasCines)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(pelicula is null)
            {
                return NotFound();
            }

            pelicula = mapper.Map(model, pelicula);

            if(model.Poster is not null)
            {
                var key = GetKey(pelicula.PosterUrl);

                var result = await almacenadorArchivos.Actualizar(key, model.Poster);
                pelicula.PosterUrl = result.URL;
            }

            EscribirOrdenActores(pelicula);

            await context.SaveChangesAsync();

            return Ok(pelicula.Id);
        }

        [HttpDelete("DeletePelicula/{id:int}")]
        public async Task<IActionResult> DeletePelicula(int id)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if(pelicula is null)
            {
                return NotFound();
            }

            context.Remove(pelicula);
            await context.SaveChangesAsync();

            var key = GetKey(pelicula.PosterUrl);

            await almacenadorArchivos.Borrar(key);

            return Ok(pelicula.Id);
        }

        private string GetKey(string url)
        {
            return url.Split('/').Last();
        }

        private void EscribirOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores is not null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }
    }
}
