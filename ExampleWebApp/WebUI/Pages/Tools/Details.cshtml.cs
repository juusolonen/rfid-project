using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Tools;

public class DetailsModel(IToolRepository toolRepository) : PageModel
{
    public ToolDbEntity? Tool { get; set; }
    public List<ProcessedEventDbEntity>? TargetedEvents { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        (Tool, TargetedEvents) = await toolRepository.GetToolAsync(id);

        if (Tool == null)
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