using Xunit;
using InboxEngine.Models;
using InboxEngine.Services;

namespace InboxEngine.Tests.Services;

public class PriorityScoringServiceTests
{
    private readonly IPriorityScoringService _service;

    public PriorityScoringServiceTests()
    {
        _service = new PriorityScoringService();
    }

    [Fact]
    public void TestSetup_ShouldPass()
    {
        // This is a dummy test to verify the test infrastructure is working.
        // If this test fails after you add your own tests, you know it's something
        // in your code that broke the build.
        Assert.True(true);
    }

    [Fact]
    public void CalculatePriorityScore_VIPEmail_Adds50Points()
    {
        var email = new Email
        {
            IsVIP = true,
            Subject = "Regular email",
            Body = "Regular content",
            ReceivedAt = DateTime.UtcNow
        };

        var score = _service.CalculatePriorityScore(email);
        
        Assert.Equal(50, score);
    }

    [Theory]
    [InlineData("URGENT: Fix this")]
    [InlineData("Please handle ASAP")]
    [InlineData("Error in production")]
    [InlineData("urgent issue")]
    [InlineData("asap please")]
    [InlineData("error occurred")]
    public void CalculatePriorityScore_UrgencyKeywords_Adds30Points(string subject)
    {
        var email = new Email
        {
            IsVIP = false,
            Subject = subject,
            Body = "Regular content",
            ReceivedAt = DateTime.UtcNow
        };

        var score = _service.CalculatePriorityScore(email);
        
        Assert.Equal(30, score);
    }

    [Fact]
    public void CalculatePriorityScore_TimeDecay_AddsHoursPassed()
    {
        var email = new Email
        {
            IsVIP = false,
            Subject = "Regular email",
            Body = "Regular content",
            ReceivedAt = DateTime.UtcNow.AddHours(-5)
        };

        var score = _service.CalculatePriorityScore(email);
        
        Assert.Equal(5, score);
    }

    [Theory]
    [InlineData("Click to unsubscribe")]
    [InlineData("Weekly Newsletter")]
    [InlineData("UNSUBSCRIBE here")]
    [InlineData("newsletter content")]
    public void CalculatePriorityScore_SpamFilter_Subtracts20Points(string body)
    {
        var email = new Email
        {
            IsVIP = false,
            Subject = "Regular email",
            Body = body,
            ReceivedAt = DateTime.UtcNow.AddHours(-30)
        };

        var score = _service.CalculatePriorityScore(email);
        
        Assert.Equal(10, score); // 30 hours - 20 spam = 10
    }

    [Fact]
    public void CalculatePriorityScore_ScoreClamping_MinimumZero()
    {
        var email = new Email
        {
            IsVIP = false,
            Subject = "Regular email",
            Body = "Unsubscribe Newsletter",
            ReceivedAt = DateTime.UtcNow
        };

        var score = _service.CalculatePriorityScore(email);
        
        Assert.Equal(0, score); // -20 clamped to 0
    }

    [Fact]
    public void CalculatePriorityScore_ScoreClamping_Maximum100()
    {
        var email = new Email
        {
            IsVIP = true,
            Subject = "URGENT Error",
            Body = "Regular content",
            ReceivedAt = DateTime.UtcNow.AddHours(-50)
        };

        var score = _service.CalculatePriorityScore(email);
        
        Assert.Equal(100, score); // 50 + 30 + 50 = 130, clamped to 100
    }

    [Fact]
    public void CalculatePriorityScore_CombinedRules_CalculatesCorrectly()
    {
        var email = new Email
        {
            IsVIP = true,
            Subject = "URGENT: Newsletter Error",
            Body = "Please unsubscribe if needed",
            ReceivedAt = DateTime.UtcNow.AddHours(-10)
        };

        var score = _service.CalculatePriorityScore(email);
        
        Assert.Equal(70, score); // 50 (VIP) + 30 (Urgent) + 10 (time) - 20 (spam) = 70
    }
}

