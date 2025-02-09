using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ACME.Web.Models
{
    public class Clientes
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Nombre"), DataType(DataType.Text)]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Direccion"), DataType(DataType.Text)]
        public string Direccion { get; set; }
    }

    public class ClientesDropDown
    {
        public Guid Id { get; set; }
        [Display(Name = "Cliente"), DataType(DataType.Text)]
        public IEnumerable<SelectListItem> Clientes { get; set; }

        public ClientesDropDown()
        {
            Id = Guid.Empty;
            Clientes = [];
        }
    }
}
