namespace DiscountEngine.Engine;
using DiscountEngine.Models;

public class DiscountContext
{
    public Order Order { get; }
    public decimal CurrentItemsTotal { get; private set; }
    public decimal CurrentDeliveryCost { get; private set; }
    public List<AppliedDiscount> Applied { get; } = new();
    public string? PromoCode => Order.PromoCode;
    public bool PromoApplied { get; private set; }

    public DiscountContext(Order order)
    {
        Order = order;
        CurrentItemsTotal = order.Items.Sum(i => i.LineTotal);
        CurrentDeliveryCost = order.DeliveryCost;
    }

    public decimal ReduceItems(decimal amount)
    {
        if (amount <= 0) return 0m;
        amount = Math.Min(amount, CurrentItemsTotal);
        amount = Money.Round(amount);
        CurrentItemsTotal = Money.Round(CurrentItemsTotal - amount);
        return amount;
    }

    public decimal ReduceDelivery(decimal amount)
    {
        if (amount <= 0) return 0m;
        amount = Math.Min(amount, CurrentDeliveryCost);
        amount = Money.Round(amount);
        CurrentDeliveryCost = Money.Round(CurrentDeliveryCost - amount);
        return amount;
    }

    public void AddDiscount(string name, decimal amount, string reason)
    {
        if (amount <= 0) return;
        Applied.Add(new AppliedDiscount { Name = name, Amount = Money.Round(amount), Reason = reason });
    }
    public void MarkPromoApplied() => PromoApplied = true;
}