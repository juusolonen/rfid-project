using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.History;

public class HistoryModel(ILogger<HistoryModel> logger, IEventRepository eventRepository) : PageModel
{
    private readonly ILogger<HistoryModel> _logger = logger;
    
    public List<EventBaseDbEntity> Events { get; set; } = new List<EventBaseDbEntity>();
    public List<ProcessedEventDbEntity> ProcessedEvents => Events.OfType<ProcessedEventDbEntity>()
        .Where(e => !e.Faulted)
        .OrderByDescending(x => x.ReceivedAt)
        .ToList();
    public List<EventBaseDbEntity> FaultedEvents => Events
        .Where(e => e.Faulted && !e.Processed)
        .OrderByDescending(x => x.ReceivedAt)
        .ToList();
    

    public async Task<IActionResult> OnGet()
    {
        Events = await eventRepository.GetAllEvents();

        return Page();
    }
}