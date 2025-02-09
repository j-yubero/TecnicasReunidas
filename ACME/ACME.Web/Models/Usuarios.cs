using ACME.Common.Dtos;

namespace ACME.Web.Models
{
    public class Usuarios
    {
        public Guid Id { get; set; }
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public Roles Rol { get; set; }
    }
}
