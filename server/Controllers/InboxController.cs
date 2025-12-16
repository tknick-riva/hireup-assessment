using Microsoft.AspNetCore.Mvc;
using InboxEngine.Models;
using InboxEngine.Services;

namespace InboxEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InboxController : ControllerBase
{
    private readonly IPriorityScoringService _scoringService;
    private readonly ILogger<InboxController> _logger;

    public InboxController(IPriorityScoringService scoringService, ILogger<InboxController> logger)
    {
        _scoringService = scoringService;
        _logger = logger;
    }

    [HttpPost("sort")]
    public IActionResult SortEmails([FromBody] List<Email> emails)
    {
        if (emails == null || emails.Count == 0)
            return BadRequest("Email list cannot be null or empty");
        
        foreach (var email in emails)
        {
            email.PriorityScore = _scoringService.CalculatePriorityScore(email);
        }
        
        var sortedEmails = emails.OrderByDescending(e => e.PriorityScore).ToList();
        
        return Ok(sortedEmails);
    }
}
