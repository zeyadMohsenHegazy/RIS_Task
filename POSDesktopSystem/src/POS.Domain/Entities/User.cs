using POS.Domain.Enums;

namespace POS.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
