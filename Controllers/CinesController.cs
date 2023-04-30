using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;
using PeliculasAPI.Models;

namespace PeliculasAPI.Controllers
{
    [Route("api/cines")]
    [ApiController]
    public class CinesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CinesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("GetCines")]
        public async Task<IActionResult> GetCines()
        {
            var listado = await context.Cines.ToListAsync();

            var cines = mapper.Map<List<CineDTO>>(listado);

            return Ok(cines);
        }

        [HttpPost("PostCine")]
        public async Task<IActionResult> PostCine([FromBody] CineDTO model)
        {
            var cine = mapper.Map<Cine>(model);

            await context.AddAsync(cine);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("PutCine/{id:int}")]
        public async Task<IActionResult> PutCine(int id, [FromBody] CineDTO model)
        {
            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == id);

            if(cine is null)
            {
                return NotFound();
            }

            cine = mapper.Map(model, cine);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteCine/{id:int}")]
        public async Task<IActionResult> DeleteCine(int id)
        {
            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == id);

            if(cine is null)
            {
                return NotFound();
            }

            context.Remove(cine);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
