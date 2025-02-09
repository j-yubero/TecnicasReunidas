using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ACME.Web.Pages.Venta
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Guid IdVenta { get; set; }
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
            IdVenta = Id;
            Venta = await GetVentaById(Id);
            IdVisita = Venta.VisitaId;
            Productos = [];

            var productos = await GetProductos();

            Productos = productos.Select(x => new Productos
            {
                Descripcion = x.Descripcion,
                Id = x.Id,
                Nombre = x.Nombre,
                PVP = x.Precio,
                Stock = x.Stock
            }).ToList();

            Producto = Venta.Producto.Id;

            PrecioTotal = Venta.PrecioTotal;// PrecioUnitario * Unidades;
            PrecioUnitario = Venta.PrecioUnitario;
            Unidades = Venta.Unidades;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (ModelState.IsValid)
            //{
            var visita = await GetVisitaById(IdVisita);
            var venta = new VentaDto()
            {
                Id = Venta.Id,
                PrecioTotal = PrecioUnitario * Unidades,
                PrecioUnitario = PrecioUnitario,
                Unidades = Unidades,
                ProductoId = Producto,
                VisitaId = IdVisita,
                CreatedBy = HttpContext.Session.GetString("UserName")
            };

            var result = await UpdateVenta(venta);

            if (result)
                return RedirectToPage($"/Venta/Index");
            ModelState.AddModelError(string.Empty, "Error al guardar la venta");
            //}
            return Page();
        }

        private async Task<bool> UpdateVenta(VentaDto venta)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(venta);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"https://localhost:7039/Ventas/UPdate", httpContent);
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

        private async Task<VentaDto> GetVentaById(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Ventas/GetVentaById?Id={Id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return await response.Content.ReadFromJsonAsync<VentaDto>();

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
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
