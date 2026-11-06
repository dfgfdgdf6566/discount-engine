namespace DiscountEngine.Models;

public class AppliedDiscount
{
    public string Name { get; init; } = "";
    public decimal Amount { get; init; }
    public string Reason { get; init; } = "";
}