using System.Windows;
using System.Windows.Controls;
using CarConfigurator.ViewModels;

namespace CarConfigurator.Views.Pages
{
    public partial class Step2Page : UserControl
    {
        public Step2Page()
        {
            InitializeComponent();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ((Step2ViewModel)DataContext).Update();
        }
    }
}