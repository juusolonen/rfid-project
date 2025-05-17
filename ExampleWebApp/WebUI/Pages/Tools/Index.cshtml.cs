using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Tools;

public class ToolsModel(ILogger<ToolsModel> logger, IToolRepository toolRepository) : PageModel
{
    private readonly ILogger<ToolsModel> _logger = logger;
    private readonly IToolRepository _toolRepository = toolRepository;
    
    private List<ToolDbEntity> Tools { get; set; } = new List<ToolDbEntity>();
    public List<ToolDbEntity> ActiveTools => Tools.Where(t => !t.Deleted).ToList();
    public List<ToolDbEntity> DeletedTools => Tools.Where(t => t.Deleted).ToList();
    
    public async Task<IActionResult> OnGet()
    {
        Tools = await toolRepository.GetAllToolsAsync();
        return Page();
    }
}