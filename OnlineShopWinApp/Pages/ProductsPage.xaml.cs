using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineShopWinApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            using (var db = new OnlineShopEntities())
            {
                ProductsList.ItemsSource = db.Products.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Products;
        }
    }
}
