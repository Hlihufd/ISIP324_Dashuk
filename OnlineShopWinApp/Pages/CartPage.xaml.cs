using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
namespace OnlineShopWinApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            LoadCart();
            CartList.ItemsSource = DB.Context.CartItems.ToList();
        }

        private void LoadCart()
        {
            var items = DB.Context.CartItems
                          .Include(c => c.Products)
                          .ToList();

            CartList.ItemsSource = items;


            decimal total = items.Sum(c => c.Products.Price * c.Quantity);
        }

        private void RemoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.Content = "Товар удален";

            var pos = (sender as Button).DataContext as CartItems;

            if (pos.Quantity > 1)
                pos.Quantity -= 1;
            else
                DB.Context.CartItems.Remove(pos);

            DB.Context.SaveChanges();
            LoadCart();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, args) =>
            {
                btn.Content = "Удалить товар";
                timer.Stop();
            };
            timer.Start();
        }
    }
}
