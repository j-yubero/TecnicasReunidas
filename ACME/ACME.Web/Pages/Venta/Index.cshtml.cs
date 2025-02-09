using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACME.Web.Pages.Venta
{
    [RequireHttps]
    [AutoValidateAntiforgeryToken]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Ventas> Ventas { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            Ventas = [];

            var ventas = await GetVentas();

            if (ventas == null || ventas.Count() == 0)
            {
                ModelState.AddModelError(string.Empty, "No hay ventas para obtener");
                return Page();
            }

            Ventas = ventas.Select( x => new Ventas
            {
                Id = x.Id,
                Producto = x.Producto.Nombre,
                PrecioUnitario = x.PrecioUnitario,
                PrecioTotal = x.PrecioTotal,
                Unidades = x.Unidades,
                Cliente = x.Visita.ClienteId.ToString()
                
            }).ToList();

            foreach (var v in Ventas)
            {
                v.Cliente = await GetClienteNameById(Guid.Parse(v.Cliente));
            }

            return Page();
        }



        private async Task<string> GetClienteNameById(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync($"https://localhost:7039/Clientes/GetClienteById?Id={Id}");
                    response.Wait();
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var cliente = await response.Result.Content.ReadFromJsonAsync<ClientesDto>();
                        return cliente.Nombre;
                    }

                    return string.Empty;

                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        private async Task<IEnumerable<VentaDto>> GetVentas()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync($"https://localhost:7039/Ventas/GetVentas");
                    response.Wait();
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var visita = await response.Result.Content.ReadFromJsonAsync<IEnumerable<VentaDto>>();
                        return visita;
                    }

                    return [];

                }
                catch (Exception ex)
                {
                    return [];
                }
            }
        }
    }
}
