using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.Web.Models
{
    public class Ventas
    {
        public Guid Id { get; set; }
        public string Producto { get; set; }
        public double PrecioUnitario { get; set; }
        public double PrecioTotal { get; set; }

        public int Unidades { get;set; }
        public string Cliente { get; set; }

    }
}
