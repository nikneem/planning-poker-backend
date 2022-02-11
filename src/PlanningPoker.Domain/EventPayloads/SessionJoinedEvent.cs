using System.Text.Json.Serialization;

namespace PlanningPoker.Domain.EventPayloads;

public class SessionJoinedEvent
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}