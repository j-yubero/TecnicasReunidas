using ACME.Common.Dtos;
using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ACME.Web.Pages.Visita
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Guid IdVisita { get; set; }
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

        public async Task<IActionResult> OnGet(Guid id)
        {
            IdVisita = id;
            VentasView = [];
            Clientes = [];

            var clientes = await GetClientes();

            Clientes = clientes.Select(x => new Clientes
            {
                Id = x.Id.Value,
                Direccion = x.Direccion,
                Nombre = x.Nombre,
            }).ToList();

            if (id.Equals(Guid.Empty))
                return Page();

            var visita = await GetVisitaById(id);

            if (visita == null)
            {
                ModelState.AddModelError(string.Empty, "No se ha encontrado la visita");
                return Page();
            }

            

            Cliente = visita.ClienteId;

            var ventas = await GetVentasByVisita(id);
            VentasView = ventas.Select(x => new Ventas
            {
                Id = x.Id,
                Producto = x.Producto.Nombre,
                PrecioUnitario = x.PrecioUnitario,
                PrecioTotal = x.PrecioTotal,
                Unidades = x.Unidades,
                Cliente = clientes.FirstOrDefault(x => x.Id.Equals(visita.ClienteId)).Nombre
            }).ToList();

            var ventasList = await GetVentasByVisita(id);

            var PrecioTotal = 0.0;
            foreach (var v in ventasList)
                PrecioTotal += v.PrecioTotal;

            NumeroVentas = ventasList.Count();
            TotalVentas = PrecioTotal;

            Visita = new Visitas()
            {
                Cliente = clientes.FirstOrDefault(x => x.Id.Equals(visita.ClienteId)).Nombre,
                Fecha = visita.Fecha,
                Id = visita.Id.Value,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var visita = await GetVisitaById(IdVisita);
            var visitaDto = new VisitaDto()
            {
                Id = IdVisita,
                ClienteId = Cliente,
                Fecha = visita.Fecha,
                UsuarioId = visita.UsuarioId,
                CreatedBy = HttpContext.Session.GetString("UserName")
            };

            var result = await UpdateVisita(visitaDto);

            if (result)
                return Redirect($"./Edit?Id={IdVisita}");
            ModelState.AddModelError(string.Empty, "Error al guardar la visita");
            //}
            return Page();
        }

        private async Task<VisitaDto> GetVisitaById(Guid id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync($"https://localhost:7039/Visitas/GetVisitaById?Id={id}");
                    response.Wait();
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var ventas = await response.Result.Content.ReadFromJsonAsync<VisitaDto>();
                        return ventas;
                    }

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
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

        private async Task<bool> UpdateVisita(VisitaDto visita)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(visita);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"https://localhost:7039/Visitas/Update", httpContent);
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return false;
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
