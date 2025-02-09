using ACME.Common.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ACME.Web.Models
{
    public class Visitas
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Vendedor"), DataType(DataType.Text)]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Cliente"), DataType(DataType.Text)]
        public string Cliente { get; set; }
        public Guid ClienteId { get; set; }
        [Required]
        [Display(Name = "Número de ventas"), DataType(DataType.Text)]
        public int NumeroVentas { get; set; }

        [Required]
        [Range(1, 99999), Display(Name = "Precio Total"), DataType(DataType.Currency)]
        public double TotalVentas { get; set; }
        [Required]
        [Display(Name = "Fecha"), DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; }


        public static Visitas VisitaDtoToVisita(VisitaDto dto, int numeroVentas, double precioTotal)
        {

            return new Visitas
            {
                Id = dto.Id ?? Guid.NewGuid(),
                Username = dto.Usuario.UserName,
                Cliente = dto.Cliente.Nombre,
                NumeroVentas = numeroVentas,
                TotalVentas = precioTotal,
                Fecha = dto.Fecha
            };
        }

    }
}
