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
    public class HistorialController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<HistorialController> _logger;

        public HistorialController(ILogger<HistorialController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("GetHistorial")]
        public IEnumerable<HistorialDto> GetHistorial()
        {
            try
            {
                List<Historial> historial = [];
                historial = [.. _context.Historial.Include(x => x.Usuario)];

                return historial.Select(x => new HistorialDto
                {
                    Id = x.Id,
                    Accion = x.Accion,
                    Fecha = x.Fecha,
                    UserName = x.Usuario.UserName,
                    UserId = x.Usuario.Id
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

                _logger.LogError($"GetHistorial Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpGet]
        [Route("GetHistorialByUser")]
        public IEnumerable<HistorialDto> GetHistorialByUser(string nombre)
        {
            try
            {
                var historial = _context.Historial.Include(x => x.Usuario).Where(x => x.Usuario.UserName.Contains(nombre)).ToList();

                return historial.Select(x => new HistorialDto
                {
                    Id = x.Id,
                    UserName = x.Usuario.UserName,
                    UserId = x.Usuario.Id,
                    Accion = x.Accion,
                    Fecha = x.Fecha
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

                _logger.LogError($"GetHistorialByUser Error:[{errorMessage}]");

                throw;
            }
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(HistorialDto historial)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.Id == historial.UserId);
                var historialToInsert = new Historial
                {
                    Fecha = historial.Fecha,
                    Accion = historial.Accion,
                    Id = Guid.NewGuid(),
                    Usuario = usuario,
                };

                var historialInsertado = _context.Historial.Add(historialToInsert);
                _context.SaveChanges();
                _logger.LogInformation($"Historial [{JsonSerializer.Serialize(historial)}] añadido correctamente");

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
    }
}
