using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.RestService.Repositories.Models
{
    public class Usuarios
    {
        [Key]
        public Guid Id { get; set; }
        [Required, NotNull]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required, NotNull]
        [MaxLength(50)]
        public string Password { get; set; }
        [Required, NotNull]
        [MaxLength(200)]
        public string Nombre { get; set; }
        [Required, NotNull]
        public Guid RolId { get; set; }

        public Rol Rol { get; set; }
        [Required, NotNull]
        public bool Activo { get; set; }

    }
}
