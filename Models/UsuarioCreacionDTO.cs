using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Models
{
    public class UsuarioCreacionDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile Foto { get; set; }
    }
    public class UsuarioLoginDTO
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
