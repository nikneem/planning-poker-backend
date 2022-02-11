namespace PlanningPoker.Api.Functions.Sessions
{
    public  class WebPubSubMessageDto
    {
        public string Command { get; set; }
        public object Payload { get; set; }
    }
}
