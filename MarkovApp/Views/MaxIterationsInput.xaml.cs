using System.Windows;
using MarkovApp.ViewModels;

namespace MarkovApp.Views
{
    public partial class MaxIterationsInput : Window
    {
        public MaxIterationsInput(MaxIterationsViewModel vm)
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