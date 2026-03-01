namespace SecondApi.Domin.Notifecation
{
    public class Notifecation
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int? UserId { get; set; }
        public required string Stutes { get; set; }
        public string? RefuseReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow.AddHours(3);
    }
}
