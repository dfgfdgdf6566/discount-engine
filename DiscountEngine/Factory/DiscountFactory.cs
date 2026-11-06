namespace DiscountEngine.Factory;
using DiscountEngine.Discounts;

public class DiscountFactory
{
    private readonly Dictionary<string, Func<Dictionary<string, object>, IDiscount>> _creators = new(StringComparer.OrdinalIgnoreCase);

    public DiscountFactory()
    {
        _creators["percent"] = cfg => new PercentDiscount(GetStr(cfg, "name", "Процентная"), GetDec(cfg, "value"));
        _creators["fixed"] = cfg => new FixedDiscount(GetStr(cfg, "name", "Фиксированная"), GetDec(cfg, "value"), GetDec(cfg, "threshold", 0m));
        _creators["threeForTwo"] = cfg => new ThreeForTwoDiscount(GetStr(cfg, "name", "3 по цене 2"), GetStr(cfg, "category"));
        _creators["loyalty"] = cfg => new LoyaltyDiscount(GetStr(cfg, "name", "Лояльность"), GetDec(cfg, "percent", 5m), (int)GetDec(cfg, "days", 30m));
        _creators["freeDelivery"] = cfg => new FreeDeliveryDiscount(GetStr(cfg, "name", "Бесплатная доставка"), GetDec(cfg, "threshold", 0m));
        _creators["firstOrder"] = cfg => new FirstOrderDiscount(GetStr(cfg, "name", "Первый заказ"), GetDec(cfg, "percent", 10m));
    }

    public IDiscount Create(string type, Dictionary<string, object> config)
    {
        if (!_creators.TryGetValue(type, out var creator)) throw new ArgumentException($"Unknown discount type '{type}'");
        return creator(config);
    }

    public PromoCodeDiscount CreatePromo(Dictionary<string, object> cfg)
    {
        var kind = GetStr(cfg, "kind") == "percent" ? PromoKind.Percent : PromoKind.Fixed;
        DateTime? expires = cfg.TryGetValue("expiresAt", out var e) && e is DateTime dt ? dt : null;
        return new PromoCodeDiscount(GetStr(cfg, "name", "Промокод"), GetStr(cfg, "code"), kind, GetDec(cfg, "value"), expires, GetBool(cfg, "active", true));
    }

    private static string GetStr(Dictionary<string, object> cfg, string key, string? def = null)
        => cfg.TryGetValue(key, out var v) && v is string s ? s : def ?? throw new Exception($"Missing {key}");
    private static decimal GetDec(Dictionary<string, object> cfg, string key, decimal? def = null)
        => cfg.TryGetValue(key, out var v) ? Convert.ToDecimal(v) : def ?? throw new Exception($"Missing {key}");
    private static bool GetBool(Dictionary<string, object> cfg, string key, bool def = false)
        => cfg.TryGetValue(key, out var v) && v is bool b ? b : def;
}