using MarkovApp.Configuration;
using Microsoft.Extensions.Options;

namespace MarkovApp;

public static class AppConfig
{
    private static GraphSettings? _graphSettings;
    private static CalculationSettings? _calculationSettings;

    public static void Initialize(IOptions<GraphSettings> graphSettings, IOptions<CalculationSettings> calculationSettings)
    {
        _graphSettings = graphSettings.Value;
        _calculationSettings = calculationSettings.Value;
    }

    public static GraphSettings Graph => _graphSettings ?? throw new InvalidOperationException("Configuration not initialized");
    public static CalculationSettings Calculation => _calculationSettings ?? throw new InvalidOperationException("Configuration not initialized");
}
