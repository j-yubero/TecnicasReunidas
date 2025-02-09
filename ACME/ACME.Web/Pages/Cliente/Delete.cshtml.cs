using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACME.Web.Pages.Cliente
{
    public class DeleteModel : PageModel
    {
        public async Task<IActionResult> OnGet(Guid Id)
        {
            using (var client = new HttpClient())
            {
                try
                {

                    var response = await client.DeleteAsync($"https://localhost:7039/Clientes/Delete?id={Id}&username={HttpContext.Session.GetString("UserName")}");
                    return RedirectToPage("/Cliente/Index");

                }
                catch (Exception ex)
                {
                    return RedirectToPage("/Cliente/Index");
                }
            }
        }
    }
}
