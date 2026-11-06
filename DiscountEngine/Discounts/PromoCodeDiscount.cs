namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public enum PromoKind { Percent, Fixed }

public class PromoCodeDiscount : IPeekableDiscount
{
    public string Name { get; }
    public string Code { get; }
    public PromoKind Kind { get; }
    public decimal Value { get; }
    public DateTime? ExpiresAt { get; }
    public bool Active { get; }

    public PromoCodeDiscount(string name, string code, PromoKind kind, decimal value, DateTime? expiresAt, bool active)
    {
        Name = name; Code = code; Kind = kind; Value = value; ExpiresAt = expiresAt; Active = active;
    }

    public bool Matches(DiscountContext ctx) 
        => !string.IsNullOrEmpty(ctx.PromoCode) && string.Equals(ctx.PromoCode, Code, StringComparison.OrdinalIgnoreCase);
    
    public bool IsValid(DiscountContext ctx)
    {
        if (!Active || ctx.PromoApplied) return false;
        if (ExpiresAt is { } exp && ctx.Order.CreatedAt.Date > exp.Date) return false;
        return true;
    }

    public decimal PeekAmount(DiscountContext ctx)
    {
        if (!Matches(ctx) || !IsValid(ctx)) return 0m;
        return Kind switch
        {
            PromoKind.Percent => Money.Round(ctx.CurrentItemsTotal * Value / 100m),
            PromoKind.Fixed => Math.Min(Value, ctx.CurrentItemsTotal),
            _ => 0m
        };
    }

    public void Apply(DiscountContext ctx)
    {
        if (!Matches(ctx) || !IsValid(ctx)) return;
        var amount = PeekAmount(ctx);
        var actually = ctx.ReduceItems(amount);
        if (actually > 0)
        {
            ctx.AddDiscount(Name, actually, $"Промокод {Code}");
            ctx.MarkPromoApplied();
        }
    }
}