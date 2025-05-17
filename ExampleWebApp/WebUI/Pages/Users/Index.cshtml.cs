using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Users;

public class UsersModel(IUserRepository userRepository) : PageModel
{
      
    public List<UserDbEntity> SystemUsers { get; set; } = new List<UserDbEntity>();
    
    public async Task<IActionResult> OnGet()
    {
        SystemUsers = await userRepository.GetAllUsersAsync();
        return Page();
    }
}