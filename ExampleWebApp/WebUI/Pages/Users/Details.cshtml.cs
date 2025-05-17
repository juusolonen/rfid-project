using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Users;

public class DetailsModel(IUserRepository userRepository) : PageModel
{
    public UserDbEntity? User { get; set; }
    public List<ProcessedEventDbEntity>? CalledEvents { get; set; }
    public List<ProcessedEventDbEntity>? TargetedEvents { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        (User, CalledEvents, TargetedEvents) = await userRepository.GetUserAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        return Page();
    }

    public string? GetToolNames(List<ToolDbEntity> tools)
    {
        var names = tools.Select(e => e.Name).ToArray();
        return names.Length > 0 ? string.Join(",", names) : null;
    }
}