using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Text;

namespace ACME.Web.Pages.Cliente
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Guid Id { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Direccion { get; set; }
        [BindProperty]
        public List<Visitas> Visitas { get; set; }

        public async Task<IActionResult> OnGet(Guid Id)
        {
            var cliente = await GetClienteById(Id);

            Name = cliente.Nombre;
            Direccion = cliente.Direccion;
            var visitas = await GetVisitasByClienteId(Id);
            Visitas = visitas.Select(x=> new Visitas()
            {
                Id = x.Id.Value,
                ClienteId = x.ClienteId,
                Username = x.Usuario.UserName,
                Fecha = x.Fecha,
            }).ToList();

            foreach (var v in Visitas)
            {
                var ventasList = await GetVentasByVisita(v.Id);

                var PrecioTotal = 0.0;
                foreach (var vl in ventasList)
                    PrecioTotal += vl.PrecioTotal;

                v.NumeroVentas = ventasList.Count();
                v.TotalVentas = PrecioTotal;
            }

            return Page();
        }

        private async Task<IEnumerable<VentaDto>> GetVentasByVisita(Guid id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync($"https://localhost:7039/Ventas/GetVentasByVisitId?Id={id}");
                    response.Wait();
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var visita = await response.Result.Content.ReadFromJsonAsync<IEnumerable<VentaDto>>();
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

        public async Task<IActionResult> OnPostAsync()
        {
            var cliente = new ClientesDto()
            {
                Id = Id,
                Direccion = Direccion,
                Nombre = Name,
                CreatedBy = HttpContext.Session.GetString("UserName")
            };

            var result = await UpdateCliente(cliente);

            if (result)
                return Redirect($"./Index");
            ModelState.AddModelError(string.Empty, "Error al guardar el cliente");
            return RedirectToPage();
        }

        private async Task<bool> UpdateCliente(ClientesDto cliente)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(cliente);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"https://localhost:7039/Clientes/Update", httpContent);
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private async Task<ClientesDto> GetClienteById(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Clientes/GetClienteById?Id={Id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return await response.Content.ReadFromJsonAsync<ClientesDto>();
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private async Task<IEnumerable<VisitaDto>> GetVisitasByClienteId(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7039/Visitas/GetVisitasByClienteId?Id={Id}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return await response.Content.ReadFromJsonAsync<IEnumerable<VisitaDto>>();
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
