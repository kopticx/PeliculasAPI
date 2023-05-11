using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Models
{
    public class GeneroCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Descripcion { get; set; }
    }

    public class GeneroDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
