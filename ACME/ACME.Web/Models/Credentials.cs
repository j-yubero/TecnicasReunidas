using System.ComponentModel.DataAnnotations;

namespace ACME.Web.Models
{
    public class Credentials
    {
        [Required]
        [Display(Name = "Nombre de usuario"), DataType(DataType.Text)]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Contraseña"), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
