namespace PlanningPoker.Domain.DataTransferObjects;

public class VoteSessionDto
{
    public string ConnectionId { get; set; }
    public string DisplayName { get; set; }
    public int Vote { get; set; }
}