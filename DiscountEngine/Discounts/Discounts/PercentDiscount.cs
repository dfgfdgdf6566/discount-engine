namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public class PercentDiscount : IPeekableDiscount
{
    public string Name { get; }
    private readonly decimal _percent;

    public PercentDiscount(string name, decimal percent)
    {
        if (percent < 0) throw new ArgumentException("percent < 0");
        Name = name; _percent = percent;
    }
    
    // Показывает, сколько скидка снимет (нужно для сравнения с промокодом)
    public decimal PeekAmount(DiscountContext ctx) 
        => Money.Round(ctx.CurrentItemsTotal * _percent / 100m);
    
    public void Apply(DiscountContext ctx)
    {
        var amount = PeekAmount(ctx);
        var actually = ctx.ReduceItems(amount);
        if (actually > 0) 
            ctx.AddDiscount(Name, actually, $"Скидка {_percent}% на товары");
    }
}