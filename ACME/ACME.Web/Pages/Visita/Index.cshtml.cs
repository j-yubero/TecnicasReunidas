using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ACME.Web.Pages.Visita
{
    [RequireHttps]
    [AutoValidateAntiforgeryToken]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Visitas> Visitas { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            Visitas = [];
            var result = await GetVisitas();

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Error obteniendo las visitas");
                return Page();
            }

            foreach (var res in result)
            {
                var numeroVentas = await GetVentasPorVisita(res.Id.Value);
                var precioTotal = await CalcularPrecioTotal(res.Id.Value);
                Visitas.Add(Models.Visitas.VisitaDtoToVisita(res, numeroVentas, precioTotal));
            }
            return Page();
        }

        private async Task<IEnumerable<VisitaDto>> GetVisitas()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Visitas/GetVisitas");
                    if (response.IsSuccessStatusCode)
                    {
                        var listVisit = await response.Content.ReadFromJsonAsync<IEnumerable<VisitaDto>>();
                        return listVisit;
                    }

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private async Task<int> GetVentasPorVisita(Guid id)
        {
            if (id == Guid.Empty)
                return -1;
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync($"https://localhost:7039/Ventas/GetVentasByVisitId?id={id}");
                    response.Wait();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var listVenta = await response.Result.Content.ReadFromJsonAsync<IEnumerable<VentaDto>>();
                        if (listVenta == null)
                            return -1;
                        return listVenta.Count();
                    }

                    return -1;

                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        private async Task<double> CalcularPrecioTotal(Guid id)
        {
            if (id == Guid.Empty)
                return -1;

            double precioTotal = 0;
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync($"https://localhost:7039/Ventas/GetVentasByVisitId?id={id}");
                    response.Wait();
                    if (response.Result.IsSuccessStatusCode)
                    {
                        var listVenta = await response.Result.Content.ReadFromJsonAsync<IEnumerable<VentaDto>>();
                        if (listVenta == null)
                            return 0;

                        foreach (var venta in listVenta)
                        {
                            precioTotal += (venta.PrecioUnitario * venta.Unidades);
                        }
                        return precioTotal;
                    }

                    return 0;

                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

    }
}
