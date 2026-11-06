namespace DiscountEngine.Models;

public class Product
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public decimal BasePrice { get; init; }
    public string Category { get; init; } = "";

    public Product(string id, string name, decimal basePrice, string category)
    {
        Id = id; Name = name; BasePrice = basePrice; Category = category;
    }
}