using InboxEngine.Models;

namespace InboxEngine.Services;

/// <summary>
/// TODO: Implement the priority scoring logic according to the requirements:
/// - VIP Status: +50 points if IsVIP is true
/// - Urgency Keywords: +30 points if Subject contains "Urgent", "ASAP", or "Error" (case-insensitive)
/// - Time Decay: +1 point for every hour passed since ReceivedAt
/// - Spam Filter: -20 points if Body contains "Unsubscribe" or "Newsletter"
/// - Clamping: Final score must be between 0 and 100 (inclusive)
/// </summary>
public class PriorityScoringService : IPriorityScoringService
{
    public int CalculatePriorityScore(Email email)
    {
        int score = 0;
        
        // VIP Status: +50 points if IsVIP is true
        if (email.IsVIP)
            score += 50;
        
        // Urgency Keywords: +30 points if Subject contains "Urgent", "ASAP", or "Error" (case-insensitive)
        if (email.Subject.Contains("Urgent", StringComparison.OrdinalIgnoreCase) ||
            email.Subject.Contains("ASAP", StringComparison.OrdinalIgnoreCase) ||
            email.Subject.Contains("Error", StringComparison.OrdinalIgnoreCase))
            score += 30;
        
        // Time Decay: +1 point for every hour passed since ReceivedAt
        var hoursPassed = (int)(DateTime.UtcNow - email.ReceivedAt).TotalHours;
        score += hoursPassed;
        
        // Spam Filter: -20 points if Body contains "Unsubscribe" or "Newsletter"
        if (email.Body.Contains("Unsubscribe", StringComparison.OrdinalIgnoreCase) ||
            email.Body.Contains("Newsletter", StringComparison.OrdinalIgnoreCase))
            score -= 20;
        
        // Clamping: Final score must be between 0 and 100 (inclusive)
        return Math.Clamp(score, 0, 100);
    }
}
