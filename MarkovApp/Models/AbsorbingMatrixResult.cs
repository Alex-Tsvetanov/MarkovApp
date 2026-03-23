namespace MarkovApp.Models
{
    public class AbsorbingMatrixResult
    {
        public double[,] AverageTransitions { get; init; }
        public double[,] Probabilities { get; init; }
        public List<int> AbsorbingStates { get; init; }
        public AbsorbingMatrixResult(double[,] avg, double[,] prob, List<int> absorbing)
        {
            AverageTransitions = avg;
            Probabilities = prob;
            AbsorbingStates = absorbing;
        }
    }
}
