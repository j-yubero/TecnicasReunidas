using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ACME.Web.Pages.Usuario
{
    public class AddModel : PageModel
    {
        [BindProperty]
        public Guid Id { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public Guid Rol { get; set; }
        [BindProperty]
        public List<Roles> Roles { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Name = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            Roles = [];

            var roles = await GetRoles();
            Roles = roles.Select(x => new Roles()
            {
                Id = x.Id,
                Nombre = x.Nombre
            }).ToList();


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuario = new UsuarioDto()
            {
                UserName = UserName,
                Password = Password,
                RolId = Rol,
                Nombre = Name,
                CreatedBy = HttpContext.Session.GetString("UserName")
            };

            var result = await AddUsuario(usuario);

            if (result)
                return Redirect($"./Index");
            ModelState.AddModelError(string.Empty, "Error al guardar el cliente");
            return RedirectToPage();
        }

        private async Task<IEnumerable<RolDto>> GetRoles()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Rol/GetRoles");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return await response.Content.ReadFromJsonAsync<IEnumerable<RolDto>>();

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private async Task<bool> AddUsuario(UsuarioDto cliente)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(cliente);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://localhost:7039/Usuarios/Add", httpContent);
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
