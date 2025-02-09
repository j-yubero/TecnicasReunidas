using ACME.Common.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ACME.Web.Pages.Cliente
{
    public class AddModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Direccion { get; set; }
        
        public async Task<IActionResult> OnGet()
        {
            Name = string.Empty;
            Direccion = string.Empty;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cliente = new ClientesDto()
            {
                Direccion = Direccion,
                Nombre = Name,
                CreatedBy = HttpContext.Session.GetString("UserName")
            };

            var result = await AddCliente(cliente);

            if (result)
                return RedirectToPage($"/Cliente/Index");
            ModelState.AddModelError(string.Empty, "Error al guardar el cliente");
            return Page();
        }

        private async Task<bool> AddCliente(ClientesDto cliente)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(cliente);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://localhost:7039/Clientes/Add", httpContent);
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
