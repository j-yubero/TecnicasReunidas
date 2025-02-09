using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACME.Common.Dtos
{
    public class RolDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public bool CanCUDVisitas { get; set; }
        public bool CanCUDVentas { get; set; }
        public bool CanCUDClientes { get; set; }
        public bool CanCUDUsuarios { get; set; }
        public bool Activo { get; set; }
        public string CreatedBy { get; set; }
    }
}
