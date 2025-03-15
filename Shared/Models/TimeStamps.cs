namespace OrderFlow.Shared.Models;

public class TimeStamps
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
 }