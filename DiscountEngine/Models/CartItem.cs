namespace DiscountEngine.Models;

public class CartItem
{
    public Product Product { get; init; }
    public int Quantity { get; init; }

    public CartItem(Product product, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");
        Product = product; 
        Quantity = quantity;
    }
    public decimal LineTotal => Product.BasePrice * Quantity;
}