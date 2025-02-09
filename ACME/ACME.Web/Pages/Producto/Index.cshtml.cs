using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACME.Web.Pages.Producto
{
    [RequireHttps]
    [AutoValidateAntiforgeryToken]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Productos> Productos { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            Productos = [];

            var productos = await GetProductos();

            if (productos == null || productos.Count() == 0)
            {
                ModelState.AddModelError(string.Empty, "No hay productos para obtener");
                return Page();
            }

            Productos = productos.Select(x => new Productos
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Descripcion = x.Descripcion,
                PVP = x.Precio,
                Stock = x.Stock
                
            }).ToList();
            return Page();
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
    }
}
