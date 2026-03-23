using System.Windows;
using MarkovApp.ViewModels;

namespace MarkovApp.Views
{
    public partial class EdgeProperties : Window
    {
        public EdgeProperties(EdgePropertiesViewModel vm)
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