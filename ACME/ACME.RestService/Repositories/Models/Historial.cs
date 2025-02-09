using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.RestService.Repositories.Models
{
    public class Historial
    {
        [Key]
        public Guid Id { get; set; }
        [Required, NotNull]
        public Guid UsuarioId { get; set; }

        public Usuarios Usuario { get; set; }
        [Required, NotNull]
        [MaxLength(200)]
        public string Accion { get; set; }

        [Required, NotNull]
        public DateTime Fecha { get; set; }

    }
}
