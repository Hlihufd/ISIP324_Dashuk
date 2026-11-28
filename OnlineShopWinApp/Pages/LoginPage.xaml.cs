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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PassBox.Password;
            if(login == "" || password == "")
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            using (var db = new CinemaDBEntities()) { 
                var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
                if (user == null) {
                    MessageBox.Show("Неверный логин или пароль");
                    return;
                }
                CurrentUser.User = user;
            }
            NavigationService.Navigate(new ProfPage());
        }

        private void RegistrBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
