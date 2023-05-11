using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PeliculasAPI.Entidades;
using PeliculasAPI.Models;
using PeliculasAPI.Servicios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeliculasAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager,
                              ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos,
                              IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.configuration = configuration;
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromForm] UsuarioCreacionDTO model)
        {
            var usuario = new Usuario() { Email = model.Email, UserName = model.UserName };
            var resultado = await userManager.CreateAsync(usuario, model.Password);

            if(resultado.Succeeded)
            {
                var result = await almacenadorArchivos.Almacenar(model.Foto);
                usuario.FotoUrl = result.URL;

                var modelToken = new UsuarioLoginDTO
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    Password = model.Password,
                };

                context.Update(usuario);
                await context.SaveChangesAsync();

                var tokenResponse = await ConstruirToken(modelToken);

                return Ok(tokenResponse);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }
            
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO model)
        {
            var usuario = await context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            model.UserName = usuario.UserName;

            var resultado = await signInManager.PasswordSignInAsync(usuario, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                var tokenResponse = await ConstruirToken(model);

                return Ok(tokenResponse);
            }
            else
            {
                return BadRequest("No se pudo iniciar sesión");
            }
        }
         
        private async Task<RespuestaAutenticacion> ConstruirToken(UsuarioLoginDTO model)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", model.Email),
                new Claim("username", model.UserName)
            };

            var usuario = await userManager.FindByEmailAsync(model.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("jwt")));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: credenciales);

            return new RespuestaAutenticacion
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion,
                FotoUsuario = usuario.FotoUrl
            };
        }
    }
}
