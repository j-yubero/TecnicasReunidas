using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACME.Common.Dtos
{
    public class HistorialDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public string Accion { get; set; }

        public DateTime Fecha { get; set; }
    }
}
