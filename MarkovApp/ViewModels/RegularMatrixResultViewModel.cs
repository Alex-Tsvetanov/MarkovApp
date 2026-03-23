using MarkovApp.Models;
using MarkovApp.Utilities;

namespace MarkovApp.ViewModels
{
    public class RegularMatrixResultViewModel
    {
        public RegularMatrixResult Result { get; }
        public string MatrixText =>
            NumericHelper.ToString(Result.TransitionMatrix);
        public string LimitingProbabilitiesText =>
            NumericHelper.VectorToString(Result.LimitingProbabilities, 3);
        public string ResultVectorText =>
            NumericHelper.VectorToString(Result.ResultVector, 2);
        public RegularMatrixResultViewModel(RegularMatrixResult result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }
}
