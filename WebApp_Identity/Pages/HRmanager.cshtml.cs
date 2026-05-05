using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_Identity.Pages
{
    [Authorize(policy: "HRManagerOnly")]
    public class HRmanagerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
