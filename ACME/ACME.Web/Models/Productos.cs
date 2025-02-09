namespace ACME.Web.Models
{
    public class Productos
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double PVP { get; set; }
        public int Stock { get; set; }
    }
}
