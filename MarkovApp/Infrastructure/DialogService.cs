using MarkovApp.Models;
using MarkovApp.ViewModels;
using MarkovApp.Views;
using Microsoft.Win32;
using System.Windows;

namespace MarkovApp.Infrastructure
{
    public class DialogService : IDialogService
    {
        public bool? ShowNodeProperties(NodePropertiesViewModel vm)
        {
            var dialog = new NodeProperties(vm);
            return dialog.ShowDialog();
        }

        public bool? ShowEdgeProperties(EdgePropertiesViewModel vm)
        {
            var dialog = new EdgeProperties(vm);
            return dialog.ShowDialog();
        }

        public bool? ShowRegularMatrixResult(RegularMatrixResultViewModel vm)
        {
            var dialog = new RegularMatrixResultDisplay(vm);
            return dialog.ShowDialog();
        }

        public bool? ShowAbsorbingMatrixResult(AbsorbingMatrixResultViewModel vm)
        {
            var dialog = new AbsorbingMatrixResultDisplay(vm);
            return dialog.ShowDialog();
        }

        public CalculationData? ShowManualMatrixInput(ManualMatrixViewModel vm)
        {
            var dialog = new ManualMatrixInput(vm);
            return dialog.ShowDialog() == true ? vm.GetCalculationData() : null;
        }

        public int? ShowMaxIterationsInput(MaxIterationsViewModel vm)
        {
            var dialog = new MaxIterationsInput(vm);
            return dialog.ShowDialog() == true ? vm.MaxIterations : null;
        }

        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        public string? ShowSaveFileDialog()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json"
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public string? ShowLoadFileDialog()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json"
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
