using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.RestService.Repositories.Models
{
    public class Productos
    {
        [Key]
        public Guid Id { get; set; }
        [Required, NotNull]
        [MaxLength(50)]
        public string Nombre { get; set; }
        [Required, NotNull]
        [MaxLength(200)]
        public string Descripcion { get; set; }
        [Required, NotNull]
        public double Precio { get; set; }
        [Required, NotNull]
        public int Stock { get; set; }
        [Required, NotNull]
        public bool Activo { get; set; }
    }
}
