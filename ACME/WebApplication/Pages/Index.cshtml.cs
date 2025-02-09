using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using WebApplication.Models;

namespace WebApplication.Pages
{
   [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                var result = await checkCredentials(credentials);
                if (result)
                {
                    return RedirectToPage("./MainPage");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
                    var response = await client.PostAsync($"https://localhost:433/Usuarios/CheckCredentials", httpContent);

                    return response.IsSuccessStatusCode;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
