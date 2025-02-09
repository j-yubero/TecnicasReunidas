using ACME.Common.Dtos;
using ACME.RestService.Repositories;
using ACME.RestService.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ACME.RestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitasController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<VisitasController> _logger;

        public VisitasController(ILogger<VisitasController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetVisitas")]
        public IEnumerable<VisitaDto> GetVisitas(bool inactivos = false)
        {
            try
            {
                List<Visitas> visitas = [];
                if (inactivos)
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor)];
                else
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Where(x => x.Activo)];

                return visitas.Select(x => new VisitaDto
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    Usuario = new UsuarioDto
                    {
                        Activo = x.Vendedor.Activo,
                        Id = x.Vendedor.Id,
                        Nombre = x.Vendedor.Nombre,
                        Password = x.Vendedor.Password,
                        RolId = x.Vendedor.RolId,
                        UserName = x.Vendedor.UserName
                    },
                    Cliente = new ClientesDto
                    {
                        Activo = x.Cliente.Activo,
                        Id = x.Cliente.Id,
                        Direccion = x.Cliente.Direccion,
                        Nombre = x.Cliente.Nombre
                    },
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

                _logger.LogError($"GetVisits Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVisitsByVendedor")]
        public IEnumerable<VisitaDto> GetVisitsByVendedor(string nombre, bool inactivos = false)
        {
            try
            {
                List<Visitas> visitas = [];
                if (inactivos)
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Where(x => x.Vendedor.UserName.Contains(nombre))];
                else
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Where(x => x.Vendedor.UserName.Contains(nombre) && x.Activo)];

                return visitas.Select(x => new VisitaDto
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    Usuario = new UsuarioDto
                    {
                        Activo = x.Vendedor.Activo,
                        Id = x.Vendedor.Id,
                        Nombre = x.Vendedor.Nombre,
                        Password = x.Vendedor.Password,
                        RolId = x.Vendedor.Rol.Id,
                        UserName = x.Vendedor.UserName
                    },
                    Cliente = new ClientesDto
                    {
                        Activo = x.Cliente.Activo,
                        Id = x.Cliente.Id,
                        Direccion = x.Cliente.Direccion,
                        Nombre = x.Cliente.Nombre
                    },
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

                _logger.LogError($"GetVisitsByVendedor Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVisitasByCliente")]
        public IEnumerable<VisitaDto> GetVisitasByCliente(string nombre, bool inactivos = false)
        {
            try
            {
                List<Visitas> visitas = [];
                if (inactivos)
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Where(x => x.Cliente.Nombre.Contains(nombre))];
                else
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Where(x => x.Cliente.Nombre.Contains(nombre) && x.Activo)];

                return visitas.Select(x => new VisitaDto
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    Usuario = new UsuarioDto
                    {
                        Activo = x.Vendedor.Activo,
                        Id = x.Vendedor.Id,
                        Nombre = x.Vendedor.Nombre,
                        Password = x.Vendedor.Password,
                        RolId = x.Vendedor.Rol.Id,
                        UserName = x.Vendedor.UserName
                    },
                    Cliente = new ClientesDto
                    {
                        Activo = x.Cliente.Activo,
                        Id = x.Cliente.Id,
                        Direccion = x.Cliente.Direccion,
                        Nombre = x.Cliente.Nombre
                    },
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

                _logger.LogError($"GetVisitsByCliente Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVisitasByClienteId")]
        public IEnumerable<VisitaDto> GetVisitasByClienteId(Guid Id, bool inactivos = false)
        {
            try
            {
                List<Visitas> visitas = [];
                if (inactivos)
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Include(x => x.Vendedor.Rol).Where(x => x.Cliente.Id.Equals(Id))];
                else
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Include(x => x.Vendedor.Rol).Where(x => x.Cliente.Id.Equals(Id) && x.Activo)];

                return visitas.Select(x => new VisitaDto
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    Usuario = new UsuarioDto
                    {
                        Activo = x.Vendedor.Activo,
                        Id = x.Vendedor.Id,
                        Nombre = x.Vendedor.Nombre,
                        Password = x.Vendedor.Password,
                        RolId = x.Vendedor.Rol.Id,
                        UserName = x.Vendedor.UserName
                    },
                    Cliente = new ClientesDto
                    {
                        Activo = x.Cliente.Activo,
                        Id = x.Cliente.Id,
                        Direccion = x.Cliente.Direccion,
                        Nombre = x.Cliente.Nombre
                    },
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

                _logger.LogError($"GetVisitsByCliente Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVisitsByClienteVendedor")]
        public IEnumerable<VisitaDto> GetVisitsByClienteVendedor(string username, string clientname, bool inactivos = false)
        {
            try
            {
                List<Visitas> visitas = [];
                if (inactivos)
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Where(x => x.Cliente.Nombre.Contains(clientname) && x.Vendedor.UserName.Contains(username))];
                else
                    visitas = [.. _context.Visitas.Include(x => x.Cliente).Include(x => x.Vendedor).Where(x => x.Cliente.Nombre.Contains(clientname) && x.Vendedor.UserName.Contains(username) && x.Activo)];

                return visitas.Select(x => new VisitaDto
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    Usuario = new UsuarioDto
                    {
                        Activo = x.Vendedor.Activo,
                        Id = x.Vendedor.Id,
                        Nombre = x.Vendedor.Nombre,
                        Password = x.Vendedor.Password,
                        RolId = x.Vendedor.Rol.Id,
                        UserName = x.Vendedor.UserName
                    },
                    Cliente = new ClientesDto
                    {
                        Activo = x.Cliente.Activo,
                        Id = x.Cliente.Id,
                        Direccion = x.Cliente.Direccion,
                        Nombre = x.Cliente.Nombre
                    },
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

                _logger.LogError($"GetVisitsByClienteVendedor Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVisitaById")]
        public VisitaDto? GetVisitaById(Guid id, bool inactivos = false)
        {
            try
            {
                Visitas? visita = null;
                if (inactivos)
                    visita = _context.Visitas.FirstOrDefault(x => x.Id.Equals(id));
                else
                    visita = _context.Visitas.FirstOrDefault(x => x.Id.Equals(id) && x.Activo);

                if (visita == null)
                {
                    _logger.LogWarning($"No existe ningún visita con el ID [{id}]");
                    return null;
                }

                return new VisitaDto
                {
                    Id = visita.Id,
                    Fecha = visita.Fecha,
                    UsuarioId = visita.VendedorId,
                    ClienteId = visita.ClienteId,
                    Activo = visita.Activo
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

                _logger.LogError($"GetVisitById Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(VisitaDto visita)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(visita.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDVisitas)
                    return StatusCode(401);

                if (visita.ClienteId == Guid.Empty)
                    return StatusCode(400);

                if (visita.UsuarioId == Guid.Empty)
                    return StatusCode(400);

                var visit = new Visitas
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    ClienteId = visita.ClienteId,
                    VendedorId = visita.UsuarioId,
                    Fecha = visita.Fecha,
                    Cliente = _context.Clientes.FirstOrDefault(x => x.Id.Equals(visita.ClienteId)) ?? null,
                    Vendedor = _context.Usuarios.FirstOrDefault(x => x.Id.Equals(visita.UsuarioId)) ?? null,
                };

                var visitaInsertada = _context.Visitas.Add(visit);
                _context.SaveChanges();
                _logger.LogInformation($"Visita [{JsonSerializer.Serialize(visita)}] añadido correctamente");

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

                VisitaDto visitaDto = new VisitaDto()
                {
                    Activo = visitaInsertada.Entity.Activo,
                    ClienteId = visitaInsertada.Entity.ClienteId,
                    Fecha = visitaInsertada.Entity.Fecha,
                    Id = visitaInsertada.Entity.Id,
                    UsuarioId = visitaInsertada.Entity.VendedorId
                };

                return Ok(visitaDto);
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
        public IActionResult Update(VisitaDto visita)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(visita.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDVentas)
                    return StatusCode(401);

                var visit = _context.Visitas.FirstOrDefault(x => x.Id == visita.Id);

                if (visit == null)
                    throw new Exception($"La visita no existe");

                var changes = false;
                if (visit.ClienteId != visita.ClienteId)
                {
                    visit.ClienteId = visita.ClienteId;
                    changes = true;
                }
                if (visit.VendedorId != visita.UsuarioId)
                {
                    visit.VendedorId = visita.UsuarioId;
                    changes = true;
                }
                if (visit.Fecha != visita.Fecha)
                {
                    visit.Fecha = visita.Fecha;
                    changes = true;
                }

                if (changes)
                {
                    _context.Visitas.Update(visit);
                    _context.SaveChanges();
                    _logger.LogInformation($"Visita [{JsonSerializer.Serialize(visita)}] actualizado correctamente");

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
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(username));

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDVisitas)
                    return StatusCode(401);

                var visit = _context.Visitas.FirstOrDefault(x => x.Id == Id);

                if (visit == null)
                    throw new Exception($"La visita no existe");

                visit.Activo = false;
                _context.Visitas.Update(visit);
                _context.SaveChanges();
                _logger.LogInformation($"Visita [{JsonSerializer.Serialize(visit)}] eliminado correctamente");

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
