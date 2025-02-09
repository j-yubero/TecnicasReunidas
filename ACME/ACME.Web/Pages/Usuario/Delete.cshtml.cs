using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACME.Web.Pages.Usuario
{
    public class DeleteModel : PageModel
    {
        public async Task<IActionResult> OnGet(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {

                    var response = await client.DeleteAsync($"https://localhost:7039/Usuarios/Delete?id={Id}&username={HttpContext.Session.GetString("UserName")}");
                    return RedirectToPage("/Usuario/Index");

                }
                catch (Exception ex)
                {
                    return RedirectToPage("/Usuario/Index");
                }
            }
        }
    }
}
