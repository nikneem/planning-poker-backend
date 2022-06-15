namespace PlanningPoker.Domain.DataTransferObjects;

public class SessionDetailsDto
{
    public string Code { get; set; }
    public string ConnectionUrl { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string OwningCode { get; set; }
}