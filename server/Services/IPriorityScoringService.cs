using InboxEngine.Models;

namespace InboxEngine.Services;

public interface IPriorityScoringService
{
    int CalculatePriorityScore(Email email);
}
