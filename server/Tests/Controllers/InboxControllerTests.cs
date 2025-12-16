using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InboxEngine.Controllers;
using InboxEngine.Models;
using InboxEngine.Services;

namespace InboxEngine.Tests.Controllers;

public class TestPriorityScoringService : IPriorityScoringService
{
    private readonly Dictionary<string, int> _subjectScores = new();
    
    public void SetScore(string subject, int score)
    {
        _subjectScores[subject] = score;
    }
    
    public int CalculatePriorityScore(Email email)
    {
        return _subjectScores.TryGetValue(email.Subject, out var score) ? score : 0;
    }
}

public class TestLogger : ILogger<InboxController>
{
    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
}

public class InboxControllerTests
{
    private readonly TestPriorityScoringService _scoringService;
    private readonly InboxController _controller;

    public InboxControllerTests()
    {
        _scoringService = new TestPriorityScoringService();
        _controller = new InboxController(_scoringService, new TestLogger());
    }

    [Fact]
    public void SortEmails_NullInput_ReturnsBadRequest()
    {
        var result = _controller.SortEmails(null);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email list cannot be null or empty", badRequestResult.Value);
    }

    [Fact]
    public void SortEmails_EmptyList_ReturnsBadRequest()
    {
        var result = _controller.SortEmails(new List<Email>());
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email list cannot be null or empty", badRequestResult.Value);
    }

    [Fact]
    public void SortEmails_ValidEmails_CalculatesScoresAndSorts()
    {
        var emails = new List<Email>
        {
            new Email { Subject = "Low priority" },
            new Email { Subject = "High priority" },
            new Email { Subject = "Medium priority" }
        };

        _scoringService.SetScore("Low priority", 20);
        _scoringService.SetScore("High priority", 80);
        _scoringService.SetScore("Medium priority", 50);

        var result = _controller.SortEmails(emails);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var sortedEmails = Assert.IsType<List<Email>>(okResult.Value);
        
        Assert.Equal(3, sortedEmails.Count);
        Assert.Equal(80, sortedEmails[0].PriorityScore);
        Assert.Equal(50, sortedEmails[1].PriorityScore);
        Assert.Equal(20, sortedEmails[2].PriorityScore);
    }

    [Fact]
    public void SortEmails_SingleEmail_ReturnsCorrectScore()
    {
        var emails = new List<Email> { new Email { Subject = "Test email" } };
        _scoringService.SetScore("Test email", 75);

        var result = _controller.SortEmails(emails);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var sortedEmails = Assert.IsType<List<Email>>(okResult.Value);
        
        Assert.Single(sortedEmails);
        Assert.Equal(75, sortedEmails[0].PriorityScore);
    }
}