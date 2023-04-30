using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;
using PeliculasAPI.Models;

namespace PeliculasAPI.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("GetGeneros")]
        public async Task<IActionResult> GetGeneros()
        {
            var generos = await context.Generos.ToListAsync();

            return Ok(generos);
        }

        [HttpGet("GetGenero/{id:int}")]
        public async Task<IActionResult> GetGenero(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (genero is null)
            {
                return NotFound();
            }

            return Ok(genero);
        }

        [HttpPost("PostGenero")]
        public async Task<IActionResult> PostGenero([FromBody] GeneroDTO model)
        {
            var genero = mapper.Map<Genero>(model);

            await context.Generos.AddAsync(genero);
            await context.SaveChangesAsync();

            return Ok(genero);
        }

        [HttpPut("UpdateGenero/{id:int}")]
        public async Task<IActionResult> UpdateGenero(int id, [FromBody] GeneroDTO model)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            //Actualizar el genero
            genero.Nombre = model.Nombre;
            genero.Descripcion = model.Descripcion;

            context.Update(genero);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteGenero/{id:int}")]
        public async Task<IActionResult> DeleteGenero(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (genero is null)
            {
                return NotFound();
            }

            context.Remove(genero);
            await context.SaveChangesAsync  ();

            return NoContent();
        }
    }
}
