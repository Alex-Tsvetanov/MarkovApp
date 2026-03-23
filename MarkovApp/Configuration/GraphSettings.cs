namespace MarkovApp.Configuration;

public class GraphSettings
{
    public int MaxNodes { get; set; } = 12;
    public double NodeRadius { get; set; } = 20.0;
    public double MinNodeDistance { get; set; } = 50.0;
    public double MinEdgeDistance { get; set; } = 50.0;
}
