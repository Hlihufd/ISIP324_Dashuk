using System.Windows;
using System.Windows.Controls;

namespace OnlineShopWinApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SearchBox.TextChanged += SearchBox_TextChanged;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == "🔍Введите название..." && textBox.Text.Trim() != string.Empty)
            {
                textBox.Text = string.Empty; 
            }
        }

    }
}
