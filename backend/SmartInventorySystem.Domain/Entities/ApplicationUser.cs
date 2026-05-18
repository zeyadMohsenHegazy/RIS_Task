namespace SmartInventorySystem.Domain.Entities;

public class ApplicationUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } =
        new List<InventoryTransaction>();

    protected ApplicationUser() { }

    public ApplicationUser(string username, string passwordHash, string role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }
}
