namespace Listjj.Infrastructure.Events
{
    public class AddItemEvent : BaseEvent
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Value { get; set; }
        public Guid CategoryId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Guid UserId { get; set; } = Guid.Empty;
    }
}
