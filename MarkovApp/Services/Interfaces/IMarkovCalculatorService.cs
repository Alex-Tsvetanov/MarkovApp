using MarkovApp.Models;

namespace MarkovApp.Services.Interfaces
{
    public interface IMarkovCalculatorService
    {
        RegularMatrixResult CalculateRegularChain(CalculationData data, int? maxIterations = 1000);
        AbsorbingMatrixResult CalculateAbsorbingChain(CalculationData data);
        bool IsAbsorbingMatrix(double[,] transitionMatrix);
    }
}
