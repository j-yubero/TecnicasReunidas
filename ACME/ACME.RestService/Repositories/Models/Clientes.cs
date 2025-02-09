using ACME.Common.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ACME.RestService.Repositories.Models
{
    public class Clientes
    {
        [Key]
        public Guid Id { get; set; }
        [Required, NotNull]
        [MaxLength(200)]
        public string Nombre { get; set; }
        [Required, NotNull]
        [MaxLength(200)]
        public string Direccion { get; set; }
        [Required, NotNull]
        public bool Activo { get; set; }

        public static ClientesDto ClientesToClientesDto(Clientes cliente)
        {
            return new ClientesDto
            {
                Id = cliente.Id,
                Activo = cliente.Activo,
                Direccion = cliente.Direccion,
                Nombre = cliente.Nombre
            };
        }
    }
}
