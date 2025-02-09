using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACME.Web.Pages.Cliente
{
    [RequireHttps]
    [AutoValidateAntiforgeryToken]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Clientes> Clientes { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            Clientes = [];

            var clientes = await GetClientes();

            if (clientes == null || clientes.Count() == 0)
            {
                ModelState.AddModelError(string.Empty, "No hay clientes para obtener");
                return Page();
            }

            Clientes = clientes.Select(x => new Clientes
            {
                Id = x.Id.Value,
                Nombre = x.Nombre,
                Direccion = x.Direccion
            }).ToList();
            return Page();
        }

        private async Task<IEnumerable<ClientesDto>> GetClientes()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Clientes/GetClientes");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var clientes = await response.Content.ReadFromJsonAsync<IEnumerable<ClientesDto>>();
                        return clientes;
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
