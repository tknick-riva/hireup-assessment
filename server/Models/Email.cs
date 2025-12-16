namespace InboxEngine.Models;

public class Email
{
    public string Sender { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public bool IsVIP { get; set; }
    public int PriorityScore { get; set; }
}
