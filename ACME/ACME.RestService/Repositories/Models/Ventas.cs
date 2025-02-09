using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.RestService.Repositories.Models
{
    public class Ventas
    {
        [Key]
        public Guid Id { get; set; }
        [Required, NotNull]
        public Guid VisitaId { get; set; }
        public Visitas Visita { get; set; }
        [Required, NotNull]
        public Guid ProductoId { get; set; }
        public Productos Producto { get; set; }
        [Required, NotNull]
        public double PrecioUnitario{ get; set; }
        [Required, NotNull]
        public int Unidades { get; set; }
        [Required, NotNull]
        public double PrecioTotal { get; set; }
        [Required, NotNull]
        public bool Activo { get; set; }
    }
}
