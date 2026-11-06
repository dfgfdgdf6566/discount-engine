namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public class FreeDeliveryDiscount : IDiscount
{
    public string Name { get; }
    private readonly decimal _threshold;

    public FreeDeliveryDiscount(string name, decimal threshold) { Name = name; _threshold = threshold; }

    public void Apply(DiscountContext ctx)
    {
        if (ctx.CurrentItemsTotal <= _threshold) return; // Строго больше!
        
        var actually = ctx.ReduceDelivery(ctx.CurrentDeliveryCost);
        if (actually > 0) 
            ctx.AddDiscount(Name, actually, $"Бесплатная доставка (сумма {ctx.CurrentItemsTotal} > {_threshold})");
    }
}