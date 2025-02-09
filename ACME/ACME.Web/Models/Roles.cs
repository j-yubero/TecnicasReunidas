namespace ACME.Web.Models
{
    public class Roles
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public bool CUDUsuarios { get; set; }
        public bool CUDVisitas { get; set; }
        public bool CUDVentas { get; set; }
        public bool CUDClientes { get; set; }

    }
}
