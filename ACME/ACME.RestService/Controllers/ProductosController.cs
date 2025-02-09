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
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(ILogger<ProductosController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetProductos")]
        public IEnumerable<ProductoDto> GetProductos(bool inactivos = false)
        {
            try
            {
                List<Productos> productos = [];
                if (inactivos)
                    productos = [.. _context.Productos];
                else
                    productos = [.. _context.Productos.Where(x => x.Activo)];

                return productos.Select(x => new ProductoDto
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    Precio = x.Precio,
                    Stock = x.Stock,
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

                _logger.LogError($"GetProducts Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetProductsByName")]
        public IEnumerable<ProductoDto> GetProductsByName(string nombre, bool inactivos = false)
        {
            try
            {
                List<Productos> productos = [];
                if (inactivos)
                    productos = [.. _context.Productos.Where(x => x.Nombre.Contains(nombre))];
                else
                    productos = [.. _context.Productos.Where(x => x.Nombre.Contains(nombre) && x.Activo)];

                return productos.Select(x => new ProductoDto
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion,
                    Precio = x.Precio,
                    Stock = x.Stock,
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

                _logger.LogError($"GetProductsByName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetProductoById")]
        public ProductoDto GetProductoById(Guid id, bool inactivos = false)
        {
            try
            {
                Productos producto = null;
                if (inactivos)
                    producto = _context.Productos.FirstOrDefault(x => x.Id.Equals(id));
                else
                    producto = _context.Productos.FirstOrDefault(x => x.Id.Equals(id) && x.Activo);

                if (producto == null)
                {
                    _logger.LogWarning($"No existe ningún producto con el ID [{id}]");
                    return null;
                }

                return new ProductoDto
                {
                    Id = producto.Id,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    Nombre = producto.Nombre,
                    Activo = producto.Activo
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

                _logger.LogError($"GetProductById Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(ProductoDto producto)
        {
            _logger.LogTrace("Adding product...");
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName.Equals(producto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                var product = new Productos
                {
                    Id = Guid.NewGuid(),
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    Nombre = producto.Nombre,
                    Activo = true
                };

                var productoInsertado = _context.Productos.Add(product);
                _context.SaveChanges();
                _logger.LogInformation($"Producto [{JsonSerializer.Serialize(producto)}] añadido correctamente");

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
        public IActionResult Update(ProductoDto producto)
        {
            _logger.LogTrace("Updating product...");
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName.Equals(producto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                var product = _context.Productos.FirstOrDefault(x => x.Id == producto.Id);

                if (product == null)
                    throw new Exception($"El producto {product.Nombre} no existe");

                var changes = false;
                if (product.Nombre != producto.Nombre)
                {
                    product.Nombre = producto.Nombre;
                    changes = true;
                }
                if (product.Descripcion != producto.Descripcion)
                {
                    product.Descripcion = producto.Descripcion;
                    changes = true;
                }
                if (product.Precio != producto.Precio)
                {
                    product.Precio = producto.Precio;
                    changes = true;
                }
                if (product.Stock != producto.Stock)
                {
                    product.Stock = producto.Stock;
                    changes = true;
                }

                if (changes)
                {
                    _context.Productos.Update(product);
                    _context.SaveChanges();
                    _logger.LogInformation($"Producto [{JsonSerializer.Serialize(producto)}] actualizado correctamente");

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

                var producto = _context.Productos.FirstOrDefault(x => x.Id == Id);

                if (producto == null)
                    throw new Exception($"El producto {producto.Nombre} no existe");

                producto.Activo = false;
                _context.Productos.Update(producto);
                _context.SaveChanges();
                _logger.LogInformation($"Producto [{JsonSerializer.Serialize(producto)}] eliminado correctamente");

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
