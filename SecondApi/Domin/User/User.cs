namespace Domin.User;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string HashPassword { get; set; }
    public bool IsConfirm { get; set; } = false;
    public required string Code { get; set; }
    public required string Email { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsOnline { get; set; } = false;
    public bool IsRemoved { get; set; } = false;
    public DateTime? RemoveDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? LastLogout { get; set; }
    public bool IsActive { get; set; }
    public byte[]? Version { get; set; }
    public string? Token { get; set; }
}
