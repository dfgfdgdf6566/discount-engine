namespace DiscountEngine.Engine;
using DiscountEngine.Discounts;
using DiscountEngine.Models;

public class PricingEngine
{
    public PricingResult Calculate(Order order, IReadOnlyList<IDiscount> configured, IReadOnlyList<PromoCodeDiscount> promos)
    {
        var ctx = new DiscountContext(order);
        decimal baseTotal = Money.Round(ctx.CurrentItemsTotal + ctx.CurrentDeliveryCost);

        // 1. "3 по цене 2" идёт первым
        foreach (var d in configured.OfType<ThreeForTwoDiscount>()) 
            d.Apply(ctx);

        // 2. Проценты vs процентный промокод
        ApplyPercentStage(ctx, configured, promos);

        // 3. Лояльность и Первый заказ
        foreach (var d in configured) 
            if (d is LoyaltyDiscount or FirstOrderDiscount) 
                d.Apply(ctx);

        // 4. Фиксированные + фиксированный промокод
        foreach (var d in configured.OfType<FixedDiscount>()) 
            d.Apply(ctx);
        foreach (var p in promos) 
            if (p.Kind == PromoKind.Fixed) 
                p.Apply(ctx);

        // 5. Бесплатная доставка в самом конце
        foreach (var d in configured.OfType<FreeDeliveryDiscount>()) 
            d.Apply(ctx);

        return new PricingResult
        {
            BaseTotal = baseTotal,
            AppliedDiscounts = ctx.Applied,
            FinalTotal = Money.Round(ctx.CurrentItemsTotal + ctx.CurrentDeliveryCost)
        };
    }

    private static void ApplyPercentStage(DiscountContext ctx, IReadOnlyList<IDiscount> configured, IReadOnlyList<PromoCodeDiscount> promos)
    {
        var ordinary = configured.OfType<PercentDiscount>().Cast<IDiscount>().ToList();
        
        // Ищем подходящий процентный промокод
        var percentPromo = promos.FirstOrDefault(p => p.Kind == PromoKind.Percent && !string.IsNullOrEmpty(ctx.PromoCode)
                              && string.Equals(p.Code, ctx.PromoCode, StringComparison.OrdinalIgnoreCase)
                              && p.Active && (p.ExpiresAt is null || ctx.Order.CreatedAt.Date <= p.ExpiresAt.Value.Date));

        // Считаем выгоду обычных процентов на временной копии
        decimal ordinaryGain = 0m;
        {
            var snapCtx = new DiscountContext(ctx.Order);
            snapCtx.ReduceItems(snapCtx.CurrentItemsTotal - ctx.CurrentItemsTotal);
            foreach (var d in ordinary) 
            { 
                var before = snapCtx.CurrentItemsTotal; 
                d.Apply(snapCtx); 
                ordinaryGain += before - snapCtx.CurrentItemsTotal; 
            }
        }

        // Считаем выгоду промокода
        decimal promoGain = percentPromo?.PeekAmount(ctx) ?? 0m;

        // При равенстве выгоды выбираем промокод (>=)
        if (percentPromo is not null && promoGain >= ordinaryGain && promoGain > 0)
            percentPromo.Apply(ctx); 
        else
            foreach (var d in ordinary) d.Apply(ctx);
    }
}