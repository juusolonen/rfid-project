using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages;

public class IndexModel() : PageModel
{
    
    public async Task<IActionResult> OnGet()
    {
        return Page();
    }
}