using MarkovApp;
using MarkovApp.Configuration;
using Microsoft.Extensions.Options;

namespace MarkovApp.Tests;

public static class TestHelper
{
    private static bool _initialized;
    private static readonly object _lock = new();

    public static void EnsureConfigurationInitialized()
    {
        if (_initialized) return;

        lock (_lock)
        {
            if (_initialized) return;

            var graphSettings = Options.Create(new GraphSettings
            {
                MaxNodes = 12,
                NodeRadius = 20.0,
                MinNodeDistance = 50.0,
                MinEdgeDistance = 50.0
            });

            var calcSettings = Options.Create(new CalculationSettings
            {
                DefaultMaxIterations = 1000,
                ConvergenceEpsilon = 0.0001
            });

            AppConfig.Initialize(graphSettings, calcSettings);
            _initialized = true;
        }
    }
}
