using MarkovApp.ViewModels;
using System.Windows;

namespace MarkovApp.Views
{
    public partial class NodeProperties : Window
    {
        public NodeProperties(NodePropertiesViewModel vm)
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