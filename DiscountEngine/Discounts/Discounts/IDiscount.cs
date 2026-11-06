namespace DiscountEngine.Discounts;
using DiscountEngine.Engine;

public interface IDiscount
{
    string Name { get; }
    void Apply(DiscountContext ctx);
}

public interface IPeekableDiscount : IDiscount
{
    decimal PeekAmount(DiscountContext ctx);
}