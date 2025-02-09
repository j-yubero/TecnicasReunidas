using ACME.Common.Dtos;
using ACME.RestService.Repositories;
using ACME.RestService.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace ACME.RestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VentasController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<VentasController> _logger;

        public VentasController(ILogger<VentasController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetVentas")]
        public IEnumerable<VentaDto> GetVentas(bool activos = false)
        {
            try
            {
                List<Ventas> ventas = [];
                if (activos)
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).Include(x => x.Visita.Cliente)];
                else
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).Include(x => x.Visita.Cliente).Where(x => x.Activo)];

                return ventas.Select(x => new VentaDto
                {
                    Id = x.Id,
                    PrecioTotal = x.PrecioTotal,
                    PrecioUnitario = x.PrecioUnitario,
                    Unidades = x.Unidades,
                    Producto = new ProductoDto
                    {
                        Activo = x.Producto.Activo,
                        Descripcion = x.Producto.Descripcion,
                        Id = x.Producto.Id,
                        Nombre = x.Producto.Nombre,
                        Precio = x.Producto.Precio,
                        Stock = x.Producto.Stock
                    },
                    Visita = new VisitaDto
                    {
                        Id = x.Visita.Id,
                        ClienteId = x.Visita.ClienteId,
                        UsuarioId = x.Visita.VendedorId,
                    }
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

                _logger.LogError($"GetVentas Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVentasByUserName")]
        public IEnumerable<VentaDto> GetVentasByUserName(string nombre, bool activos = false)
        {
            try
            {
                var vendedor = _context.Usuarios.FirstOrDefault(x => x.UserName == nombre);
                if (vendedor == null)
                    throw new Exception($"No existe el usuario");


                List<Ventas> ventas = [];
                if (!activos)
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita)];
                else
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).Where(x => x.Visita.VendedorId.Equals(vendedor.Id) && x.Activo)];


                return ventas.Select(x => new VentaDto
                {
                    Id = x.Id,
                    PrecioTotal = x.PrecioTotal,
                    PrecioUnitario = x.PrecioUnitario,
                    Unidades = x.Unidades,
                    Producto = new ProductoDto
                    {
                        Activo = x.Producto.Activo,
                        Descripcion = x.Producto.Descripcion,
                        Id = x.Producto.Id,
                        Nombre = x.Producto.Nombre,
                        Precio = x.Producto.Precio,
                        Stock = x.Producto.Stock
                    },
                    Visita = new VisitaDto
                    {
                        Id = x.Visita.Id,
                        ClienteId = x.Visita.ClienteId,
                        UsuarioId = x.Visita.VendedorId,
                    }
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

                _logger.LogError($"GetVentasByUserName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVentasByProductName")]
        public IEnumerable<VentaDto> GetVentasByProductName(string nombre, bool activos = false)
        {
            try
            {
                var producto = _context.Productos.FirstOrDefault(x => x.Nombre == nombre);
                if (producto == null)
                    throw new Exception($"No existe el producto");


                List<Ventas> ventas = [];
                if (!activos)
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita)];
                else
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).Where(x => x.ProductoId.Equals(producto.Id) && x.Activo)];


                return ventas.Select(x => new VentaDto
                {
                    Id = x.Id,
                    PrecioTotal = x.PrecioTotal,
                    PrecioUnitario = x.PrecioUnitario,
                    Unidades = x.Unidades,
                    Producto = new ProductoDto
                    {
                        Activo = x.Producto.Activo,
                        Descripcion = x.Producto.Descripcion,
                        Id = x.Producto.Id,
                        Nombre = x.Producto.Nombre,
                        Precio = x.Producto.Precio,
                        Stock = x.Producto.Stock
                    },
                    Visita = new VisitaDto
                    {
                        Id = x.Visita.Id,
                        ClienteId = x.Visita.ClienteId,
                        UsuarioId = x.Visita.VendedorId,
                    }
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

                _logger.LogError($"GetVentasByProductName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVentasByUserAndProductName")]
        public IEnumerable<VentaDto> GetVentasByUserAndProductName(string username, string productname, bool activos = false)
        {
            try
            {
                var vendedor = _context.Usuarios.FirstOrDefault(x => x.UserName == username);
                if (vendedor == null)
                    throw new Exception($"No existe el usuario");

                var producto = _context.Productos.FirstOrDefault(x => x.Nombre == productname);
                if (producto == null)
                    throw new Exception($"No existe el producto");


                List<Ventas> ventas = [];
                if (!activos)
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita)];
                else
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).Where(x => x.ProductoId.Equals(producto.Id) && x.Visita.VendedorId.Equals(vendedor.Id) && x.Activo)];


                return ventas.Select(x => new VentaDto
                {
                    Id = x.Id,
                    PrecioTotal = x.PrecioTotal,
                    PrecioUnitario = x.PrecioUnitario,
                    Unidades = x.Unidades,
                    Producto = new ProductoDto
                    {
                        Activo = x.Producto.Activo,
                        Descripcion = x.Producto.Descripcion,
                        Id = x.Producto.Id,
                        Nombre = x.Producto.Nombre,
                        Precio = x.Producto.Precio,
                        Stock = x.Producto.Stock
                    },
                    Visita = new VisitaDto
                    {
                        Id = x.Visita.Id,
                        ClienteId = x.Visita.ClienteId,
                        UsuarioId = x.Visita.VendedorId,
                    }
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

                _logger.LogError($"GetVentasByUserAndProductName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVentasByVisitId")]
        public IEnumerable<VentaDto> GetVentasByVisitId(Guid id, bool activos = false)
        {
            try
            {
                var visita = _context.Visitas.FirstOrDefault(x => x.Id == id);
                if (visita == null)
                    throw new Exception($"No existe el usuario");

                List<Ventas> ventas = [];
                if (activos)
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).Where(x => x.Visita.Id.Equals(id))];
                else
                    ventas = [.. _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).Where(x => x.Visita.Id.Equals(id) && x.Activo)];


                return ventas.Select(x => new VentaDto
                {
                    Id = x.Id,
                    PrecioTotal = x.PrecioTotal,
                    PrecioUnitario = x.PrecioUnitario,
                    Unidades = x.Unidades,
                    Producto = new ProductoDto
                    {
                        Activo = x.Producto.Activo,
                        Descripcion = x.Producto.Descripcion,
                        Id = x.Producto.Id,
                        Nombre = x.Producto.Nombre,
                        Precio = x.Producto.Precio,
                        Stock = x.Producto.Stock
                    },
                    Visita = new VisitaDto
                    {
                        Id = x.Visita.Id,
                        ClienteId = x.Visita.ClienteId,
                        UsuarioId = x.Visita.VendedorId,
                    }
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

                _logger.LogError($"GetVentasByUserAndProductName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetVentaById")]
        public VentaDto GetVentaById(Guid id, bool activos = false)
        {
            try
            {
                Ventas venta = null;
                if (!activos)
                    venta = _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).FirstOrDefault(x => x.Id.Equals(id));
                else
                    venta = _context.Ventas.Include(x => x.Producto).Include(x => x.Visita).FirstOrDefault(x => x.Id.Equals(id) && x.Activo);


                if (venta == null)
                {
                    _logger.LogWarning($"No existe ningún venta con el ID [{id}]");
                    return null;
                }

                return new VentaDto
                {
                    Id = venta.Id,
                    PrecioTotal = venta.PrecioTotal,
                    PrecioUnitario = venta.PrecioUnitario,
                    Unidades = venta.Unidades,
                    Producto = new ProductoDto
                    {
                        Activo = venta.Producto.Activo,
                        Descripcion = venta.Producto.Descripcion,
                        Id = venta.Producto.Id,
                        Nombre = venta.Producto.Nombre,
                        Precio = venta.Producto.Precio,
                        Stock = venta.Producto.Stock
                    },
                    Visita = new VisitaDto
                    {
                        Id = venta.Visita.Id,
                        ClienteId = venta.Visita.ClienteId,
                        UsuarioId = venta.Visita.VendedorId,
                    }
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

                _logger.LogError($"GetVentasById Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(VentaDto ventaDto)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(ventaDto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDVentas)
                    return StatusCode(401);

                var visita = _context.Visitas.FirstOrDefault(x => x.Id == ventaDto.VisitaId);
                if (visita == null)
                    throw new Exception($"No existe el usuario");

                var producto = _context.Productos.FirstOrDefault(x => x.Id == ventaDto.ProductoId);
                if (producto == null)
                    throw new Exception($"No existe el producto");

                if (producto.Stock < ventaDto.Unidades)
                    return StatusCode(StatusCodes.Status409Conflict);

                var venta = new Ventas
                {
                    Id = Guid.NewGuid(),
                    Producto = producto,
                    Visita = visita,
                    PrecioTotal = ventaDto.PrecioTotal,
                    PrecioUnitario = ventaDto.PrecioUnitario,
                    Unidades = ventaDto.Unidades,
                    Activo = true
                };

                var ventaInsertado = _context.Ventas.Add(venta);
                _context.SaveChanges();
                _logger.LogInformation($"Venta [{JsonSerializer.Serialize(venta)}] añadido correctamente");

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

                producto.Stock -= venta.Unidades;
                _context.Productos.Update(producto);
                _context.SaveChanges();

                historial = new Historial()
                {
                    Accion = "Update",
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

                _logger.LogError($"AddVenta Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult Update(VentaDto ventaDto)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(ventaDto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDVentas)
                    return StatusCode(401);

                var venta = _context.Ventas.FirstOrDefault(x => x.Id == ventaDto.Id);
                if (venta == null)
                    throw new Exception($"La venta no existe");

                var changes = false;
                if (venta.PrecioTotal != ventaDto.PrecioTotal)
                {
                    venta.PrecioTotal = ventaDto.PrecioTotal;
                    changes = true;
                }
                if (venta.PrecioUnitario != ventaDto.PrecioUnitario)
                {
                    venta.PrecioUnitario = ventaDto.PrecioUnitario;
                    changes = true;
                }
                if (venta.Unidades != ventaDto.Unidades)
                {
                    venta.Unidades = ventaDto.Unidades;
                    changes = true;
                }
                if (venta.ProductoId != ventaDto.ProductoId)
                {
                    venta.ProductoId = ventaDto.ProductoId;
                    changes = true;
                }

                if (changes)
                {
                    _context.Ventas.Update(venta);
                    _context.SaveChanges();
                    _logger.LogInformation($"Venta [{JsonSerializer.Serialize(venta)}] actualizado correctamente");

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

                if (!usuario.Rol.CanCUDVentas)
                    return StatusCode(401);

                var venta = _context.Ventas.FirstOrDefault(x => x.Id == Id);

                if (venta == null)
                    throw new Exception($"La venta no existe");

                venta.Activo = false;
                _context.Ventas.Update(venta);
                _context.SaveChanges();
                _logger.LogInformation($"Venta [{JsonSerializer.Serialize(venta)}] eliminado correctamente");

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
