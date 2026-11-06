namespace DiscountEngine.Models;

public class PricingResult
{
    public decimal BaseTotal { get; init; }
    public List<AppliedDiscount> AppliedDiscounts { get; init; } = new();
    public decimal FinalTotal { get; init; }
}