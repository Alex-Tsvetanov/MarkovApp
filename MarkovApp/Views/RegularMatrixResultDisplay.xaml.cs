using MarkovApp.ViewModels;
using System.Windows;

namespace MarkovApp.Views
{
    public partial class RegularMatrixResultDisplay : Window
    {
        public RegularMatrixResultDisplay(RegularMatrixResultViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
