namespace DiscountEngine.Factory;
using DiscountEngine.Discounts;

public sealed class ConfigProvider
{
    private static readonly Lazy<ConfigProvider> _instance = new(() => new ConfigProvider());
    public static ConfigProvider Instance => _instance.Value;

    private readonly DiscountFactory _factory = new();
    private List<Dictionary<string, object>> _configs = new();
    private List<Dictionary<string, object>> _promoConfigs = new();

    private ConfigProvider() { }

    public void LoadConfiguration(List<Dictionary<string, object>> configs, List<Dictionary<string, object>>? promos = null)
    {
        _configs = configs;
        _promoConfigs = promos ?? new();
    }

    public IReadOnlyList<IDiscount> BuildDiscounts() => _configs.Select(c => _factory.Create((string)c["type"], c)).ToList();
    public IReadOnlyList<PromoCodeDiscount> BuildPromos() => _promoConfigs.Select(c => _factory.CreatePromo(c)).ToList();
}