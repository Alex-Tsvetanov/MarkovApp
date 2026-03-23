using System.ComponentModel.DataAnnotations;

namespace MarkovApp.Api.DTOs
{
    public class MarkovRequestDto
    {
        [Required(ErrorMessage = "Transition matrix is required")]
        public double[][] TransitionMatrix { get; set; } = default!;

        [Required(ErrorMessage = "Initial state vector is required")]
        public double[] InitialStateVector { get; set; } = default!;

        public bool IsAbsorbing { get; set; }

        [Range(1, 1_000_000, ErrorMessage = "Max iterations must be between 1 and 1,000,000")]
        public int? MaxIterations { get; set; }
    }
}
