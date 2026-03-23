using MarkovApp.ViewModels;
using System.Windows;

namespace MarkovApp.Views
{
    public partial class AbsorbingMatrixResultDisplay : Window
    {
        public AbsorbingMatrixResultDisplay(AbsorbingMatrixResultViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
