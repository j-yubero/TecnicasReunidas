using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.RestService.Repositories.Models
{
    public class Visitas
    {
        [Key]
        public Guid Id { get; set; }
        [Required, NotNull]
        public Guid VendedorId { get; set; }
        public Usuarios Vendedor { get; set; }
        [Required, NotNull]
        public Guid ClienteId { get; set; }
        public Clientes Cliente { get; set; }
        [Required, NotNull]
        public bool Activo { get; set; }
        [Required, NotNull]
        public DateTime Fecha { get; set; }
    }
}
