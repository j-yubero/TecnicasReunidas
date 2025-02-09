using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ACME.Web.Pages.Visita
{
    public class AddVisitaModel : PageModel
    {

        [BindProperty]
        public Visitas Visita { get; set; }

        [BindProperty]
        public Guid Cliente { get; set; }
        [BindProperty]
        public List<Clientes> Clientes { get; set; }
        [BindProperty]
        public List<Ventas> VentasView { get; set; }
        [BindProperty]
        public int NumeroVentas { get; set; }
        [BindProperty]
        public double TotalVentas { get; set; }

        public async Task<IActionResult> OnGet()
        {
            VentasView = [];
            Clientes = [];

            var clientes = await GetClientes();

            Clientes = clientes.Select(x => new Clientes
            {
                Id = x.Id.Value,
                Direccion = x.Direccion,
                Nombre = x.Nombre,
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var username = HttpContext.Session.GetString("UserName");
            var usuario = await GetUserByUserName(username);
            var visit = new VisitaDto()
            {
                ClienteId = Cliente,
                UsuarioId = usuario.Id,
                Fecha = DateTime.Now,
                CreatedBy = username
            };
            var result = await AddVisit(visit);
            if (result != null)
                return Redirect($"./Edit?Id={result.Id}");
            ModelState.AddModelError(string.Empty, "Usuario y/o contraseña incorrectos");
            return Page();
        }

        private async Task<VisitaDto> AddVisit(VisitaDto visitaDto)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(visitaDto);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://localhost:7039/Visitas/Add", httpContent);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var visita = await response.Content.ReadFromJsonAsync<VisitaDto>();
                        return visita;
                    }

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private async Task<UsuarioDto> GetUserByUserName(string nombre)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Usuarios/GetUsersByName?nombre={nombre}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var usuario = await response.Content.ReadFromJsonAsync<IEnumerable<UsuarioDto>>();
                        return usuario.FirstOrDefault(x => x.UserName.Equals(nombre));
                    }

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private async Task<IEnumerable<ClientesDto>> GetClientes()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Clientes/GetClientes");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return await response.Content.ReadFromJsonAsync<IEnumerable<ClientesDto>>();

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
