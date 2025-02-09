using ACME.Common.Dtos;
using ACME.RestService.Repositories;
using ACME.RestService.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace ACME.RestService.Controllers
{
    public class ClientRequestDto
    {
        public string UserName { get; set; }
        public DateTime Fecha { get; set; }
        public ClientesDto Cliente { get; set; }
        public ClientRequestDto(string username, DateTime fecha, Clientes cliente)
        {
            UserName = username;
            Fecha = fecha;
            Cliente = new ClientesDto
            {
                Id = cliente.Id,
                Activo = cliente.Activo,
                Direccion = cliente.Direccion,
                Nombre = cliente.Nombre
            };
        }
    }


    [ApiController]
    [Route("[controller]")]
    public class ClientesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(ILogger<ClientesController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetClientes")]
        public IEnumerable<ClientesDto> GetClientes(bool inactivos = false)
        {
            try
            {
                List<Clientes> clientes = [];
                if (inactivos)
                    clientes = [.. _context.Clientes];
                else
                    clientes = [.. _context.Clientes.Where(x => x.Activo)];

                return clientes.Select(x => new ClientesDto
                {
                    Id = x.Id,
                    Direccion = x.Direccion,
                    Nombre = x.Nombre,
                    Activo = x.Activo
                }).ToList();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $"{Environment.NewLine}{innerException.Message}";
                    innerException = innerException.InnerException;
                }

                _logger.LogError($"GetClients Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetClientsByName")]
        public IEnumerable<ClientesDto> GetClientsByName(string nombre, bool inactivos = false)
        {
            try
            {
                List<Clientes> clientes = [];
                if (inactivos)
                    clientes = [.. _context.Clientes.Where(x => x.Nombre.Contains(nombre))];
                else
                    clientes = [.. _context.Clientes.Where(x => x.Nombre.Contains(nombre) && x.Activo)];

                return clientes.Select(x => new ClientesDto
                {
                    Id = x.Id,
                    Direccion = x.Direccion,
                    Nombre = x.Nombre,
                    Activo = x.Activo
                }).ToList();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $"{Environment.NewLine}{innerException.Message}";
                    innerException = innerException.InnerException;
                }

                _logger.LogError($"GetClientsByName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetClienteById")]
        public ClientesDto? GetClienteById(Guid id, bool inactivos = false)
        {
            try
            {
                Clientes? cliente = null;
                if (inactivos)
                    cliente = _context.Clientes.FirstOrDefault(x => x.Id.Equals(id));
                else
                    cliente = _context.Clientes.FirstOrDefault(x => x.Id.Equals(id) && x.Activo);

                if (cliente == null)
                {
                    _logger.LogWarning($"No existe ningún cliente con el ID [{id}]");
                    return null;
                }

                return new ClientesDto
                {
                    Id = cliente.Id,
                    Direccion = cliente.Direccion,
                    Nombre = cliente.Nombre,
                    Activo = cliente.Activo,
                };
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $"{Environment.NewLine}{innerException.Message}";
                    innerException = innerException.InnerException;
                }

                _logger.LogError($"GetClientById Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(ClientesDto cliente)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x=> x.Rol).FirstOrDefault(x=> x.UserName == cliente.CreatedBy);

                if(usuario == null)
                    return StatusCode(401);

                if(!usuario.Rol.CanCUDClientes)
                    return StatusCode(401);


                var client = new Clientes
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    Direccion = cliente.Direccion,
                    Nombre = cliente.Nombre
                }; 

                var clienteInsertado = _context.Clientes.Add(client);
                _context.SaveChanges();
                _logger.LogInformation($"Cliente [{JsonSerializer.Serialize(cliente)}] añadido correctamente");

                var historial = new Historial
                {
                    Accion = "Add",
                    Fecha = DateTime.Now,
                    UsuarioId = usuario.Id,
                    Usuario = usuario,
                    Id = Guid.NewGuid()
                };

                _context.Historial.Add(historial);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $"{Environment.NewLine}{innerException.Message}";
                    innerException = innerException.InnerException;
                }

                _logger.LogError($"Add Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Update(ClientesDto cliente)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName == cliente.CreatedBy);

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDClientes)
                    return StatusCode(401);

                var client = _context.Clientes.FirstOrDefault(x => x.Id == cliente.Id);

                if (client == null)
                    throw new Exception($"El cliente no existe");

                var changes = false;
                if(client.Nombre != cliente.Nombre)
                {
                    client.Nombre = cliente.Nombre;
                    changes = true;
                }
                if (client.Direccion != cliente.Direccion)
                {
                    client.Direccion = cliente.Direccion;
                    changes = true;
                }

                if (changes)
                {
                    _context.Clientes.Update(client);
                    _context.SaveChanges();
                    _logger.LogInformation($"Cliente [{JsonSerializer.Serialize(cliente)}] actualizado correctamente");

                    var historial = new Historial
                    {
                        Accion = "Update",
                        Fecha = DateTime.Now,
                        UsuarioId = usuario.Id,
                        Usuario = usuario,
                        Id = Guid.NewGuid()
                    };

                    _context.Historial.Add(historial);
                    _context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $"{Environment.NewLine}{innerException.Message}";
                    innerException = innerException.InnerException;
                }

                _logger.LogError($"Update Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(Guid id, string username)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName == username);

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDClientes)
                    return StatusCode(401);

                var cliente = _context.Clientes.FirstOrDefault(x => x.Id == id);

                if (cliente == null)
                    throw new Exception($"El cliente no existe");

                cliente.Activo = false;
                _context.Clientes.Update(cliente);
                _context.SaveChanges();
                _logger.LogInformation($"Cliente [{JsonSerializer.Serialize(cliente)}] eliminado correctamente");

                var historial = new Historial
                {
                    Accion = "Delete",
                    Fecha = DateTime.Now,
                    UsuarioId = usuario.Id,
                    Usuario = usuario,
                    Id = Guid.NewGuid()
                };

                _context.Historial.Add(historial);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    errorMessage += $"{Environment.NewLine}{innerException.Message}";
                    innerException = innerException.InnerException;
                }

                _logger.LogError($"Delete Error:[{errorMessage}]");

                throw;
            }
        }
    }
}
