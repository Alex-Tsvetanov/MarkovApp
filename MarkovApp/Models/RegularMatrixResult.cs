namespace MarkovApp.Models
{
    public class RegularMatrixResult
    {
        public double[,] TransitionMatrix { get; init; }
        public double[] LimitingProbabilities { get; init; }
        public double[] ResultVector { get; init; }
        public RegularMatrixResult(double[,] matrix, double[] limiting, double[] result)
        {
            TransitionMatrix = matrix;
            LimitingProbabilities = limiting;
            ResultVector = result;
        }
    }
}
