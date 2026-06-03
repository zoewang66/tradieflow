namespace TradieFlow.Domain.Entities;

public enum JobStatus
{
    Quoted,       
    Scheduled,    
    InProgress,   
    Completed,    
    Cancelled    
}

public class Job
{
    public int Id { get; set; }


    public int ClientId { get; set; }          
    public Client Client { get; set; } = null!; 
    public List<Invoice> Invoices { get; set; } = new();

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Quoted;
    public DateTime? ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}