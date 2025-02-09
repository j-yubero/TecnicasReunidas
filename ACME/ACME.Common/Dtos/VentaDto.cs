using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACME.Common.Dtos
{
    public class VentaDto
    {
        public Guid Id { get; set; }
        public VisitaDto? Visita { get; set; }
        public Guid VisitaId { get; set; }
        public ProductoDto? Producto { get; set; }
        public Guid ProductoId { get; set; }
        public double PrecioTotal { get; set; }
        public double PrecioUnitario { get; set; }
        public int Unidades { get; set; }
        public bool Activo { get; set; }
        public string CreatedBy { get; set; }
    }
}
