using ACME.Common.Dtos;
using ACME.RestService.Repositories;
using ACME.RestService.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;

namespace ACME.RestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(ILogger<UsuariosController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetUsuarios")]
        public IEnumerable<UsuarioDto> GetUsuarios(bool inactivos = false)
        {
            try
            {
                List<Usuarios> usuarios = [];
                if (inactivos)
                    usuarios = [.. _context.Usuarios.Include(x => x.Rol)];
                else
                    usuarios = [.. _context.Usuarios.Include(x => x.Rol).Where(x => x.Activo)];

                return usuarios.Select(x => new UsuarioDto
                {
                    Id = x.Id,
                    Password = x.Password,
                    Rol = new RolDto
                    {
                        CanCUDClientes = x.Rol.CanCUDClientes,
                        CanCUDUsuarios = x.Rol.CanCUDUsuarios,
                        CanCUDVentas = x.Rol.CanCUDVentas,
                        CanCUDVisitas = x.Rol.CanCUDVisitas,
                        Id = x.Rol.Id,
                        Nombre = x.Rol.Nombre
                    },
                    UserName = x.UserName,
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

                _logger.LogError($"GetUsers Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("CheckCredentials")]
        public bool CheckCredentials(Login login)
        {
            var usuario = _context.Usuarios.Include(u=> u.Rol).FirstOrDefault(u => u.UserName == login.userName);
            if (usuario == null)
                return false;

            var passbyte = Encoding.UTF8.GetBytes(login.password);
            var passSha = Convert.ToBase64String(passbyte);

            if (passSha.Equals(usuario.Password))
                return true;

            return false;
        }

        [HttpGet]
        [Route("GetUsersByName")]
        public IEnumerable<UsuarioDto> GetUsersByName(string nombre, bool inactivos = false)
        {
            try
            {
                List<Usuarios> usuarios = [];
                if (inactivos)
                    usuarios = [.. _context.Usuarios.Include(x => x.Rol).Where(x => x.UserName.Contains(nombre))];
                else
                    usuarios = [.. _context.Usuarios.Include(x => x.Rol).Where(x => x.UserName.Contains(nombre) && x.Activo)];

                return usuarios.Select(x => new UsuarioDto
                {
                    Id = x.Id,
                    Password = x.Password,
                    Rol = new RolDto
                    {
                        CanCUDClientes = x.Rol.CanCUDClientes,
                        CanCUDUsuarios = x.Rol.CanCUDUsuarios,
                        CanCUDVentas = x.Rol.CanCUDVentas,
                        CanCUDVisitas = x.Rol.CanCUDVisitas,
                        Id = x.Rol.Id,
                        Nombre = x.Rol.Nombre
                    },
                    UserName = x.UserName,
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

                _logger.LogError($"GetUsersByName Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetUserById")]
        public UsuarioDto GetUserById(Guid id, bool inactivos = false)
        {
            try
            {
                Usuarios usuario = null;
                if (inactivos)
                    usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.Id.Equals(id));
                else
                    usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.Id.Equals(id) && x.Activo);

                if (usuario == null)
                {
                    _logger.LogWarning($"No existe ningún usuario con el ID [{id}]");
                    return null;
                }

                return new UsuarioDto
                {
                    Id = usuario.Id,
                    Password = usuario.Password,
                    Rol = new RolDto
                    {
                        CanCUDClientes = usuario.Rol.CanCUDClientes,
                        CanCUDUsuarios = usuario.Rol.CanCUDUsuarios,
                        CanCUDVentas = usuario.Rol.CanCUDVentas,
                        CanCUDVisitas = usuario.Rol.CanCUDVisitas,
                        Id = usuario.Rol.Id,
                        Nombre = usuario.Rol.Nombre
                    },
                    UserName = usuario.UserName,
                    Nombre = usuario.Nombre,
                    Activo = usuario.Activo
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

                _logger.LogError($"GetUserById Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(UsuarioDto usuarioDto)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(usuarioDto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDUsuarios)
                    return StatusCode(401);

                var rol = _context.Roles.FirstOrDefault(x => x.Id.Equals(usuarioDto.RolId));

                if (rol == null)
                    throw new Exception($"El rol elegido no existe");

                var passbyte = Encoding.UTF8.GetBytes(usuarioDto.Password);
                var passSha = Convert.ToBase64String(passbyte);

                var usuarioInsertado = new Usuarios
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    Password = passSha,
                    Rol = rol,
                    UserName = usuarioDto.UserName,
                    Nombre = usuarioDto.Nombre
                };

                _context.Usuarios.Add(usuarioInsertado);
                _context.SaveChanges();
                _logger.LogInformation($"Usuario [{JsonSerializer.Serialize(usuarioDto)}] añadido correctamente");

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
        public IActionResult Update(UsuarioDto usuarioDto)
        {
            try
            {
                var usuario = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(usuarioDto.CreatedBy));

                if (usuario == null)
                    return StatusCode(401);

                if (!usuario.Rol.CanCUDUsuarios)
                    return StatusCode(401);

                var user = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.Id == usuarioDto.Id);
                var rol = _context.Roles.FirstOrDefault(x => x.Id.Equals(usuarioDto.RolId));

                if (user == null)
                    throw new Exception($"El usuario {user.Nombre} no existe");

                if (rol == null)
                    throw new Exception($"El rol elegido no existe");

                var changes = false;
                if (user.Nombre != usuarioDto.Nombre)
                {
                    user.Nombre = usuarioDto.Nombre;
                    changes = true;
                }
                if (user.Password != usuarioDto.Password && !string.IsNullOrEmpty(usuarioDto.Password))
                {
                    user.Password = usuarioDto.Password;
                    changes = true;
                }
                if (user.UserName != usuarioDto.UserName)
                {
                    user.UserName = usuarioDto.UserName;
                    changes = true;
                }
                if (user.Rol.Id != usuarioDto.RolId)
                {
                    user.Rol = rol;
                    changes = true;
                }

                if (changes)
                {
                    _context.Usuarios.Update(user);
                    _context.SaveChanges();
                    _logger.LogInformation($"Usuario [{JsonSerializer.Serialize(usuarioDto)}] actualizado correctamente");

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
                var userBBDD = _context.Usuarios.Include(x => x.Rol).FirstOrDefault(x => x.UserName.Equals(username));

                if (userBBDD == null)
                    return StatusCode(401);

                if (!userBBDD.Rol.CanCUDUsuarios)
                    return StatusCode(401);

                var usuario = _context.Usuarios.FirstOrDefault(x => x.Id == Id);

                if (usuario == null)
                    throw new Exception($"El usuario {usuario.Nombre} no existe");

                usuario.Activo = false;
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                _logger.LogInformation($"Usuario [{JsonSerializer.Serialize(usuario)}] eliminado correctamente");

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

    public class Login
    {
        public string userName { get; set; }
        public string password { get; set; }
    }
}
