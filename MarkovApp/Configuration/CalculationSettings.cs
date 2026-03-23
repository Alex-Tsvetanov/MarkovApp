namespace MarkovApp.Configuration;

public class CalculationSettings
{
    public int DefaultMaxIterations { get; set; } = 1000;
    public double ConvergenceEpsilon { get; set; } = 0.0001;
}
