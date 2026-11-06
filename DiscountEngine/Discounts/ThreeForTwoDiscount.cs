namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public class ThreeForTwoDiscount : IDiscount
{
    public string Name { get; }
    private readonly string _category;

    public ThreeForTwoDiscount(string name, string category) { Name = name; _category = category; }

    public void Apply(DiscountContext ctx)
    {
        var prices = new List<decimal>();
        int totalUnits = 0;
        
        // Собираем все цены товаров нужной категории
        foreach (var item in ctx.Order.Items)
        {
            if (!string.Equals(item.Product.Category, _category, StringComparison.OrdinalIgnoreCase)) 
                continue;
            totalUnits += item.Quantity;
            for (int i = 0; i < item.Quantity; i++) 
                prices.Add(item.Product.BasePrice);
        }
        
        int freeCount = totalUnits / 3; // Сколько бесплатных единиц
        if (freeCount == 0) return;
        
        prices.Sort(); // Сортируем, чтобы бесплатными стали самые дешевые
        decimal amount = 0m;
        for (int i = 0; i < freeCount; i++) 
            amount += prices[i];
            
        var actually = ctx.ReduceItems(amount);
        if (actually > 0) 
            ctx.AddDiscount(Name, actually, $"Категория {_category}: {totalUnits} ед., {freeCount} бесплатно");
    }
}