using MarkovApp.Models;

namespace MarkovApp.Services.Interfaces
{
    public interface IValidationService
    {
        bool IsProbabilityValid(double value);
        bool ValidateNode(Node node, out string errorMessage);
        bool ValidateEdge(Edge edge, out string errorMessage);
        bool ValidateMaxIterations(int maxIterations, out string errorMessage);
        bool ValidateCalculationData(CalculationData data, out string? errorMessage);
        string? ValidateAppState(AppState state);
    }
}
