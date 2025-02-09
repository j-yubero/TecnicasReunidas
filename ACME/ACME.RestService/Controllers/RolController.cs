using ACME.Common.Dtos;
using ACME.RestService.Repositories;
using ACME.RestService.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ACME.RestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolController> _logger;

        public RolController(ILogger<RolController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetRoles")]
        public IEnumerable<RolDto> GetRoles(bool inactivos = false)
        {
            try
            {
                List<Rol> roles = [];
                if (inactivos)
                    roles = [.. _context.Roles];
                else
                    roles = [.. _context.Roles.Where(x => x.Activo)];

                return roles.Select(x => new RolDto
                {
                    Id = x.Id,
                    CanCUDClientes = x.CanCUDClientes,
                    CanCUDUsuarios = x.CanCUDUsuarios,
                    CanCUDVentas = x.CanCUDVentas,
                    CanCUDVisitas = x.CanCUDVisitas,
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

                _logger.LogError($"GetRoles Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetRolesByName")]
        public IEnumerable<RolDto> GetRolesByName(string nombre, bool inactivos = false)
        {
            try
            {
                List<Rol> roles = [];
                if (inactivos)
                    roles = [.. _context.Roles.Where(x => x.Nombre.Contains(nombre))];
                else
                    roles = [.. _context.Roles.Where(x => x.Nombre.Contains(nombre) && x.Activo)];

                return roles.Select(x => new RolDto
                {
                    Id = x.Id,
                    CanCUDClientes = x.CanCUDClientes,
                    CanCUDUsuarios = x.CanCUDUsuarios,
                    CanCUDVentas = x.CanCUDVentas,
                    CanCUDVisitas = x.CanCUDVisitas,
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

                _logger.LogError($"GetRolesByName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetRolById")]
        public RolDto GetRolById(Guid id, bool inactivos = false)
        {
            try
            {
                Rol rol = null;
                if (inactivos)
                    rol = _context.Roles.FirstOrDefault(x => x.Id.Equals(id));
                else
                    rol = _context.Roles.FirstOrDefault(x => x.Id.Equals(id) && x.Activo);

                if (rol == null)
                {
                    _logger.LogWarning($"No existe ningún rol con el ID [{id}]");
                    throw new Exception($"No existe ningún rol con el ID [{id}]");
                }

                return new RolDto
                {
                    Id = rol.Id,
                    CanCUDClientes = rol.CanCUDClientes,
                    CanCUDUsuarios = rol.CanCUDUsuarios,
                    CanCUDVentas = rol.CanCUDVentas,
                    CanCUDVisitas = rol.CanCUDVisitas,
                    Nombre = rol.Nombre,
                    Activo = rol.Activo
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

                _logger.LogError($"GetRolById Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(RolDto rolDto)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName.Equals(rolDto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                var rol = new Rol
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    CanCUDClientes = rolDto.CanCUDClientes,
                    CanCUDUsuarios = rolDto.CanCUDUsuarios,
                    CanCUDVentas = rolDto.CanCUDVentas,
                    CanCUDVisitas = rolDto.CanCUDVisitas,
                    Nombre = rolDto.Nombre
                };

                var rolInsertado = _context.Roles.Add(rol);
                _context.SaveChanges();
                _logger.LogInformation($"Rol [{JsonSerializer.Serialize(rolDto)}] añadido correctamente");

                var historial = new Historial()
                {
                    Accion = "Add",
                    Fecha = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Usuario = usuario,
                    UsuarioId = usuario.Id
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
        public IActionResult Update(RolDto rolDto)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName.Equals(rolDto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                var rol = _context.Roles.FirstOrDefault(x => x.Id == rolDto.Id);

                if (rol == null)
                    throw new Exception($"El rol {rol.Nombre} no existe");

                var changes = false;
                if (rol.Nombre != rolDto.Nombre)
                {
                    rol.Nombre = rolDto.Nombre;
                    changes = true;
                }
                if (rol.CanCUDClientes != rolDto.CanCUDClientes)
                {
                    rol.CanCUDClientes = rolDto.CanCUDClientes;
                    changes = true;
                }
                if (rol.CanCUDUsuarios != rolDto.CanCUDUsuarios)
                {
                    rol.CanCUDUsuarios = rolDto.CanCUDUsuarios;
                    changes = true;
                }
                if (rol.CanCUDVentas != rolDto.CanCUDVentas)
                {
                    rol.CanCUDVentas = rolDto.CanCUDVentas;
                    changes = true;
                }
                if (rol.CanCUDVisitas != rolDto.CanCUDVisitas)
                {
                    rol.CanCUDVisitas = rolDto.CanCUDVisitas;
                    changes = true;
                }

                if (changes)
                {
                    _context.Roles.Update(rol);
                    _context.SaveChanges();
                    _logger.LogInformation($"Rol [{JsonSerializer.Serialize(rol)}] actualizado correctamente");

                    var historial = new Historial()
                    {
                        Accion = "Update",
                        Fecha = DateTime.Now,
                        Id = Guid.NewGuid(),
                        Usuario = usuario,
                        UsuarioId = usuario.Id
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
        public IActionResult Delete(Guid Id, string username)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName.Equals(username));

                if (usuario == null)
                    return StatusCode(401);

                var rol = _context.Roles.FirstOrDefault(x => x.Id == Id);

                if (rol == null)
                    throw new Exception($"El rol {rol.Nombre} no existe");

                rol.Activo = false;
                _context.Roles.Update(rol);
                _context.SaveChanges();
                _logger.LogInformation($"Rol [{JsonSerializer.Serialize(rol)}] eliminado correctamente");

                var historial = new Historial()
                {
                    Accion = "Delete",
                    Fecha = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Usuario = usuario,
                    UsuarioId = usuario.Id
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
