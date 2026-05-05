using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_Identity.Pages
{
    [Authorize(policy: "Must Belong to HR Department")]
    public class HumanResourceModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
