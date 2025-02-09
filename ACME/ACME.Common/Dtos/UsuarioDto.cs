using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACME.Common.Dtos
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public RolDto? Rol { get; set; }
        public Guid RolId { get; set; }
        public bool Activo { get; set; }
        public string CreatedBy { get; set; }
    }
}
