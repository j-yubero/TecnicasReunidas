using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ACME.Web.Pages.Visita
{
    public class AddVentaModel : PageModel
    {
        [BindProperty]
        public Guid Producto { get; set; }
        [BindProperty]
        public List<Productos> Productos { get; set; }
        [BindProperty]
        public Guid IdVisita { get; set; }
        [BindProperty]
        public VentaDto Venta { get; set; }
        [BindProperty]
        public double PrecioTotal { get; set; }
        [BindProperty]
        public double PrecioUnitario { get; set; }
        [BindProperty]
        public int Unidades { get; set; }

        public async Task<IActionResult> OnGet(Guid Id)
        {
            IdVisita = Id;
            Productos = [];
            Venta = new VentaDto();
            Venta.Visita = await GetVisitaById(Id);

            var productos = await GetProductos();

            Productos = productos.Select(x => new Productos
            {
                Descripcion = x.Descripcion,
                Id = x.Id,
                Nombre = x.Nombre,
                PVP = x.Precio,
                Stock = x.Stock
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (ModelState.IsValid)
            //{
            var visita = await GetVisitaById(IdVisita);
            var venta = new VentaDto()
            {
                PrecioTotal = PrecioUnitario * Unidades,
                PrecioUnitario = PrecioUnitario,
                Unidades = Unidades,
                ProductoId = Producto,
                VisitaId = IdVisita,
                CreatedBy = HttpContext.Session.GetString("UserName")
            };

            var result = await AddVenta(venta);

            if (result)
                return Redirect($"./Edit?Id={IdVisita}");
            ModelState.AddModelError(string.Empty, "Error al guardar la venta");
            //}
            return Page();
        }

        private async Task<bool> AddVenta(VentaDto venta)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(venta);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://localhost:7039/Ventas/Add", httpContent);
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private async Task<IEnumerable<ProductoDto>> GetProductos()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Productos/GetProductos");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var productos = await response.Content.ReadFromJsonAsync<IEnumerable<ProductoDto>>();
                        return productos;
                    }

                    return [];

                }
                catch (Exception ex)
                {
                    return [];
                }
            }
        }

        private async Task<VisitaDto> GetVisitaById(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Visitas/GetVisitaById?Id={Id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return await response.Content.ReadFromJsonAsync<VisitaDto>();

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
