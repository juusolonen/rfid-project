using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.History;

public class DetailsModel(IEventRepository eventRepository) : PageModel
{
    public EventBaseDbEntity? HistoryEvent { get; set; }
    public UserDbEntity? Caller { get; set; }
    public UserDbEntity? TargetUser { get; set; }
    public List<ToolDbEntity> Tools { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        (HistoryEvent, Caller, TargetUser, Tools) = await eventRepository.GetEventAsync(id);

        if (HistoryEvent == null)
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