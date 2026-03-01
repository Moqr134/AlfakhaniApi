namespace SecondApi.Domin.User
{
    public class UserConnection
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public required string ConnectionId { get; set; }
        public string? DeviceToken { get; set; }
        public string? role { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
