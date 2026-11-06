using DiscountEngine.Discounts;
using DiscountEngine.Engine;
using DiscountEngine.Models;
using Xunit;

public class StrategyTests
{
    // Вспомогательная функция, чтобы быстро создавать заказы в тестах
    private static Order MakeOrder(decimal price, int qty = 1, bool firstOrder = false, 
        DateTime? lastPurchase = null, string category = "X", string? promo = null, 
        decimal delivery = 0m, DateTime? created = null)
    {
        var p = new Product("1", "P", price, category);
        return new Order 
        { 
            Id = "o", 
            CreatedAt = created ?? new DateTime(2024, 6, 15), 
            PromoCode = promo, 
            DeliveryCost = delivery,
            Customer = new Customer { IsFirstOrder = firstOrder, LastPurchaseDate = lastPurchase }, 
            Items = new List<CartItem> { new(p, qty) } 
        };
    }

    [Fact]
    public void Percent_Applies() 
    { 
        var ctx = new DiscountContext(MakeOrder(1000m)); 
        new PercentDiscount("10%", 10m).Apply(ctx); 
        Assert.Equal(900m, ctx.CurrentItemsTotal); 
    }

    [Fact]
    public void Fixed_BelowThreshold_DoesNothing() 
    { 
        var ctx = new DiscountContext(MakeOrder(1000m)); 
        new FixedDiscount("fix", 500m, 3000m).Apply(ctx); 
        Assert.Empty(ctx.Applied); 
    }

    [Fact]
    public void ThreeForTwo_OneFreeForThree() 
    { 
        var ctx = new DiscountContext(MakeOrder(100m, qty: 3, category: "Книги")); 
        new ThreeForTwoDiscount("3/2", "Книги").Apply(ctx); 
        Assert.Equal(100m, ctx.Applied.Single().Amount); 
    }

    [Fact]
    public void Loyalty_Exactly30Days_Applies() 
    { 
        var created = new DateTime(2024, 6, 30); 
        var ctx = new DiscountContext(MakeOrder(1000m, lastPurchase: created.AddDays(-30), created: created)); 
        new LoyaltyDiscount("L", 5m, 30).Apply(ctx); 
        Assert.Single(ctx.Applied); 
    }

    [Fact]
    public void FreeDelivery_StrictlyGreater() 
    { 
        var ctx = new DiscountContext(MakeOrder(5000m, delivery: 300m)); 
        new FreeDeliveryDiscount("FD", 5000m).Apply(ctx); 
        Assert.Empty(ctx.Applied); // Ровно 5000 не срабатывает
        
        var ctx2 = new DiscountContext(MakeOrder(5001m, delivery: 300m)); 
        new FreeDeliveryDiscount("FD", 5000m).Apply(ctx2); 
        Assert.Equal(300m, ctx2.Applied.Single().Amount); 
    }
}