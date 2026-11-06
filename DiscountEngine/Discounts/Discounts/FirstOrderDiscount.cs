namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public class FirstOrderDiscount : IDiscount
{
    public string Name { get; }
    private readonly decimal _percent;

    public FirstOrderDiscount(string name, decimal percent) { Name = name; _percent = percent; }

    public void Apply(DiscountContext ctx)
    {
        if (!ctx.Order.Customer.IsFirstOrder) return;
        
        var amount = Money.Round(ctx.CurrentItemsTotal * _percent / 100m);
        var actually = ctx.ReduceItems(amount);
        if (actually > 0) 
            ctx.AddDiscount(Name, actually, "Скидка на первый заказ");
    }
}