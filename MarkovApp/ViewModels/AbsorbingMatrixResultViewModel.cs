using MarkovApp.Models;
using MarkovApp.Utilities;

namespace MarkovApp.ViewModels
{
    public class AbsorbingMatrixResultViewModel
    {
        public AbsorbingMatrixResult Result { get; }

        public string AverageTransitionsText =>
            NumericHelper.ToString(Result.AverageTransitions);

        public string ProbabilitiesText =>
            NumericHelper.ToString(Result.Probabilities, Result.AbsorbingStates.Select(i => i + 1));

        public AbsorbingMatrixResultViewModel(AbsorbingMatrixResult result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }
}
