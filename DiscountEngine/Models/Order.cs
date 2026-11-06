namespace DiscountEngine.Models;

public class Order
{
    public string Id { get; init; } = "";
    public Customer Customer { get; init; } = new();
    public List<CartItem> Items { get; init; } = new();
    public string? PromoCode { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal DeliveryCost { get; init; }
}