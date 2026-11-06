namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public class LoyaltyDiscount : IDiscount
{
    public string Name { get; }
    private readonly decimal _percent;
    private readonly int _days;

    public LoyaltyDiscount(string name, decimal percent, int days) { Name = name; _percent = percent; _days = days; }

    public void Apply(DiscountContext ctx)
    {
        var last = ctx.Order.Customer.LastPurchaseDate;
        if (last is null) return;
        
        var created = ctx.Order.CreatedAt.Date;
        var lastDate = last.Value.Date;
        
        if (lastDate > created) return; // Будущее — некорректно
        
        var diff = (created - lastDate).Days;
        if (diff > _days) return; // Слишком давно
        
        var amount = Money.Round(ctx.CurrentItemsTotal * _percent / 100m);
        var actually = ctx.ReduceItems(amount);
        if (actually > 0) 
            ctx.AddDiscount(Name, actually, $"Последняя покупка была {diff} дн. назад");
    }
}