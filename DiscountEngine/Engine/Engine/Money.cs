namespace DiscountEngine.Engine;

public static class Money
{
    // Банковское округление: round half to even
    public static decimal Round(decimal value)
        => decimal.Round(value, 2, MidpointRounding.ToEven);
}
