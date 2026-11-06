using DiscountEngine.Discounts;
using DiscountEngine.Engine;
using DiscountEngine.Models;
using Xunit;

public class EngineTests
{
    [Fact]
    public void ThreeForTwo_ThenPercent_OrderMatters()
    {
        // 6 книг по 100 = 600. Скидка 3-по-2 снимает 200 (остается 400). 
        // 10% от 400 = 40. Итог: 360.
        var p = new Product("p", "P", 100m, "Книги");
        var order = new Order { Id = "o", CreatedAt = DateTime.Today, DeliveryCost = 0m, Customer = new Customer(), Items = new List<CartItem> { new(p, 6) } };
        
        var engine = new PricingEngine();
        var result = engine.Calculate(order, 
            new List<IDiscount> { new ThreeForTwoDiscount("3/2", "Книги"), new PercentDiscount("10%", 10m) }, 
            new List<PromoCodeDiscount>());
            
        Assert.Equal(360m, result.FinalTotal); 
        Assert.Equal(200m, result.AppliedDiscounts[0].Amount); // Сначала 3-по-2
        Assert.Equal(40m, result.AppliedDiscounts[1].Amount);  // Потом 10%
    }

    [Fact]
    public void Promo_BetterThanPercent_Chosen()
    {
        // Товар 1000. Обычная скидка 10% = 100. Промокод 20% = 200.
        // Должен выбраться промокод (он выгоднее).
        var p = new Product("p", "P", 1000m, "X");
        var order = new Order { Id = "o", CreatedAt = DateTime.Today, PromoCode = "BIG", Customer = new Customer(), Items = new List<CartItem> { new(p, 1) } };

        var engine = new PricingEngine();
        var result = engine.Calculate(order, 
            new List<IDiscount> { new PercentDiscount("10%", 10m) }, 
            new List<PromoCodeDiscount> { new("Промо 20%", "BIG", PromoKind.Percent, 20m, null, true) });

        Assert.Single(result.AppliedDiscounts); // Применилась только одна
        Assert.Equal(200m, result.AppliedDiscounts[0].Amount);
        Assert.Contains("Промо", result.AppliedDiscounts[0].Name);
    }
}