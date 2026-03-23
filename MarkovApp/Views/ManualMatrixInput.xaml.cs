using System.Windows;
using MarkovApp.ViewModels;

namespace MarkovApp.Views
{
    public partial class ManualMatrixInput : Window
    {
        public ManualMatrixInput(ManualMatrixViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.RequestClose += (s, result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}