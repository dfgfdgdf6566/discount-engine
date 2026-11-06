namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public class FixedDiscount : IDiscount
{
    public string Name { get; }
    private readonly decimal _value;
    private readonly decimal _threshold;

    public FixedDiscount(string name, decimal value, decimal threshold)
    {
        if (value < 0) throw new ArgumentException("value < 0");
        Name = name; _value = value; _threshold = threshold;
    }

    public void Apply(DiscountContext ctx)
    {
        if (ctx.CurrentItemsTotal < _threshold) return;
        var actually = ctx.ReduceItems(_value);
        if (actually > 0) 
            ctx.AddDiscount(Name, actually, $"Фикс. скидка {_value} ₽ (порог {_threshold})");
    }
}