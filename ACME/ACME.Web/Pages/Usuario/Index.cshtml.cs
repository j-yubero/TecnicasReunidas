using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACME.Web.Pages.Usuario
{
    [RequireHttps]
    [AutoValidateAntiforgeryToken]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Usuarios> Usuarios { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            Usuarios = [];

            var usuarios = await GetUsuarios();

            if (usuarios == null || usuarios.Count() == 0)
            {
                ModelState.AddModelError(string.Empty, "No hay usuarios para obtener");
                return Page();
            }

            Usuarios = usuarios.Select(x => new Usuarios
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Usuario = x.UserName,
                Rol = new Roles
                {
                    Id = x.Rol.Id,
                    Nombre = x.Rol.Nombre,
                    CUDClientes = x.Rol.CanCUDClientes,
                    CUDUsuarios = x.Rol.CanCUDUsuarios,
                    CUDVentas = x.Rol.CanCUDVentas,
                    CUDVisitas = x.Rol.CanCUDVisitas
                }
            }).ToList();
            return Page();
        }

        private async Task<IEnumerable<UsuarioDto>> GetUsuarios()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Usuarios/GetUsuarios");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var usuarios = await response.Content.ReadFromJsonAsync<IEnumerable<UsuarioDto>>();
                        return usuarios;
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
