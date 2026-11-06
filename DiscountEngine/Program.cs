using DiscountEngine.Discounts;
using DiscountEngine.Engine;
using DiscountEngine.Factory;
using DiscountEngine.Models;

var book1 = new Product("p1", "Книга A", 500m, "Книги");
var book2 = new Product("p2", "Книга B", 400m, "Книги");
var pen = new Product("p3", "Ручка", 100m, "Канцтовары");

var order = new Order
{
    Id = "ORD-1",
    Customer = new Customer { Id = "u1", LastPurchaseDate = DateTime.Today.AddDays(-12), IsFirstOrder = true },
    Items = new List<CartItem> { new(book1, 2), new(book2, 2), new(pen, 3) },
    PromoCode = "SALE10",
    CreatedAt = DateTime.Today,
    DeliveryCost = 300m
};

ConfigProvider.Instance.LoadConfiguration(
    new List<Dictionary<string, object>>
    {
        new() { ["type"] = "threeForTwo", ["category"] = "Книги", ["name"] = "3 по цене 2" },
        new() { ["type"] = "percent", ["value"] = 10m, ["name"] = "Осенняя распродажа" },
        new() { ["type"] = "fixed", ["value"] = 500m, ["threshold"] = 3000m, ["name"] = "500 от 3000" },
        new() { ["type"] = "loyalty", ["percent"] = 5m, ["days"] = 30, ["name"] = "Лояльность" },
        new() { ["type"] = "freeDelivery", ["threshold"] = 5000m, ["name"] = "Бесплатная доставка" }
    },
    new List<Dictionary<string, object>> 
    { 
        new() { ["code"] = "SALE10", ["kind"] = "percent", ["value"] = 10m, ["name"] = "Промокод SALE10", ["active"] = true } 
    });

var provider = ConfigProvider.Instance;
var engine = new PricingEngine();
var result = engine.Calculate(order, provider.BuildDiscounts(), provider.BuildPromos());

Console.WriteLine($"BaseTotal: {result.BaseTotal}");
foreach (var d in result.AppliedDiscounts) 
    Console.WriteLine($"- {d.Name}: {d.Amount} ({d.Reason})");
Console.WriteLine($"FinalTotal: {result.FinalTotal}");