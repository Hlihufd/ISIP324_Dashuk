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
using System.Windows.Threading;

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
            using (var db = new OnlineShopEntities1())
            {
                ProductsList.ItemsSource = db.Products.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.Content = "Товар добавлен";
            var product = (sender as Button).DataContext as Products;
            var item = DB.Context.CartItems.FirstOrDefault(x => x.ProductId == product.Id);
            if (item == null)
            {
                var CarItem = new CartItems();
                CarItem.ProductId = product.Id;
                CarItem.Quantity = 1;
                DB.Context.CartItems.Add(CarItem);
            }
            else
            {
                item.Quantity += 1;
            }
            DB.Context.SaveChanges();
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, args) =>
            {
                btn.Content = "Добавить в корзину";
                timer.Stop();
            };
            timer.Start();
        }
    }
}
