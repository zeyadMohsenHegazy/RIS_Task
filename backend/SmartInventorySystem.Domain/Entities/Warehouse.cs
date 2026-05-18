namespace SmartInventorySystem.Domain.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();

    protected Warehouse() { }

    public Warehouse(string name, string location)
    {
        Name = name;
        Location = location;
    }
}
