using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACME.Web.Pages.Rol
{
    [RequireHttps]
    [AutoValidateAntiforgeryToken]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Roles> Roles { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            Roles = [];

            var roles = await GetRoles();

            if (roles == null || roles.Count() == 0)
            {
                ModelState.AddModelError(string.Empty, "No hay roles para obtener");
                return Page();
            }

            Roles = roles.Select(x => new Roles
            {

                Id = x.Id,
                Nombre = x.Nombre,
                CUDClientes = x.CanCUDClientes,
                CUDUsuarios = x.CanCUDUsuarios,
                CUDVentas = x.CanCUDVentas,
                CUDVisitas = x.CanCUDVisitas

            }).ToList();
            return Page();
        }

        private async Task<IEnumerable<RolDto>> GetRoles()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Rol/GetRoles");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var roles = await response.Content.ReadFromJsonAsync<IEnumerable<RolDto>>();
                        return roles;
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
