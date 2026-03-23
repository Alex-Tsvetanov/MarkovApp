namespace MarkovApp.Api.DTOs
{
    public class AbsorbingChainResponseDto
    {
        public double[,] FundamentalMatrix { get; set; } = default!;

        public double[,] AbsorptionProbabilities { get; set; } = default!;

        public List<int> AbsorbingStates { get; set; } = new();
    }
}
