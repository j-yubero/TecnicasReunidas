using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ACME.Web.Pages.Producto
{
    public class AddModel : PageModel
    {
        [BindProperty]
        public Guid IdProducto { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Description { get; set; }
        [BindProperty]
        public double Precio { get; set; }
        [BindProperty]
        public int Stock { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Name = string.Empty;
            Description = string.Empty;
            Precio = 0.0;
            Stock = 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var producto = new ProductoDto()
            {
                Id = IdProducto,
                Descripcion = Description,
                Nombre = Name,
                Precio = Precio,
                Stock = Stock,
                CreatedBy = HttpContext.Session.GetString("UserName")
            };

            var result = await AddProducto(producto);

            if (result)
                return RedirectToPage($"/Producto/Index");
            ModelState.AddModelError(string.Empty, "Error al guardar la venta");
            //}
            return Page();
        }

        private async Task<bool> AddProducto(ProductoDto producto)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(producto);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://localhost:7039/Productos/Add", httpContent);
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private async Task<ProductoDto> GetProductoById(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Productos/GetProductoById?Id={Id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var productos = await response.Content.ReadFromJsonAsync<ProductoDto>();
                        return productos;
                    }

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
