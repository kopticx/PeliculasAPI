using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Models
{
    public class GeneroDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }
    }
}
