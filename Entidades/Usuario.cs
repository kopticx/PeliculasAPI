using Microsoft.AspNetCore.Identity;

namespace PeliculasAPI.Entidades
{
    public class Usuario : IdentityUser
    {
        public string FotoUrl { get; set; }
    }
}
