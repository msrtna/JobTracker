namespace JobTracker.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Location { get; set; }
    }
}
