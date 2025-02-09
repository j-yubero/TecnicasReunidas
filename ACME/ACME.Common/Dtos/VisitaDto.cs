using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACME.Common.Dtos
{
    public class VisitaDto
    {
        public Guid? Id { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public Guid UsuarioId { get; set; }
        public ClientesDto? Cliente { get; set; }
        public Guid ClienteId { get; set; }
        public bool? Activo { get; set; }
        public DateTime Fecha { get; set; }
        public string CreatedBy { get; set; }
    }
}
