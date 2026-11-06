namespace DiscountEngine.Models;

public class Customer
{
    public string Id { get; init; } = "";
    public DateTime? LastPurchaseDate { get; init; }
    public bool IsFirstOrder { get; init; }
}