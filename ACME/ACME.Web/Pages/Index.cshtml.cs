using ACME.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Security.Permissions;
using System.Web;

namespace ACME.Web.Pages
{
    public class IndexModel : PageModel
    {

        public void OnGet()
        {

        }

        [BindProperty]
        public Credentials Credentials { get; set; } = default!;


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await checkCredentials(Credentials);
                if (result)
                {
                    HttpContext.Session.SetString("UserName", Credentials.Username);
                    return RedirectToPage("/Visita/Index");
                }
                ModelState.AddModelError(string.Empty, "Usuario y/o contraseña incorrectos");
            }
            return Page();
        }

        private async Task<bool> checkCredentials(Credentials login)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var requestBody = JsonConvert.SerializeObject(login);
                    var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://localhost:7039/Usuarios/CheckCredentials", httpContent);

                    return Convert.ToBoolean(await response.Content.ReadAsStringAsync());

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
