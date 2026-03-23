namespace MarkovApp.Api.DTOs
{
    public class RegularChainResponseDto
    {
        public double[] SteadyState { get; set; } = default!;

        public double[] ResultVector { get; set; } = default!;
    }
}
