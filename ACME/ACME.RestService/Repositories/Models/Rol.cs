using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.RestService.Repositories.Models
{
    public class Rol
    {
        [Key]
        public Guid Id { get; set; }
        [Required, NotNull]
        [MaxLength(200)]
        public string Nombre { get; set; }
        [Required, NotNull]
        public bool CanCUDVisitas { get; set; }
        [Required, NotNull]
        public bool CanCUDVentas { get; set; }
        [Required, NotNull]
        public bool CanCUDClientes { get; set; }
        [Required, NotNull]
        public bool CanCUDUsuarios { get; set; }
        [Required, NotNull]
        public bool Activo {  get; set; }
    }
}
