using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;
using PeliculasAPI.Models;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;

        public ActoresController(ApplicationDbContext context, IMapper mapper,
                                 IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet("GetActores")]
        public async Task<IActionResult> GetActores()
        {
            var actores = await context.Actores.ToListAsync();
            return Ok(actores);
        }

        [HttpGet("GetActor/{id:int}")]
        public async Task<IActionResult> GetActor(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
            {
                return NotFound();
            }

            return Ok(actor);
        }

        [HttpPost("PostActor")]
        public async Task<IActionResult> PostActor([FromForm] ActorDTO model)
        {
            var actor = mapper.Map<Actor>(model);
            var result = await almacenadorArchivos.Almacenar(model.Foto);

            actor.FotoURL = result.URL;
            await context.Actores.AddAsync(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("PutActor/{id:int}")]
        public async Task<IActionResult> PutActor(int id, [FromForm] ActorDTO model)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
            {
                return NotFound();
            }

            actor = mapper.Map(model, actor);

            if (model.Foto is not null)
            {
                var key = GetKey(actor.FotoURL);

                var result = await almacenadorArchivos.Actualizar(key, model.Foto);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("DeleteActor/{id:int}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
            {
                return NotFound();
            }

            var key = GetKey(actor.FotoURL);

            await almacenadorArchivos.Borrar(key);
            context.Actores.Remove(actor);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private string GetKey(string url)
        {
            return url.Split('/').Last();
        }
    }
}
