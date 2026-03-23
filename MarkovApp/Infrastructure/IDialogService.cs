using MarkovApp.Models;
using MarkovApp.ViewModels;

namespace MarkovApp.Infrastructure
{
    public interface IDialogService
    {
        bool? ShowNodeProperties(NodePropertiesViewModel vm);
        bool? ShowEdgeProperties(EdgePropertiesViewModel vm);
        bool? ShowRegularMatrixResult(RegularMatrixResultViewModel vm);
        bool? ShowAbsorbingMatrixResult(AbsorbingMatrixResultViewModel vm);
        CalculationData? ShowManualMatrixInput(ManualMatrixViewModel vm);
        int? ShowMaxIterationsInput(MaxIterationsViewModel vm);
        bool ShowConfirmation(string title, string message);
        string? ShowSaveFileDialog();
        string? ShowLoadFileDialog();
        void ShowError(string message);
    }
}
