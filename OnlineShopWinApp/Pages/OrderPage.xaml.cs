using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OnlineShopWinApp.Pages
{
    public partial class OrderPage : Page
    {
        private List<CartItems> cartItems;
        private decimal total;

        public OrderPage()
        {
            InitializeComponent();
            LoadOrder();
        }

        private void LoadOrder()
        {
            cartItems = DB.Context.CartItems
                .Include(x => x.Products)
                .ToList();
            OrderList.ItemsSource = cartItems;
            total = cartItems.Sum(x => x.Products.Price * x.Quantity);
            TotalText.Text = "Итого: " + total + " руб";
        }

        private void OrderButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NameError.Visibility = Visibility.Collapsed;
            AdressError.Visibility = Visibility.Collapsed;
            PostError.Visibility = Visibility.Collapsed;

            bool ok = true;

            if (string.IsNullOrWhiteSpace(TextName.Text))
            {
                NameError.Text = "Введите ФИО";
                NameError.Visibility = Visibility.Visible;
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(TextAdress.Text))
            {
                AdressError.Text = "Введите адрес";
                AdressError.Visibility = Visibility.Visible;
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(TextPost.Text))
            {
                PostError.Text = "Введите почту";
                PostError.Visibility = Visibility.Visible;
                ok = false;
            }
            else if (!TextPost.Text.Contains("@"))
            {
                PostError.Text = "Почта должна содержать @";
                PostError.Visibility = Visibility.Visible;
                ok = false;
            }

            if (!ok) return;

            var order = new Orders()
            {
                FullName = TextName.Text,
                Address = TextAdress.Text,
                Email = TextPost.Text,
                TotalPrice = total,
                CreatedAt = DateTime.Now
            };

            foreach(var item in cartItems)
            {
                order.OrderItems.Add(new OrderItems{
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Products.Price
                });
            }

            try
            {
                DB.Context.Orders.Add(order);
                foreach (var item in cartItems)
                {
                    DB.Context.CartItems.Remove(item);
                }
                DB.Context.SaveChanges();

                MessageBox.Show("Заказ оформлен! Спасибо за покупку.");
                NavigationService.Navigate(new ProductsPage());
            }
            catch(Exception ex)
            {
                DB.Context.Orders.Remove(order);  
                MessageBox.Show("Не удалось оформить заказ: " + ex.Message);
            }
        }
    }
}